using devmon_library.Models;
using Microsoft.VisualBasic.Devices;
using NLog;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WUApiLib;

namespace devmon_library.Core
{
    internal sealed class OsCollector : IOsCollector
    {
        static DateTime lastAction = DateTime.Now;
        static UInt64 previousReadOpCount = 0;
        const int AllowedMinutesIdle = 3;

        [DllImport("kernel32.dll")]
        public static extern UInt64 GetTickCount64();

        static ILogger _logger;

        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            public uint cbSize;
            public int dwTime;
        }

        private ICancellation _cancellation;

        public OsCollector(ICancellation cancellation, ILogger logger = null)
        {
            _cancellation = cancellation;
            _logger = logger;
        }

        private DateTime BootTime
        {
            get
            {
                return DateTime.UtcNow.AddMilliseconds(-Environment.TickCount);
            }
        }

        private int MinutesIdle()
        {
            UInt64 totalReadOpCount = GetTotalReadOperationCount();
            var minutesIdle = 0;
            if (totalReadOpCount != previousReadOpCount)
            {
                previousReadOpCount = totalReadOpCount;
                lastAction = DateTime.Now;
            }
            else
            {
                minutesIdle = (int)DateTime.Now.Subtract(lastAction).TotalMinutes;
                if (minutesIdle <= AllowedMinutesIdle)
                {
                    minutesIdle = 0;
                }
            }
            return minutesIdle;
        }

        private UInt64 GetTotalReadOperationCount()
        {
            UInt64 readOps = 0;
            try
            {
                // see: https://www.autoitscript.com/forum/topic/160918-how-to-detect-user-idle-from-system-process-or-service/
                ManagementObjectSearcher searcher =
                   new ManagementObjectSearcher("root\\CIMV2",
                   "SELECT * FROM Win32_Process where Name = 'csrss.exe'");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    readOps += Convert.ToUInt64(queryObj["ReadOperationCount"]);
                }
                return readOps;
            }
            catch (Exception e)
            {
                Console.WriteLine("*** An error occurred while querying for WMI data: " + e.Message);
            }
            return 0;
        }

        public Task<OsInfo> ReadOsInfo()
        {
            var computerInfo = new ComputerInfo();

            var info = new OsInfo
            {
                MachineName = Environment.MachineName,
                Bitness = Environment.Is64BitOperatingSystem ? (byte)64 : (byte)32,
                Edition = computerInfo.OSFullName,
                Version = computerInfo.OSVersion,
                InstalledUICulture = CultureInfo.InstalledUICulture.IetfLanguageTag,
                EnvironmentVariables = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine)
            };

            return Task.FromResult(info);
        }



        public async Task<OsUtilization> ReadOsUtilization()
        {
            var tickCount64 = GetTickCount64();
            var upTime = TimeSpan.FromTicks((long)tickCount64);

            var os = new OsUtilization
            {
                Processes = Process.GetProcesses().Length,
                IdleTime = MinutesIdle(),
                UpTime = upTime
            };

            return os;
        }

        private Task<ISearchResult> SearchUpdatesAsync()
        {
            var tcs = new TaskCompletionSource<ISearchResult>();

            var updateSession = new UpdateSession();
            IUpdateSearcher updateSearcher = updateSession.CreateUpdateSearcher();
            updateSearcher.Online = false;

            var callback = new UpdateSearcherCallback(updateSearcher, tcs);

            try
            {
                updateSearcher.BeginSearch("IsInstalled = 0 And IsHidden = 0 And BrowseOnly = 0", callback, null);
            }
            catch (OperationCanceledException)
            {
                tcs.SetCanceled();
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            return tcs.Task;
        }
    }
}
