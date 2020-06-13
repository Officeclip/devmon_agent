using devmon_library.Core;
using devmon_library.Models;
using devmon_test.Extensions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace devmon_test.Core
{
    public class VolatileDeviceCollectorTests
    {
        [Fact]
        public async Task Read_HasAllProperties()
        {
            var collector = new VolatileDeviceCollector(
                new Mock<ICpuCollector>().Object,
                new Mock<IMemoryCollector>().Object,
                new Mock<INetworkCollector>().Object,
                new Mock<IDriveCollector>().Object,
                new Mock<IOsCollector>().Object);

            var info = await collector.Read();

            Assert.Equal(5, info.GetPublicPropertyCount());
        }

        [Fact]
        public async Task Read_HasCpuUtilization()
        {
            var collector = MockCollector<VolatileDeviceCollector>.Mock<ICpuCollector, CpuUtilization>(
                c => c.ReadCpuUtilization(),
                new CpuUtilization
                {
                    SpeedMhz = 1000,
                    LoadPercentage = 1
                });

            var info = await collector.Read();

            Assert.Equal(2, info.Cpu.GetPublicPropertyCount());

            Assert.Equal(1000, info.Cpu.SpeedMhz);
            Assert.Equal(1.0f, info.Cpu.LoadPercentage);
        }

        [Fact]
        public async Task Read_HasMemoryUtilization()
        {
            var collector = MockCollector<VolatileDeviceCollector>.Mock<IMemoryCollector, MemoryUtilization>(
                c => c.ReadMemoryUtilization(),
                new MemoryUtilization
                {
                    FreeBytes = 1
                });

            var info = await collector.Read();

            Assert.Equal(1, info.Mem.GetPublicPropertyCount());
 
            Assert.Equal(1UL, info.Mem.FreeBytes);
        }

        [Fact]
        public async Task Read_HasNetworkUtilization()
        {
            var collector = MockCollector<VolatileDeviceCollector>.Mock<INetworkCollector, NetworkUtilization[]>(
                c => c.ReadNetworkUtilization(),
                new NetworkUtilization[]
                {
                    new NetworkUtilization
                    {
                        Name = "wifi",
                        ReceivedBytesPerSecond = 1,
                        SentBytesPerSecond = 2
                    },
                    new NetworkUtilization
                    {
                        Name = "lan",
                        ReceivedBytesPerSecond = 1,
                        SentBytesPerSecond = 2
                    }
                });

            var info = await collector.Read();

            Assert.Equal(2, info.Net.Length);
            Assert.Equal(3, info.Net[0].GetPublicPropertyCount());

            Assert.Equal("wifi", info.Net[0].Name);
            Assert.Equal(1UL, info.Net[0].ReceivedBytesPerSecond);
            Assert.Equal(2UL, info.Net[0].SentBytesPerSecond);
            Assert.Equal("lan", info.Net[1].Name);
            Assert.Equal(1UL, info.Net[1].ReceivedBytesPerSecond);
            Assert.Equal(2UL, info.Net[1].SentBytesPerSecond);
        }

        [Fact]
        public async Task Read_HasDriveUtilization()
        {
            var collector = MockCollector<VolatileDeviceCollector>.Mock<IDriveCollector, DriveUtilization[]>(
                c => c.ReadDriveUtilization(), 
                new DriveUtilization[]
                {
                    new DriveUtilization
                    {
                        Name = "C:",
                        FreeBytes = 1
                    },
                    new DriveUtilization
                    {
                        Name = "D:"
                    }
                });

            var info = await collector.Read();

            Assert.Equal(2, info.Drives.Length);
            Assert.Equal(2, info.Drives[0].GetPublicPropertyCount());

            Assert.Equal("C:", info.Drives[0].Name);
            Assert.Equal(1UL, info.Drives[0].FreeBytes);
            Assert.Equal("D:", info.Drives[1].Name);
            Assert.Null(info.Drives[1].FreeBytes);
        }

        [Fact]
        public async Task Read_HasOsUtilization()
        {
            var collector = MockCollector<VolatileDeviceCollector>.Mock<IOsCollector, OsUtilization>(
                c => c.ReadOsUtilization(),
                new OsUtilization
                {
                    Processes = 1,
                    Update = new WindowsUpdateInfo
                    {
                        LastUpdateInstalledAt = new DateTime(2000, 1, 1),
                        PendingUpdates = 1
                    },
                    UpTime = TimeSpan.FromHours(1),
                });

            var info = await collector.Read();

            Assert.Equal(3, info.Os.GetPublicPropertyCount());
            Assert.Equal(2, info.Os.Update.GetPublicPropertyCount());

            Assert.Equal(1, info.Os.Processes);
            Assert.Equal(new DateTime(2000, 1, 1), info.Os.Update.LastUpdateInstalledAt);
            Assert.Equal(1, info.Os.Update.PendingUpdates);
            Assert.Equal(TimeSpan.FromHours(1), info.Os.UpTime);
        }
    }
}
