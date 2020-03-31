using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ABManagerWeb.ApplicationCore.Consts;

namespace ABManagerWeb.ApplicationCore.Helpers.Paths
{
    public static class ABHostingPaths
    {
        public static string GetMainPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), NameConsts.AssetBundleHostingDirectoryName);
        }

        public static string GetVersionName(string version)
        {
            return $"_version({version})";
        }
        public static string GetManifestFileName()
        {
            return $"manifest.json";
        }
    }
}
