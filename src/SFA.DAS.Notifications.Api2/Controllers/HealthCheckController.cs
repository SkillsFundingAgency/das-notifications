using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Notifications.Api2.Controllers
{
    //todo think this can be done by config and doesn't need a hand rolled controller
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        /// <summary>
        /// Gets a status code to indicate the health of the application
        /// </summary>
        /// <returns>A status code to indicate the health of the application</returns>
        /// <response code="200">Health check successful</response>
        /// <response code="401">The client is not authorized to access this endpoint</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [HttpGet()]
        public ActionResult Get()
        {
            return StatusCode(200);
        }
    }
}
