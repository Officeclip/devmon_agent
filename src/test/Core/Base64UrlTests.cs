using Geheb.DevMon.Agent.Core;
using System;
using Xunit;

namespace Geheb.DevMon.Agent.Test.Core
{
    public class Base64UrlTests
    {

        [Theory]
        [InlineData("", "")]
        [InlineData("", null)]
        [InlineData("ßß123ßß!", "w5_DnzEyM8Ofw58h")]
        public void Decode_IsSuccessful(string expected, string value)
        {
            var decoder = new Base64Url();

            var decoded = decoder.Decode(value);

            Assert.Equal(expected, decoded);
        }

        [Fact]
        public void Decode_WithInvalidData_ThrowsException()
        {
            var decoder = new Base64Url();

            Assert.Throws<FormatException>(() => decoder.Decode("Zm9v="));
        }
    }
}
