namespace FunctionalHelpers.Tests
{
    using System;
    using Xunit;
    using static FunctionalHelpers.Core;

    public class UseORailwayForBetterContinuations
    {
        [Fact]
        public void use_result()
        {
            Result<int> inc(int value) => Success(value+1);

            var one = inc(1)
                .Then(
                    success : i=> i,
                    failure : _=> 0)
                .Final(
                    success : i => $"value={i}",
                    failure : _=> $"error");

            Assert.Equal("value=2",one);
        }

        [Fact]
        public void use_result_and_continue_with_another()
        {
            Result<int> inc(int value) => Success(value+1);

            var one = inc(1)
                .Then(
                    success : i=>inc(i),
                    failure : _=> Success(0))
                .Final(
                    success : i => $"value={i}",
                    failure : _=> $"error");

            Assert.Equal("value=3",one);
        }

        [Fact]
        public void use_result_only_with_success_when_needed()
        {
            Result<int> inc(int value) => Success(value+1);
            Func<int,Result<int>> div(int divider) => (int value) => Success(value/divider);
            var sut = inc(1)
                .Then(inc)
                .Then(div(0))
                .Final(
                    success : i => $"value={i}",
                    failure : e => e.Message);

            Assert.Equal("Attempted to divide by zero.",sut);
        }

        [Fact]
        public void use_result_with_arrows_with_success()
        {
            Func<int,Result<int>> inc = (int value) => Success(value+1);
            NextOperationResult<int> divideBy(int divider) => 
                (value) => value.Then(x=>Success(x/divider));
            var sut = Success(4)
                    >= inc
                    >= inc
                    >= divideBy(2)
                    <= (
                        success : i => $"value={i}",
                        failure : e => e.Message);

            Assert.Equal("value=3",sut);
        }

        [Fact]
        public void use_result_with_arrows_with_failure()
        {
            Func<int,Result<int>> inc = (int value) => Success(value+1);
            NextOperationResult<int> divideBy(int divider) => 
                (value) => value.Then(x=>Success(x/divider));
            var sut = Success(4)
                    >= inc
                    >= inc
                    >= divideBy(0)
                    <= (
                        success : i => $"value={i}",
                        failure : e => e.Message);

            Assert.Equal("Attempted to divide by zero.",sut);
        }
    }
}