using System;
using Xunit;
using FunctionalHelpers;

namespace FunctionalHelpers.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Assert.Equal("Breaker",Class1.Ice);
        }
    }
}
