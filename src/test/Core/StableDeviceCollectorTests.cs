using devmon_library.Core;
using devmon_test.Extensions;
using Moq;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Xunit;
using devmon_library.Models;

namespace devmon_test.Core
{
    public class StableDeviceCollectorTests
    {
        [Fact]
        public async Task Read_HasAllProperties()
        {
            var collector = new StableDeviceCollector(
                new Mock<ICpuCollector>().Object,
                new Mock<IMemoryCollector>().Object,
                new Mock<INetworkCollector>().Object,
                new Mock<IDriveCollector>().Object,
                new Mock<IOsCollector>().Object,
                new Mock<ISoftwareCollector>().Object);

            var info = await collector.Read();
            Assert.Equal(5, info.GetPublicPropertyCount());
        }

        [Fact]
        public async Task Read_HasCpuInfo()
        {
            var collector = MockCollector<StableDeviceCollector>.Mock<ICpuCollector, CpuInfo>(
                c => c.ReadCpuInfo(),
                new CpuInfo
                {
                    Cores = 1,
                    Name = "foo",
                    SpeedMhz = 1000,
                    Threads = 2
                });

            var info = await collector.Read();

            Assert.Equal(4, info.Cpu.GetPublicPropertyCount());

            Assert.Equal(1, info.Cpu.Cores);
            Assert.Equal("foo", info.Cpu.Name);
            Assert.Equal(1000, info.Cpu.SpeedMhz);
            Assert.Equal(2, info.Cpu.Threads);
        }

        [Fact]
        public async Task Read_HasMemoryInfo()
        {
            var collector = MockCollector<StableDeviceCollector>.Mock<IMemoryCollector, MemoryInfo>(
                c => c.ReadMemoryInfo(),
                new MemoryInfo
                {
                    TotalBytes = 1
                });

            var info = await collector.Read();

            Assert.Equal(1, info.Mem.GetPublicPropertyCount());

            Assert.Equal(1UL, info.Mem.TotalBytes);
        }

        [Fact]
        public async Task Read_HasNetworkInfo()
        {
            var collector = MockCollector<StableDeviceCollector>.Mock<INetworkCollector, NetworkInfo[]>(
                c => c.ReadNetworkInfo(),
                new NetworkInfo[]
                {
                    new NetworkInfo
                    {
                        DhcpServerAddresses = new[] { "foo" },
                        DnsAddresses = new[] { "bar" },
                        MacAddress = "aa:bb:cc:dd:ee:ff",
                        Model = "intel",
                        Name = "wifi",
                        Type = NetworkInterfaceType.Wireless80211.ToString(),
                        UnicastAddresses = new[] { "1.1.1.1" }
                    },
                    new NetworkInfo
                    {
                        DhcpServerAddresses = new[] { "foo", "bar" },
                        DnsAddresses = new[] { "baz" },
                        MacAddress = "00:11:22:33:44:55",
                        Model = "intel",
                        Name = "lan",
                        Type = NetworkInterfaceType.Ethernet.ToString(),
                        UnicastAddresses = new[] { "1.1.1.1", "2.2.2.2" }
                    }
                });

            var info = await collector.Read();

            Assert.Equal(2, info.Net.Length);

            Assert.Equal(7, info.Net[0].GetPublicPropertyCount());

            Assert.Equal(new[] { "foo" }, info.Net[0].DhcpServerAddresses);
            Assert.Equal(new[] { "bar" }, info.Net[0].DnsAddresses);
            Assert.Equal("aa:bb:cc:dd:ee:ff", info.Net[0].MacAddress);
            Assert.Equal("intel", info.Net[0].Model);
            Assert.Equal("wifi", info.Net[0].Name);
            Assert.Equal("Wireless80211", info.Net[0].Type);
            Assert.Equal(new[] { "1.1.1.1" }, info.Net[0].UnicastAddresses);

            Assert.Equal(new[] { "foo", "bar" }, info.Net[1].DhcpServerAddresses);
            Assert.Equal(new[] { "baz" }, info.Net[1].DnsAddresses);
            Assert.Equal("00:11:22:33:44:55", info.Net[1].MacAddress);
            Assert.Equal("intel", info.Net[1].Model);
            Assert.Equal("lan", info.Net[1].Name);
            Assert.Equal("Ethernet", info.Net[1].Type);
            Assert.Equal(new[] { "1.1.1.1", "2.2.2.2" }, info.Net[1].UnicastAddresses);
        }

        [Fact]
        public async Task Read_HasDriveInfo()
        {
            var collector = MockCollector<StableDeviceCollector>.Mock<IDriveCollector, DriveInfo[]>(
                p => p.ReadDriveInfo(),
                new DriveInfo[]
                {
                    new DriveInfo
                    {
                        Name = "C:",
                        Format = "NTFS",
                        Label = "foo",
                        TotalBytes = 1,
                        Type = System.IO.DriveType.Fixed.ToString()
                    },
                    new DriveInfo
                    {
                        Name = "D:",
                        Type = System.IO.DriveType.CDRom.ToString()
                    }
                });

            var info = await collector.Read();

            Assert.Equal(2, info.Drives.Length);
            Assert.Equal(5, info.Drives[0].GetPublicPropertyCount());

            Assert.Equal("C:", info.Drives[0].Name);
            Assert.Equal("NTFS", info.Drives[0].Format);
            Assert.Equal("foo", info.Drives[0].Label);
            Assert.Equal(1UL, info.Drives[0].TotalBytes);
            Assert.Equal("Fixed", info.Drives[0].Type);
            Assert.Equal("D:", info.Drives[1].Name);
            Assert.Null(info.Drives[1].Format);
            Assert.Null(info.Drives[1].Label);
            Assert.Null(info.Drives[1].TotalBytes);
            Assert.Equal("CDRom", info.Drives[1].Type);
        }

        [Fact]
        public async Task Read_HasOsInfo()
        {
            var collector = MockCollector<StableDeviceCollector>.Mock<IOsCollector, OsInfo>(
                c => c.ReadOsInfo(), 
                new OsInfo
                {
                    Bitness = 32,
                    Edition = "Windows 10",
                    EnvironmentVariables = new Dictionary<string, object> { { "foo", "bar" } },
                    InstalledUICulture = "de-DE",
                    MachineName = "baz",
                    Version = "10.0.0.0"
                });

            var info = await collector.Read();

            Assert.Equal(6, info.Os.GetPublicPropertyCount());

            Assert.Equal(32, info.Os.Bitness);
            Assert.Equal("Windows 10", info.Os.Edition);
            Assert.Equal("bar", info.Os.EnvironmentVariables["foo"]);
            Assert.Equal("de-DE", info.Os.InstalledUICulture);
            Assert.Equal("baz", info.Os.MachineName);
            Assert.Equal("10.0.0.0", info.Os.Version);
        }
    }
}
