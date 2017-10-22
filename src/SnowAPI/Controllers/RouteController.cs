using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SnowBLL.Service.Interfaces;
using SnowBLL.Models;
using SnowBLL.Models.Routes;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using SnowBLL.Validators.Routes;

namespace SnowAPI.Controllers
{
    /// <summary>
    /// Controller for routes
    /// </summary>
    [Produces("application/json")]
    [Route("api/routes")]
    [AllowAnonymous]
    public class RouteController : BaseController
    {
        private IRouteBLService _routeService;

        /// <summary>
        /// Default controller
        /// </summary>
        /// <param name="routeService">Route business logic service</param>
        public RouteController(IRouteBLService routeService)
        {
            this._routeService = routeService;
        }

        /// <summary>
        /// Gets routes on selected parameters
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pagesize">Amount of elements on page</param>
        /// <param name="filter">Filter query, e.g name::newline|difficulty::4|userid::7</param>
        /// <param name="sort">Sorting filed e.g name (Ascending), -name (desceding)</param>
        /// <returns>Collecion of routes</returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string filter, [FromQuery]  string sort, [FromQuery] int? page, [FromQuery] int? pagesize)
        {
            var result = await _routeService.GetAllRoutes(new CollectionRequestModel(page, pagesize, sort, filter));

            return WrapResponse(result, HttpStatusCode.OK);
        }

        /// <summary>
        /// Gets route by given identifier.
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _routeService.GetRouteById(new IdModel() { Id = id });

            return WrapResponse(result, HttpStatusCode.OK);
        }

        /// <summary>
        /// Creates new route
        /// </summary>
        /// <param name="model">Route model</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RouteCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return WrapResponse(new Result<RouteViewModel>(WrapModelStateErrors(ModelState)));
            }

            var result = await _routeService.Create(model);

            if (result.IsOk)
            {
                var idUrl = Url.Action("GetById", new { id = result.Content });
                Result<string> urlResult = new Result<string>(idUrl);
                return WrapResponse(urlResult, HttpStatusCode.Created);
            }

            return WrapResponse(result, HttpStatusCode.Created);
        }

        /// <summary>
        /// Updates the route
        /// </summary>
        /// <param name="id">Route identifier</param>
        /// <param name="model">Route update model</param>
        /// <returns>Status 200 if OK, 404 if not found, 402 if invalid model</returns>
        [HttpPut]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Update([FromQuery] int id, [FromBody] RouteUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                return WrapResponse(new Result<RouteUpdateModel>(WrapModelStateErrors(ModelState)));
            }

            var result = await _routeService.Update(id, model);

            return WrapResponse(result, HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Removes selected route
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Remove([FromQuery] int id)
        {
            var result = await _routeService.Remove(new IdModel() { Id = id });

            return WrapResponse(result, HttpStatusCode.OK);
        }

        /// <summary>
        /// Get geometries for selected routes
        /// </summary>
        /// <param name="routes">Ids separated by comma</param>
        /// <returns>Collection of geometries</returns>
        [HttpGet]
        [Route("geometries")]
        public async Task<IActionResult> GetGeometries([FromQuery] string routes)
        {
            var result = await _routeService.GetGeometries(routes);

            return WrapResponse(result);
        }

        /// <summary>
        /// Get routes in point neighbourhood
        /// </summary>
        /// <param name="lat">Latitude of point</param>
        /// <param name="lng">Longitude of point</param>
        /// <param name="kilometers">Kilometers range</param>
        /// <param name="pagesize">Page size</param>
        /// <param name="page">Page number</param>
        /// <returns>Collection of routes</returns>
        [HttpGet]
        [Route("inrange")]
        public async Task<IActionResult> GetInRange([FromQuery] decimal? lat, [FromQuery] decimal? lng, [FromQuery] int kilometers, [FromQuery] int? pagesize, [FromQuery] int? page)
        {
            var model = new RoutesInRangeRequestModel(kilometers, lat, lng, page, pagesize);

            var validationResult = new RoutesInRangeRequestModelValidator().Validate(model);

            if (!validationResult.IsValid)
            {
                return WrapResponse(new Result<ListResult<RouteInfo>>(WrapModelStateErrors(validationResult)));
            }

            var result = await _routeService.GetRoutesInRange(model);

            return WrapResponse(result, HttpStatusCode.OK);
        }

        /// <summary>
        /// Get main points of the routes
        /// </summary>
        /// <param name="pagesize">Page size</param>
        /// <param name="page">Page number</param>
        /// <returns>Collection of routes</returns>
        [HttpGet]
        [Route("points")]
        public async Task<IActionResult> GetPoints([FromQuery] int? pagesize, [FromQuery] int? page)
        {
            var model = new PagingRequestModel(page, pagesize);

            var result = await _routeService.GetPoints(model, string.Empty);

            return WrapResponse(result, HttpStatusCode.OK);
        }
    }
}