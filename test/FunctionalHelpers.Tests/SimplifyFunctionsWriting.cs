namespace FunctionalHelpers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using static FunctionalHelpers.Core;


    public class SimplifyFunctions
    {
        [Fact]
        public void use_helper_to_declare_functions()
        {
            Assert.ThrowsAsync<Exception>(()=> {
                var increment = f((int i)=> i++);
                throw new Exception("ok");
            });
        }

        [Fact]
        public void use_helper_to_continue_self_with_a_next_function()
        {
            var toupper = f((string[] list) => list
                .ToList().Select(x=>x.ToUpper()));

            var join = f((IEnumerable<string> list) => string.Join(",",list));
            
            string message = "a,b,c".Split(new []{','})
                    .Then(toupper)
                    .Then(join);
            
            Assert.Equal("A,B,C",message);

        }

        
    }

    
    
    

    
}
