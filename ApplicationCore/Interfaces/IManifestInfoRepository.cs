using ABManagerWeb.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ABManagerWeb.ApplicationCore.Interfaces
{
    public interface IManifestInfoRepository : IAsyncRepository<ManifestInfo>
    {
        Task<ManifestInfo> GetByVersionAsync(string version);
        Task<ManifestInfo> GetByPathAsync(string path);
        Task<ManifestInfo> GetCurrentManifestAsync();
    }
}
