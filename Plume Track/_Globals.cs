using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Plume_Track
{
    public static class _Globals
    {
        public static readonly string basePath = AppContext.BaseDirectory;
        // Basic configuration settings
        public static readonly string PythonDllPath = Path.Combine(basePath, "python", "python313.dll");
        public static readonly string PythonModulePath = basePath;
        public static readonly string BackendModuleName = "backend.backend";
        public static readonly string CMapsPath = Path.Combine(basePath, "colormaps");
        public static readonly string dataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        // XML document for project configuration
        public static XmlDocument Config { get; private set; } = new XmlDocument();
        public static XmlDocument mapOptions { get; private set; } = new XmlDocument();
        public static bool isSaved { get; set; } = false;
        // Global pathes
        public static readonly string _ColorMapsPath = "colormaps";
    }
}
