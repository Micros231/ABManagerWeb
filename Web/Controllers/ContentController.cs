using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ABManagerWeb.ApplicationCore.Helpers.Paths;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace ABManagerWeb.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ContentController : ControllerBase
    {

        private readonly IWebHostEnvironment _appEnvironment;
        private readonly ILogger<ContentController> _logger;
        public ContentController(IWebHostEnvironment appEnvironment, ILogger<ContentController> logger)
        {
            _appEnvironment = appEnvironment;
            _logger = logger;
        }

        public string AssetBundleHosting { get; private set; }

        [HttpGet("{version}/manifest/{id?}")]
        public IActionResult GetManifest(string version, int id)
        {
            var jObjecct = new { Version = version, ID = id };
            _logger.LogInformation(ABHostingPaths.GetMainPath());
            return new JsonResult(jObjecct);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UploadManifest(IFormFile manifestFile)
        {
            _logger.LogInformation("UploadManifest Start");
            if (manifestFile != null)
            {
                _logger.LogInformation($"ManifestFile is not null {manifestFile.ToString()}");
                string jsonManifest = string.Empty;
                string versionManifest = string.Empty;
                using (var readStream = manifestFile.OpenReadStream())
                using (var reader = new StreamReader(readStream))
                {
                    jsonManifest = reader.ReadToEnd();
                    _logger.LogInformation($"Get jsonManifest");
                }
                if (!string.IsNullOrEmpty(jsonManifest))
                {
                    versionManifest = JObject.Parse(jsonManifest)["_version"].Value<string>();
                    _logger.LogInformation($"Get Version. Version: {versionManifest}");
                }
                if (!string.IsNullOrEmpty(versionManifest))
                {

                    string pathToVersion = Path.Combine(AssetBundleHosting, $"_version({versionManifest})");
                    string completePath = Path.Combine(_appEnvironment.ContentRootPath, pathToVersion);
                    _logger.LogInformation($"Path to hosting: {completePath}");
                    if (!Directory.Exists(completePath))
                    {
                        Directory.CreateDirectory(completePath);
                        using (var fileStream = new FileStream(Path.Combine(completePath,$"manifest_{versionManifest}.json"), FileMode.Create))
                        {
                            await manifestFile.CopyToAsync(fileStream);
                        }
                        return Ok();
                    }
                }
            }
            return BadRequest();
        }
    }
}
