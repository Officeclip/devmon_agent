﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devmon_library
{
    public static class Util
    {
        public static string CurrentVersion = "0.5.5";
        public static string GetAgentGuid()
        {

            return Guid.NewGuid().ToString();

        }

        private static string GetProductId()
        {
            string productId = null;
            try
            {
                using (RegistryKey key = GetRegistryRoot(64).OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion"))
                {
                    if (key != null)
                    {
                        Object o = key.GetValue("ProductId");
                        if (o != null)
                        {
                            productId = o.ToString();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Exception:{e.Message}");
            }

            return productId;
        }

        private static RegistryKey GetRegistryRoot(int byteSize)
        {
            return RegistryKey.OpenBaseKey(
                                    RegistryHive.LocalMachine,
                                    (byteSize == 64)
                                    ? RegistryView.Registry64
                                    : RegistryView.Registry32);
        }
    }
}
