namespace FunctionalHelpers.Tests
{
    using Xunit;

    public class UseOptionsForBetterTypes
    {
        [Fact]
        public void use_options()
        {
            var one = None<int>.New;
            var two = Some<int>.NewValue(42);

            int value = one.WithValue(
                none : ()=> 0,
                some : i => i);
            
            Assert.Equal(0,value);

            value = two.WithValue(
                none : () => 0,
                some : i => i);

            Assert.Equal(42,value);
            
        }
    }
}