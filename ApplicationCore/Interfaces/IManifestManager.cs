using ABManagerWeb.ApplicationCore.Entities;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ABManagerWeb.ApplicationCore.Interfaces
{
    public interface IManifestManager
    {
        Task<ManifestInfo> GetCurrentManifestAsync();
        Task<ManifestInfo> GetManifestByIdAsync(string id);
        Task<ManifestInfo> GetManifestByVersionAsync(string version);
        Task<ManifestInfo> GetManifestByPathAsync(string path);
        Task<string> GetManifestVersionByStreamAsync(Func<Stream> getManifestHandler);
        Task AddManifestAsync(string version, Func<Stream, Task> addManifestHandler);
        Task RemoveManifestToIdAsync(string id);
        Task RemoveManifestToVersionAsync(string version);
        Task RemoveManifestToPathAsync(string path);
        Task RemoveManifestAsync(ManifestInfo manifestInfo);
    }
}
