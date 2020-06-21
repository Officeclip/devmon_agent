using devmon_library.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
                            RegistryKey registryRoot,
                            ref List<SoftwareInfo> installedprograms,
                            string registryKey)
        {
            using (RegistryKey key = registryRoot.OpenSubKey(registryKey))
            {
                if (key != null)
                {
                    foreach (string subkey_name in key.GetSubKeyNames())
                    {
                        using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                        {
                            if (subkey.GetValue("DisplayName") != null)
                            {
                                var softwareInfo = new SoftwareInfo
                                {
                                    Name = GetValue(subkey.GetValue("DisplayName")),
                                    Version = GetValue(subkey.GetValue("DisplayVersion")),
                                    Installed = GetDate(subkey.GetValue("InstallDate")),
                                    Publisher = GetValue(subkey.GetValue("Publisher")),
                                    Size = GetSize(subkey.GetValue("EstimatedSize"))
                                };
                                installedprograms.Add(softwareInfo);
                            }
                        }
                    }
                }
            }
        }

        private string GetValue(object stringValue)
        {
            return (stringValue ?? string.Empty).ToString();
        }

        private string GetSize(object sizeValue)
        {
            if (sizeValue != null)
            {
                return (Convert.ToDouble(sizeValue) / 1024).ToString("N2") + " mb";
            }
            return string.Empty;
        }

        private string GetDate(object dateValue)
        {
            if (dateValue != null)
            {
                if (DateTime.TryParseExact(
                                        (string)dateValue,
                                        "yyyyMMdd",
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None,
                                        out DateTime result))
                {
                    return result.ToString("MMMM dd yyyy");
                }
            }
            return string.Empty;
        }

        public Task<SoftwareInfo[]> ReadSoftware()
        {
            var installedPrograms32Bit = ReadSoftware32Bit();
            var installedPrograms64Bit = ReadSoftware64Bit();
            var installedProgramsUser = ReadSoftwareUser();
            // From: https://stackoverflow.com/a/1606686/89256
            var installedPrograms = installedPrograms64Bit
                                                    .Concat(installedPrograms32Bit)
                                                    .Concat(installedProgramsUser)
                                                    .ToList();
            installedPrograms = installedPrograms
                                            .Distinct()
                                            .ToList();
            // From: https://stackoverflow.com/a/2779382/89256
            installedPrograms = installedPrograms
                                            .OrderBy(o => o.Name)
                                            .GroupBy(o => o.Name)
                                            .Select(y => y.First())
                                            .ToList();

            return Task.FromResult(installedPrograms.ToArray());
        }

        /// <summary>
        /// From: https://stackoverflow.com/a/18772256/89256
        /// </summary>
        /// <returns></returns>
        public List<SoftwareInfo> ReadSoftware64Bit()
        {
            var installedprograms = new List<SoftwareInfo>();

            var registryRoot = RegistryKey.OpenBaseKey(
                                            RegistryHive.LocalMachine,
                                            RegistryView.Registry64);

            ReadRegistry(
                   registryRoot,
                   ref installedprograms,
                   @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");

            return installedprograms;
        }

        public List<SoftwareInfo> ReadSoftware32Bit()
        {
            var installedprograms = new List<SoftwareInfo>();

            var registryRoot = RegistryKey.OpenBaseKey(
                                            RegistryHive.LocalMachine,
                                            RegistryView.Registry32);

            ReadRegistry(
                   registryRoot,
                   ref installedprograms,
                   @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");

            return installedprograms;
        }

        public List<SoftwareInfo> ReadSoftwareUser()
        {
            var installedprograms = new List<SoftwareInfo>();
            var registryRoot = RegistryKey.OpenBaseKey(
                                            RegistryHive.CurrentUser,
                                            RegistryView.Registry64);
            ReadRegistry(
                   registryRoot,
                   ref installedprograms,
                   @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");

            return installedprograms;

        }

    }
}
