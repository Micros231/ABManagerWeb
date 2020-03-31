using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABManagerWeb.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ABManagerWeb.Web.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class VersionsController : ControllerBase
    {
        private readonly ILogger<VersionsController> _logger;
        private readonly IManifestManager _manifestManager;
        public VersionsController(ILogger<VersionsController> logger, IManifestManager manifestManager)
        {
            _logger = logger;
            _manifestManager = manifestManager;
        }
        [HttpGet()]
        public async Task<ActionResult> GetVersions()
        {
            var manifestInfo = await _manifestManager.GetCurrentManifestInfoAsync();
            if (manifestInfo != null)
            {
                return Ok(manifestInfo.Version);
            }
            return BadRequest();
        }
    }
}
