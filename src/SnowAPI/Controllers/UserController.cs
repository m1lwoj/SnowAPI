using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnowBLL.Models;
using SnowBLL.Models.Auth;
using SnowBLL.Models.Users;
using SnowBLL.Service.Interfaces;
using System.Net;
using System.Threading.Tasks;

namespace SnowAPI.Controllers
{
    /// <summary>
    /// Controller for users action
    /// </summary>
    [Produces("application/json")]
    [Route("api/users")]
    [AllowAnonymous]
    public class UserController : BaseController
    {
        private IUserBLService _userService;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="routeService">User business logic service</param>
        public UserController(IUserBLService userService)
        {
            this._userService = userService;
        }

        /// <summary>
        /// Registers new user in service
        /// </summary>
        /// <param name="model">User model</param>
        /// <returns>Authorization token</returns>
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return WrapResponse(new Result<object>(WrapModelStateErrors(ModelState)));
            }

            var result = await _userService.Create(model);

            if (result.IsOk)
            {
                return WrapResponse(result, HttpStatusCode.OK);
            }

            return WrapResponse(result, HttpStatusCode.OK);
        }

        /// <summary>
        /// Get users 
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pagesize">Amount of elements on page</param>
        /// <param name="filter">Filter query, e.g name::marian</param>
        /// <param name="sort">Sorting filed e.g name (Ascending), -name (desceding)</param>
        /// <returns>Collecion of users</returns>
        [HttpGet]
        [Authorize("User")]
        public async Task<IActionResult> Get([FromQuery] string filter, [FromQuery]  string sort, [FromQuery] int? page, [FromQuery] int? pagesize)
        {
            var result = await _userService.GetAllUsers(new CollectionRequestModel(page, pagesize, sort, filter));

            return WrapResponse(result, HttpStatusCode.OK);
        }


        /// <summary>
        /// Get user by identifier.
        /// </summary>
        /// <returns>Return user</returns>
        [HttpGet]
        [Authorize("User")]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _userService.GetById(id);

            return WrapResponse(result, HttpStatusCode.OK);
        }

        /// <summary>
        /// Updates the user
        /// </summary>
        /// <param name="id">User identifier</param>
        /// <param name="model">User update model</param>
        /// <returns>Status 200 if OK, 404 if not found, 402 if invalid model</returns>
        [HttpPut]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Update([FromQuery] int id, [FromBody] UserUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                return WrapResponse(new Result<UserUpdateModel>(WrapModelStateErrors(ModelState)));
            }

            var result = await _userService.Update(id, model);

            return WrapResponse(result, HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Removes selected user
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Remove([FromQuery] int id)
        {
            var result = await _userService.Remove(new IdModel() { Id = id });

            return WrapResponse(result, HttpStatusCode.OK);
        }


        /// <summary>
        /// Send user email containing special code for reseting password
        /// </summary>
        /// <param name="email">User email></param>
        /// <returns>Information about sent email.</returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("/api/account/resetpassword")]
        public async Task<IActionResult> ResetPassword([FromQuery] string email)
        {
            var result = await _userService.SendResetPasswordEmail(email);
            return WrapResponse(result, HttpStatusCode.OK);
        }

        /// <summary>
        /// Set users new password
        /// </summary>
        /// <returns>Information about succes or not.</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("/api/account/resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody]NewPasswordModel model)
        {
            var result = await _userService.ResetPassword(model);
            return WrapResponse(result, HttpStatusCode.OK);
        }

        /// <summary>
        /// Send user email containing special code for confirming account
        /// </summary>
        /// <returns>Information about account status.</returns>
        [HttpGet]
        [Route("/api/account/confirm")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ConfirmAccount()
        {
            var result = await _userService.SendConfirmAccountEmail();
            return WrapResponse(result, HttpStatusCode.OK);
        }

        /// <summary>
        /// Confirms user's account
        /// </summary>
        /// <returns>Information about account status.</returns>
        [HttpPost]
        [Route("/api/account/confirm")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ConfirmAccount([FromBody]UserAccountConfirmationModel model)
        {
            var result = await _userService.ConfirmAccount(model);
            return WrapResponse(result, HttpStatusCode.OK);
        }

        /// <summary>
        /// Get logged user details.
        /// </summary>
        /// <returns>Information about user account.</returns>
        [HttpGet]   
        [Route("/api/account")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetAccountDetails()
        {
            var result = await _userService.GetLoggedUserDetails();
            return WrapResponse(result, HttpStatusCode.OK);
        }
    }
}
