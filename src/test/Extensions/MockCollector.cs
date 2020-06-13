using devmon_library.Core;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace devmon_test.Extensions
{
    internal static class MockCollector<T> where T : class
    {
        public static T Mock<TCollector, TResult>(Expression<Func<TCollector, Task<TResult>>> action, TResult result)
            where TCollector : class
        {
            var mock = new Mock<TCollector>();
            mock.Setup(action).ReturnsAsync(result);

            var types = new[]
            {
                typeof(ICpuCollector),
                typeof(IMemoryCollector),
                typeof(INetworkCollector),
                typeof(IDriveCollector),
                typeof(IOsCollector)
            };

            var @params = new List<object>();

            foreach (var t in types)
            {
                if (t == typeof(TCollector))
                {
                    @params.Add(mock.Object);
                }
                else
                {
                    var mockType = typeof(Mock<>);
                    var mockGenericType = mockType.MakeGenericType(t);
                    var mockInstance = Activator.CreateInstance(mockGenericType);
                    var mockObject = mockGenericType.GetProperty(nameof(mock.Object), t).GetValue(mockInstance, null);
                    @params.Add(mockObject);
                }
            }

            var instance = Activator.CreateInstance(typeof(T), @params.ToArray());
            return instance as T;
        }
    }
}
