using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SnowBLL.Service.Interfaces;
using System.Net;
using SnowBLL.Models.Auth;
using SnowBLL.Models;

namespace SnowAPI.Controllers
{
    /// <summary>
    /// Authorization controller
    /// </summary>
    [Route("api/auth")]
    public class AuthController : BaseController
    {
        private IAuthBLService _authService;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="authService">Authorization service</param>
        public AuthController(IAuthBLService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Authorize user and returns authorization token
        /// </summary>
        /// <param name="applicationUser"Application user></param>
        /// <returns>Authorization token</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Authorize([FromBody] ApplicationUserModel applicationUser)
        {
            if (!ModelState.IsValid)
            {
                return WrapResponse(new Result<AuthorizeResponseModel>(WrapModelStateErrors(ModelState)));
            }

            var result = await _authService.Authorize(applicationUser);
            return WrapResponse(result, HttpStatusCode.OK);
        }
    }
}
