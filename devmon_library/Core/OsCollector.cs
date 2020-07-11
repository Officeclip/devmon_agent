using devmon_library.Models;
using Microsoft.VisualBasic.Devices;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WUApiLib;

namespace devmon_library.Core
{
    internal sealed class OsCollector : IOsCollector
    {
        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            public uint cbSize;
            public int dwTime;
        }

        private ICancellation _cancellation;

        public OsCollector(ICancellation cancellation)
        {
            _cancellation = cancellation;
        }

        private DateTime BootTime
        {
            get
            {
                return DateTime.UtcNow.AddMilliseconds(-Environment.TickCount);
            }
        }

        private DateTime LastInputTime
        {
            get
            {
                LASTINPUTINFO lii = new LASTINPUTINFO();
                lii.cbSize = (uint)Marshal.SizeOf(typeof(LASTINPUTINFO));
                GetLastInputInfo(ref lii);

                DateTime lastInputTime = BootTime.AddMilliseconds(lii.dwTime);
                return lastInputTime;
            }
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
            var ticks = (double)Stopwatch.GetTimestamp();
            var upTime = TimeSpan.FromSeconds(ticks / Stopwatch.Frequency);

            var os = new OsUtilization
            {
                Processes = Process.GetProcesses().Length,
                IdleTime = (int)DateTime.UtcNow.Subtract(LastInputTime).TotalMinutes,
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

        //private async Task<WindowsUpdateInfo> GetLatestUpdateInfo()
        //{
        //    var updateSession = new UpdateSession();
        //    var updateSearcher = updateSession.CreateUpdateSearcher();
        //    updateSearcher.Online = false;

        //    var searchResult = await SearchUpdatesAsync();

        //    var update = new WindowsUpdateInfo();
        //    update.PendingUpdates = searchResult.Updates.Count;
        //    var count = updateSearcher.GetTotalHistoryCount();

        //    if (count > 0)
        //    {
        //        var history = updateSearcher.QueryHistory(0, 1);
        //        update.LastUpdateInstalledAt = history[0].Date;
        //    }

        //    return update;
        //}
    }
}
