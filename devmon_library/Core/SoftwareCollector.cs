using devmon_library.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace devmon_library.Core
{
    internal sealed class SoftwareCollector : ISoftwareCollector
    {
        private readonly ICancellation _cancellation;

        public SoftwareCollector(ICancellation cancellation)
        {
            _cancellation = cancellation;
        }

        /// <summary>
        /// From: https://stackoverflow.com/a/59804150/89256
        /// </summary>
        /// <param name="registryKey"></param>
        /// <returns></returns>
        private void ReadRegistry(
                            ref List<SoftwareInfo> installedprograms,
                            string registryKey, 
                            int bitSize)
        {
            //string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        if (subkey.GetValue("DisplayName") != null)
                        {
                            var softwareInfo = new SoftwareInfo
                            {
                                DisplayName = (string)subkey.GetValue("DisplayName"),
                                Version = (string)subkey.GetValue("DisplayVersion"),
                                InstalledDate = (string)subkey.GetValue("InstallDate"),
                                Publisher = (string)subkey.GetValue("Publisher"),
                                //UnninstallCommand = (string)subkey.GetValue("UninstallString"),
                                //ModifyPath = (string)subkey.GetValue("ModifyPath"),
                                BitSize = bitSize
                            };
                            softwareInfo.EstimatedSize =
                                subkey.GetValue("EstimatedSize") == null
                                ? ""
                                : $"{subkey.GetValue("EstimatedSize")} kb";
                            installedprograms.Add(softwareInfo);
                        }
                    }
                }
            }
        }
    
        public Task<SoftwareInfo[]> ReadSoftware()
        {
            var installedprograms = new List<SoftwareInfo>();
            //ReadRegistry(
            //        ref installedprograms,
            //        @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
            //        32);
            ReadRegistry(
                   ref installedprograms,
                   @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall",
                    64);

            installedprograms = installedprograms.OrderBy(o => o.DisplayName).ToList();

            return Task.FromResult(installedprograms.ToArray());
        }

    }
}
