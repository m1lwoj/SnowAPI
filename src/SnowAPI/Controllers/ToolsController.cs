using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SnowBLL.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SnowAPI.Controllers
{
    [Produces("application/json")]
    [AllowAnonymous]
    public class ToolsController : BaseController
    {
        #region Members

        private IUserResolver _userResolver;

        #endregion Members

        #region Constructor

        public ToolsController (IUserResolver userResolver)
        {
            this._userResolver = userResolver;
        }

        #endregion Constructor

        [Route("api/ping")]
        [HttpGet]
        public IActionResult Ping()
        {
            return Ok();
        }

        [Route("api/version")]
        [HttpGet]
        public IActionResult Version()
        {
            string version = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationVersion;
            return Ok(new { Version = version });
        }

        [Route("api/test/authorization")]
        [HttpGet]
        [Authorize(Roles = "User")]
        public IActionResult Authorization()
        {
            string email = _userResolver.GetEmail();

            if (email == null)
            {
                return Content(HttpStatusCode.Forbidden, "Not authorized");
            }

            return new OkObjectResult($"Hello {email}");
        }
    }
}