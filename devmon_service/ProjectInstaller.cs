using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace devmon_service
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            new ServiceController(serviceInstaller1.ServiceName).Start();
        }

        private void AddSettings(string serverUrl, string serverGuid)
        {
            var jsonFilePath = Path.Combine(GetInstallationPath, "appSettings.json");

            var isFirstTime = false;
            try
            {
                JObject settings = JObject.Parse(
                                            File.ReadAllText(jsonFilePath));
                if ((string)settings["agent_guid"] == "")
                {
                    settings["agent_guid"] = Guid.NewGuid().ToString();
                    isFirstTime = true;
                }
                if ((string)settings["server_url"] == "")
                {
                    settings["server_url"] = serverUrl;
                    isFirstTime = true;
                }
                if ((string)settings["server_guid"] == "")
                {
                    settings["server_guid"] = serverGuid;
                    isFirstTime = true;
                }
                if (isFirstTime)
                {
                    File.WriteAllText(
                        jsonFilePath,
                        settings.ToString());
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine($"Error: {e.Message}");
            }
        }

        private string GetInstallationPath
        {
            get
            {
                var exeFullpath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                return Path.GetDirectoryName(exeFullpath);
            }
        }

        private void WriteLog(string str)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(Path.Combine(GetInstallationPath, "logFile.txt"), true);
                sw.WriteLine(DateTime.Now.ToString() + ":" + str);
                sw.Flush();
                sw.Close();
            }
            catch
            {

            }
        }

        private void serviceInstaller1_BeforeInstall(object sender, InstallEventArgs e)
        {
            string serverUrl = this.Context.Parameters["ServerUrl"];
            string serverGuid = this.Context.Parameters["ServerGuid"];
            AddSettings(serverUrl, serverGuid);
        }
    }
}
