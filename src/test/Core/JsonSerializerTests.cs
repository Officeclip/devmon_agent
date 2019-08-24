using Geheb.DevMon.Agent.Core;
using System;
using Xunit;

namespace Geheb.DevMon.Agent.Test.Core
{
    public class JsonSerializerTests
    {
        readonly IJsonSerializer _serializer = new JsonSerializer();

        [Fact]
        public void SerializeWithFormatting_IsSuccessful()
        {
            var content = _serializer.SerializeWithFormatting(new { foo = "bar" });
            Assert.Equal("{\r\n  \"foo\": \"bar\"\r\n}", content);
        }

        [Fact]
        public void SerializeWithoutFormatting_IsSuccessful()
        {
            var content = _serializer.SerializeWithoutFormatting(new { foo = "bar" });
            Assert.Equal("{\"foo\":\"bar\"}", content);
        }

        [Fact]
        public void SerializeWithoutFormatting_IsSnakeCase()
        {
            var content = _serializer.SerializeWithoutFormatting(new { FooBar = "baz" });
            Assert.Equal("{\"foo_bar\":\"baz\"}", content);
        }

        [Fact]
        public void SerializeWithFormatting_IsSnakeCase()
        {
            var content = _serializer.SerializeWithFormatting(new { FooBar = "baz" });
            Assert.Equal("{\r\n  \"foo_bar\": \"baz\"\r\n}", content);
        }

        [Fact]
        public void SerializeWithoutFormatting_WithDateTime_IsSuccessful()
        {
            var content = _serializer.SerializeWithoutFormatting(new { foo = new DateTime(2000, 1, 1, 10, 11, 12) });
            Assert.Equal("{\"foo\":\"2000-01-01T10:11:12\"}", content);
        }
    }
}
