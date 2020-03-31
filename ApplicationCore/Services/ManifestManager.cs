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

        public async Task<ManifestInfo> AddManifestAsync(string version, Func<Stream, Task> addManifestHandler)
        {
            _logger.LogInformation("AddManifestStart");
            version = await GetFinalVersion(version);
            await CreateManifestFile(version, addManifestHandler);
            string relativePathToManifest = GetRelativePathByVersion(version);
            ManifestInfo manifest = new ManifestInfo(version, relativePathToManifest);
            return await _repository.AddAsync(manifest);
        }
        public async Task<ManifestInfo> GetCurrentManifestInfoAsync()
        {
            _logger.LogInformation("GetCurrentManifestAsync");
            return await _repository.GetCurrentManifestAsync();
        }
        public async Task<ManifestInfo> GetManifestInfoByIdAsync(string id)
        {
            _logger.LogInformation("GetManifestByIdAsync");
            if (!string.IsNullOrEmpty(id))
            {
                return await _repository.GetByIdAsync(id);
            }
            return null;
        }
        public async Task<ManifestInfo> GetManifestInfoByPathAsync(string path)
        {
            _logger.LogInformation("GetManifestToPathAsync");
            if (!string.IsNullOrEmpty(path))
            {
                return await _repository.GetByPathAsync(path);
            }
            return null;
        }
        public async Task<ManifestInfo> GetManifestInfoByVersionAsync(string version)
        {
            _logger.LogInformation("GetManifestToVersionAsync");
            if (!string.IsNullOrEmpty(version))
            {
                return await _repository.GetByVersionAsync(version);
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
        public FileStream GetManifestFileByInfo(ManifestInfo manifestInfo)
        {
            if (manifestInfo != null)
            {
                if (CheckExitsManifestInfoFile(manifestInfo))
                {
                    string filePath = Path.Combine(ABHostingPaths.GetMainPath(), manifestInfo.Path);
                    var fileStream = new FileStream(filePath, FileMode.Open);
                    return fileStream;
                }
            }
            return null;
        }
        public async Task RemoveManifestToIdAsync(string id)
        {
            _logger.LogInformation("RemoveManifestToIdAsync");
            if (!string.IsNullOrEmpty(id))
            {
                var manifestInfo = await GetManifestInfoByIdAsync(id);
                if (manifestInfo != null)
                {
                    if (CheckExitsManifestInfoFile(manifestInfo))
                    {
                        DeleteManifestFile(manifestInfo);
                    }
                    await _repository.DeleteAsync(manifestInfo);
                }
            }
        }
        public async Task RemoveManifestToPathAsync(string path)
        {
            _logger.LogInformation("RemoveManifestToPathAsync");
            if (!string.IsNullOrEmpty(path))
            {
                var manifestInfo = await GetManifestInfoByPathAsync(path);
                if (manifestInfo != null)
                {
                    if (CheckExitsManifestInfoFile(manifestInfo))
                    {
                        DeleteManifestFile(manifestInfo);
                    }
                    await _repository.DeleteAsync(manifestInfo);
                }
            }
        }
        public async Task RemoveManifestToVersionAsync(string version)
        {
            _logger.LogInformation("RemoveManifestToVersionAsync");
            if (!string.IsNullOrEmpty(version))
            {
                var manifestInfo = await GetManifestInfoByVersionAsync(version);
                if (manifestInfo != null)
                {
                    if (CheckExitsManifestInfoFile(manifestInfo))
                    {
                        DeleteManifestFile(manifestInfo);
                    }
                    await _repository.DeleteAsync(manifestInfo);
                }
            }
        }
        public async Task RemoveManifestAsync(ManifestInfo manifestInfo)
        {
            _logger.LogInformation("RemoveManifestAsync");
            if (manifestInfo != null)
            {
                if (CheckExitsManifestInfoFile(manifestInfo))
                {
                    DeleteManifestFile(manifestInfo);
                }
                await _repository.DeleteAsync(manifestInfo);
            }
        }
        public bool CheckExitsManifestInfoFile(ManifestInfo manifestInfo)
        {
            if (manifestInfo != null)
            {
                if (File.Exists(Path.Combine(ABHostingPaths.GetMainPath(), manifestInfo.Path)))
                    return true;
            }
            return false;
        }
        public string GetRelativePathByVersion(string version)
        {
            string pathToFileManifest =
                Path.Combine(
                    ABHostingPaths.GetVersionName(version),
                    ABHostingPaths.GetManifestFileName());
            return pathToFileManifest;
        }

        private async Task<string> GetFinalVersion(string version)
        {
            _logger.LogDebug("CheckVersionIsNull");
            if (string.IsNullOrEmpty(version))
            {
                _logger.LogWarning("Version is null. Check current manifest and update version");
                var currentManifest = await GetCurrentManifestInfoAsync();
                var currentVersion = currentManifest.Version;
                version = currentVersion + "_nv";
            }
            _logger.LogDebug($"CheckExistManifestToVersion: {version}");
            int exitsLevel = 0;
            while (await _repository.GetByVersionAsync(version) != null)
            {
                _logger.LogWarning($"Manifest to version: {version} exists");
                _logger.LogWarning($"UpdateVersion");
                exitsLevel++;
                version += "e" + exitsLevel;
            }
            _logger.LogInformation($"Final Version: {version}");
            return version;
        }
        private async Task CreateManifestFile(string version, Func<Stream, Task> addManifestHandler)
        {
            CreateDirectoryByVersion(version);
            string pathToFile = GetRelativePathByVersion(version);
            string completePathToFile = Path.Combine(ABHostingPaths.GetMainPath(), pathToFile);
            using (var fileStream = File.Create(completePathToFile))
            {
                await addManifestHandler?.Invoke(fileStream);
            }
        }
        private void DeleteManifestFile(ManifestInfo manifestInfo)
        {
            File.Delete(Path.Combine(ABHostingPaths.GetMainPath(), manifestInfo.Path));
        }
        private void CreateDirectoryByVersion(string version)
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
        }
    }
}
