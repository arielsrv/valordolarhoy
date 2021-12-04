using System;
using Xunit;

namespace ValorDolarHoy.Test.Extensions
{
    public class StringExtensionsTest
    {
        [Fact]
        public void Convert_To_Snake_Case()
        {
            string actual = "HelloWorld".ToSnakeCase();
            
            Assert.Equal("hello_world", actual);
        }
        
        [Fact]
        public void Convert_To_Snake_Case_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => StringExtensions.ToSnakeCase(null));
        }
    }
}