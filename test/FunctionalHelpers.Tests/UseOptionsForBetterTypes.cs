namespace FunctionalHelpers.Tests
{
    using System;
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

        [Fact]
        public void use_option_from_valid_existing_object()
        {
            var hello = "hello";
            var opt = hello.AsOption();

            opt.WithValue(
                none : ()=> throw new Exception("should not happend"),
                some : value => Assert.Equal("hello",value)
            );
        }

        [Fact]
        public void use_option_from_valid_null_object()
        {
            object IamBad = null;
            var opt = IamBad.AsOption();

            Assert.Equal(None<object>.New,opt);
        }

    }
}