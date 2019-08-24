using Geheb.DevMon.Agent.Core;
using Newtonsoft.Json;
using System.IO;
using Xunit;

namespace Geheb.DevMon.Agent.Test.Core
{
    public class AppSettingsTests
    {
        [Fact]
        public void Open_WithNoneExistentFile_ThrowsException()
        {
            Assert.Throws<FileNotFoundException>(() => new AppSettings("foo.json"));
        }

        [Fact]
        public void Open_WithInvalidFile_ThrowsException()
        {
            File.WriteAllText("foo.json", "bar");
            try
            {
                Assert.Throws<JsonReaderException>(() => new AppSettings("foo.json"));
            }
            finally
            {
                File.Delete("foo.json");
            }
        }

        [Fact]
        public void Open_WithAllProperties_IsSuccessful()
        {
            var settings = new AppSettings("appSettings.example.json");
            foreach (var key in new[] 
                {
                "auth_token_url",
                "auth_client_id",
                "auth_client_secret",
                "auth_audience",
                "server_url"
                })
            {
                Assert.False(string.IsNullOrEmpty(settings[key] as string));
            }
        }
    }
}
