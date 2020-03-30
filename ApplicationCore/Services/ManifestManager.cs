using ABManagerWeb.ApplicationCore.Consts;
using ABManagerWeb.ApplicationCore.Entities;
using ABManagerWeb.ApplicationCore.Helpers.Paths;
using ABManagerWeb.ApplicationCore.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ABManagerWeb.ApplicationCore.Services
{
    public class ManifestManager : IManifestManager
    {
        private readonly IManifestInfoRepository _repository;
        private readonly ILogger<ManifestManager> _logger;
        public ManifestManager(IManifestInfoRepository repository, ILogger<ManifestManager> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        public async Task AddManifestAsync(string version, Func<Stream, Task> addManifestHandler)
        {
            _logger.LogInformation("AddManifestStart");
            version = await GetFinalVersion(version);
            string pathToFile = CreateDirectoryAndGetPath(version);
            string completePathToFile = Path.Combine(ABHostingPaths.GetMainPath(), pathToFile);
            using (var fileStream = File.Create(completePathToFile))
            {
                await addManifestHandler?.Invoke(fileStream);
            }
            ManifestInfo manifest = new ManifestInfo(version, pathToFile);
            if (CheckExitsManifesInfoFile(manifest))
            {
                await _repository.AddAsync(manifest);
            }

        }
        public async Task<ManifestInfo> GetCurrentManifestAsync()
        {
            _logger.LogInformation("GetCurrentManifestAsync");
            var manifestInfo = await _repository.GetCurrentManifest();
            if (CheckExitsManifesInfoFile(manifestInfo))
            {
                return manifestInfo;
            }
            
            return null;
        }
        public async Task<ManifestInfo> GetManifestByIdAsync(string id)
        {
            _logger.LogInformation("GetManifestByIdAsync");
            if (!string.IsNullOrEmpty(id))
            {
                var manifestInfo = await _repository.GetByIdAsync(id);
                if (CheckExitsManifesInfoFile(manifestInfo))
                {
                    return manifestInfo;
                }
            }
            return null;
        }
        public async Task<ManifestInfo> GetManifestByPathAsync(string path)
        {
            _logger.LogInformation("GetManifestToPathAsync");
            if (!string.IsNullOrEmpty(path))
            {
                if (path.Contains(NameConsts.AssetBundleHosting))
                {
                    string finalPath = path.Remove(0, path.IndexOf(NameConsts.AssetBundleHosting));
                    var manifestInfo = await _repository.GetByPath(finalPath);
                    if (CheckExitsManifesInfoFile(manifestInfo))
                    {
                        return manifestInfo;
                    }
                }
            }
            return null;
        }
        public async Task<ManifestInfo> GetManifestByVersionAsync(string version)
        {
            _logger.LogInformation("GetManifestToVersionAsync");
            if (!string.IsNullOrEmpty(version))
            {
                var manifestInfo = await _repository.GetByVersion(version);
                if (CheckExitsManifesInfoFile(manifestInfo))
                {
                    return manifestInfo;
                }
            }
            return null;
        }
        public async Task<string> GetManifestVersionByStreamAsync(Func<Stream> getManifestHandler)
        {
            _logger.LogInformation($"Start GetManifestVersionToStream");

            string jsonManifest = string.Empty;
            string versionManifest = string.Empty;
            using (var readStream = getManifestHandler?.Invoke())
            using (var reader = new StreamReader(readStream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                _logger.LogInformation($"Get jsonManifest");
                var jManifest = await JObject.LoadAsync(jsonReader);
                _logger.LogInformation($"Get Version");
                versionManifest = jManifest["_version"].Value<string>();
            }
            _logger.LogInformation($"Version: {versionManifest}");
            return versionManifest;
        }

        public async Task RemoveManifestToIdAsync(string id)
        {
            _logger.LogInformation("RemoveManifestToIdAsync");
            var manifestInfo = await GetManifestByIdAsync(id);
            if (manifestInfo != null)
            {
                DeleteManifestFile(manifestInfo);                
                await _repository.DeleteAsync(manifestInfo);
            }
            else
            {
                manifestInfo = await _repository.GetByIdAsync(id);
                if (manifestInfo != null)
                {
                    await _repository.DeleteAsync(manifestInfo);
                }
            }

        }

        public async Task RemoveManifestToPathAsync(string path)
        {
            _logger.LogInformation("RemoveManifestToPathAsync");
            var manifestInfo = await GetManifestByPathAsync(path);
            if (manifestInfo != null)
            {
                DeleteManifestFile(manifestInfo);
                await _repository.DeleteAsync(manifestInfo);
            }
            else
            {
                manifestInfo = await _repository.GetByPath(path);
                if (manifestInfo != null)
                {
                    await _repository.DeleteAsync(manifestInfo);
                }
            }
        }
        public async Task RemoveManifestToVersionAsync(string version)
        {
            _logger.LogInformation("RemoveManifestToPathAsync");
            var manifestInfo = await GetManifestByVersionAsync(version);
            if (manifestInfo != null)
            {
                DeleteManifestFile(manifestInfo);
                await _repository.DeleteAsync(manifestInfo);
            }
            else
            {
                manifestInfo = await _repository.GetByVersion(version);
                if (manifestInfo != null)
                {
                    await _repository.DeleteAsync(manifestInfo);
                }
            }
        }
        public async Task RemoveManifestAsync(ManifestInfo manifestInfo)
        {
            _logger.LogInformation("RemoveManifestAsync");
            if (manifestInfo != null)
            {
                if (CheckExitsManifesInfoFile(manifestInfo))
                {
                    DeleteManifestFile(manifestInfo);
                }
                await _repository.DeleteAsync(manifestInfo);
            }
        }

        private async Task<string> GetFinalVersion(string version)
        {
            _logger.LogDebug("CheckVersionIsNull");
            if (string.IsNullOrEmpty(version))
            {
                _logger.LogWarning("Version is null. Check current manifest and update version");
                var currentManifest = await GetCurrentManifestAsync();
                var currentVersion = currentManifest.Version;
                version = currentVersion + "_nv";
            }
            _logger.LogDebug($"CheckExistManifestToVersion: {version}");
            int exitsLevel = 0;
            while (await _repository.GetByVersion(version) != null)
            {
                _logger.LogWarning($"Manifest to version: {version} exists");
                _logger.LogWarning($"UpdateVersion");
                exitsLevel++;
                version += "e" + exitsLevel;
            }
            _logger.LogInformation($"Final Version: {version}");
            return version;
        }
        private string CreateDirectoryAndGetPath(string version)
        {
            
            string completePathToVersion = 
                Path.Combine(
                    ABHostingPaths.GetMainPath(), 
                    ABHostingPaths.GetVersionName(version));
            _logger.LogInformation($"Complete path to version: {completePathToVersion}");
            if (!Directory.Exists(completePathToVersion))
            {
                _logger.LogDebug($"Create Directory: {completePathToVersion}");
                Directory.CreateDirectory(completePathToVersion);
            }
            string pathToFileManifest =
                Path.Combine(
                    ABHostingPaths.GetVersionName(version),
                    ABHostingPaths.GetManifestFileName());
            return pathToFileManifest;
        }
        private bool CheckExitsManifesInfoFile(ManifestInfo manifestInfo)
        {
            if (manifestInfo != null)
            {
                if (File.Exists(Path.Combine(ABHostingPaths.GetMainPath(), manifestInfo.Path)))
                    return true;
            }
            return false;
        }
        private void DeleteManifestFile(ManifestInfo manifestInfo)
        {
            File.Delete(Path.Combine(ABHostingPaths.GetMainPath(), manifestInfo.Path));
        }
  
    }
}
