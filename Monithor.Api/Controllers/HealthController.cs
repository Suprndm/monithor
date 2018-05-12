using Microsoft.AspNetCore.Mvc;

namespace Monithor.Api.Controllers
{
    [Route("api/health")]
    public class HealthController : Controller
    {
        [HttpGet("")]
        public ObjectResult Get()
        {
            return Ok(true);
        }
    }
}
