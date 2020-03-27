using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ABManagerWeb.Web.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class VersionsController : ControllerBase
    {
        private readonly string _curentVersion = "0";
        private ILogger<VersionsController> _logger;
        public VersionsController(ILogger<VersionsController> logger)
        {
            _logger = logger;
        }
        [Route("")]
        public ActionResult GetVersions()
        {
            var jsonObj = new
            {
                Version = _curentVersion,
                Manifest = new
                {
                    CRC = 256,
                    Hash = 256.GetHashCode()
                }
            };
            _logger.LogInformation($"Versions: {jsonObj.Version}");
            return new JsonResult(jsonObj);
        }
    }
}
