using ABManagerWeb.ApplicationCore.Entities;
using ABManagerWeb.ApplicationCore.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ABManagerWeb.Infrastructure.Data.ABManager
{
    public class ManifestInfoRepository : BaseEFRepository<ManifestInfo, ABManagerContext>, IManifestInfoRepository
    {
        public ManifestInfoRepository(ABManagerContext context) : base(context)
        {

        }

        public async Task<ManifestInfo> GetByPath(string path)
        {
            return await _dbContext.Set<ManifestInfo>().AsQueryable().FirstOrDefaultAsync(manifest => manifest.Path == path);
        }

        public async Task<ManifestInfo> GetByVersion(string version)
        {
            return await _dbContext.Set<ManifestInfo>().AsQueryable().FirstOrDefaultAsync(manifest => manifest.Version == version);
        }

        public async Task<ManifestInfo> GetCurrentManifest()
        {
            return await _dbContext.Set<ManifestInfo>().AsQueryable().FirstOrDefaultAsync();
        }
    }
}
