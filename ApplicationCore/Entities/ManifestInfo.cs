using System;
using System.Collections.Generic;
using System.Text;

namespace ABManagerWeb.ApplicationCore.Entities
{
    public class ManifestInfo : BaseEntity
    {
        public string Version { get; private set; }
        public string Path { get; private set; }

        public ManifestInfo(string version, string path)
        {
            Version = version;
            Path = path;
        }
    }
}
