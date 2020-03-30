using ABManagerWeb.ApplicationCore.Entities;
using ABManagerWeb.ApplicationCore.Consts;
using ABManagerWeb.ApplicationCore.Helpers.Paths;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ABManagerWeb.Infrastructure.Data.ABManager
{
    public class ABManagerContext : DbContext
    {
        public DbSet<ManifestInfo> ManifestInfos { get; set; }
        public ABManagerContext(DbContextOptions<ABManagerContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ManifestInfo>(options =>
            {
                options.HasKey(manifest => new { manifest.Id, manifest.Path, manifest.Version });
                options.Property(manifest => manifest.Id).IsRequired(true).ValueGeneratedOnAdd();
                options.Property(manifest => manifest.Version).IsRequired(true);
                options.Property(manifest => manifest.Path).IsRequired(true);
            });
        }
    }
}
