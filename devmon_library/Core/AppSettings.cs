using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;

namespace devmon_library.Core
{
    sealed class AppSettings : IAppSettings
    {
        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        readonly Dictionary<string, object> _settings = new Dictionary<string, object>();

        public object this[string key]
        {
            get
            {
                return _settings.TryGetValue(key, out var value) ? value : null;
            }
        }
       
        public AppSettings(string fileName)
        {
            var fi = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName));
            if (!fi.Exists)
            {
                throw new FileNotFoundException($"app settings file not found {fileName}");
            }

            var json = File.ReadAllText(fi.FullName);

            _settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        }
    }
}
