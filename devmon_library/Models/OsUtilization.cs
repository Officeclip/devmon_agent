using System;
namespace devmon_library.Models
{
    public sealed class OsUtilization
    {
        /// <summary>
        /// IdleTime in minutes
        /// </summary>
        //public int IdleTime { get; set; }
        public int Processes { get; set; }
        public TimeSpan UpTime { get; set; }
        public bool IsBusy { get; set; }
    }
}
