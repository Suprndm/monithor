using Microsoft.AspNetCore.Mvc;
using Monithor.Api.Logging;

namespace Monithor.Api.Controllers
{
    [Route("api/log")]
    public class LogController : Controller
    {
        private readonly ILogCollector _logCollector;

        public LogController(ILogCollector logCollector)
        {
            _logCollector = logCollector;
        }

        [HttpGet("")]
        public ObjectResult Get()
        {
            return Ok(_logCollector.GetAllLogs());
        }
    }
}
