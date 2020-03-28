using ABManagerWeb.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ABManagerWeb.ApplicationCore.Interfaces
{
    public interface IManifestInfoRepository : IAsyncRepository<ManifestInfo>
    {
        Task<ManifestInfo> GetByVersion(string version);
        Task<ManifestInfo> GetByPath(string path);
        Task<ManifestInfo> GetCurrentManifest();
    }
}
