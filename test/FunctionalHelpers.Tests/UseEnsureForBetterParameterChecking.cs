namespace FunctionalHelpers.Tests
{
    using Xunit;
    using static FunctionalHelpers.Core;

    public class UseEnsureForBetterParameterChecking
    {
        [Fact]
        public void verify_value_not_null()
        {
            Assert.Throws<FunctionalHelpersException>(()=> {
                Ensure.ValueNotNull<object>(null);
            });
        }
    }
}