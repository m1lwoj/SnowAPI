using EmailSender;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SnowBLL.Enums;
using SnowBLL.Helpers;
using SnowBLL.Models;
using SnowBLL.Models.Auth;
using SnowBLL.Service.Interfaces;
using SnowDAL.Repositories.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SnowBLL.Service.Concrete
{
    public class AuthBLService : BLService, IAuthBLService
    {
        #region Members

        private const string HASHKEY = "KFWPANVZJE";
        private IUserRepository _userRepository;
        private JwtIssuerOptions _jwtOptions;

        #endregion Members

        #region Constructor

        public AuthBLService(ILogger<BLService> logger, IOptions<JwtIssuerOptions> jwtOptions, IUserRepository repository) : base(logger)
        {
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);
            this._userRepository = repository;
        }

        #endregion Constructor

        #region Public Methods

        //TODO API-19 validation of models 
        public async Task<Result<AuthorizeResponseModel>> Authorize(ApplicationUserModel model)
        {
            try
            {
                var hashedPassword = CryptographyHelper.Hash(model.Password, HASHKEY + model.Email);
                var user = await _userRepository.GetSingle(u => u.Email == model.Email && u.HashedPassword == hashedPassword);

                if (user != null)
                {
                    user.LastLogin = DateTime.Now;
                    _userRepository.Update(user);
                    var role = await GetRole(model.Email);
                    string encodedJwt = await GetJWTToken(model, role);
                    user.LastLogin = DateTime.Now;

                    var response = new AuthorizeResponseModel()
                    {
                        AccessToken = encodedJwt,
                        Expires = (int)_jwtOptions.ValidFor.TotalSeconds
                    };

                    return new Result<AuthorizeResponseModel>(response);
                }
                else
                {
                    ErrorResult error = GenerateError("User not found", "Email", "Invalid email", ErrorStatus.ObjectNotFound);
                    return new Result<AuthorizeResponseModel>(error);
                }
            }
            catch (Exception ex)
            {
                ErrorResult error = GenerateError(ex);
                return new Result<AuthorizeResponseModel>(error);
            }
        }

        public async Task<Role> GetRole(string email)
        {
            var user = await _userRepository.GetSingle(u => u.Email == email);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            return (Role)user.Role;
        }

        public string HashUserPassword(ApplicationUserModel model)
        {
            return CryptographyHelper.Hash(model.Password, HASHKEY + model.Email);
        }

        #endregion Public Methods

        #region Private Methods

        private async Task<string> GetJWTToken(ApplicationUserModel applicationUser, Role role)
        {
            var userClaim = GetClaims(applicationUser.Email, role);
            Claim cl = userClaim.Claims.First();
            var now = DateTime.UtcNow;

            var claims = new[]
               {
                            new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                            new Claim(JwtRegisteredClaimNames.Iat,ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                            cl
                          };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(_jwtOptions.ValidFor),
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

        private ClaimsIdentity GetClaims(string email, Role role)
        {
            Claim claim;

            switch (role)
            {
                case Role.User:
                    claim = new Claim("Admin", "Admin");
                    break;
                case Role.Admin:
                    claim = new Claim("User", "User");
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return new ClaimsIdentity(new GenericIdentity(email, "Token"), new[] { claim });
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);

        private void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }

        #endregion Private Methods
    }
}
