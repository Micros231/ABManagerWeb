using ABManagerWeb.ApplicationCore.Entities;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ABManagerWeb.ApplicationCore.Interfaces
{
    public interface IManifestManager
    {
        Task<ManifestInfo> GetCurrentManifestInfoAsync();
        Task<ManifestInfo> GetManifestInfoByIdAsync(string id);
        Task<ManifestInfo> GetManifestInfoByVersionAsync(string version);
        Task<ManifestInfo> GetManifestInfoByPathAsync(string path);
        FileStream GetManifestFileByInfo(ManifestInfo manifestInfo);
        Task<string> GetManifestVersionByStreamAsync(Func<Stream> getManifestHandler);
        Task<ManifestInfo> AddManifestAsync(string version, Func<Stream, Task> addManifestHandler);
        Task RemoveManifestToIdAsync(string id);
        Task RemoveManifestToVersionAsync(string version);
        Task RemoveManifestToPathAsync(string path);
        Task RemoveManifestAsync(ManifestInfo manifestInfo);
    }
}
