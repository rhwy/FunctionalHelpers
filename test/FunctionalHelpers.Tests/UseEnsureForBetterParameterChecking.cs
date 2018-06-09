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

        // [Fact]
        // public void verify_value_with_predicate()
        // {
        //     Assert.Throws<FunctionalHelpersException>(()=> {
        //         var sut = "hello world";

        //         Ensure.Value(sut)
        //             .ValidatesCondition(v=>v.Contains("hello"));
        //     });
        // }

        
    }
}