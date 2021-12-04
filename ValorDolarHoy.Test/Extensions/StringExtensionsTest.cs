using System;
using NUnit.Framework;

namespace ValorDolarHoy.Test.Extensions
{
    [TestFixture]
    public class StringExtensionsTest
    {
        [Test]
        public void Convert_To_Snake_Case()
        {
            string actual = "HelloWorld".ToSnakeCase();
            
            Assert.AreEqual("hello_world", actual);
        }
        
        [Test]
        public void Convert_To_Snake_Case_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => StringExtensions.ToSnakeCase(null));
        }
    }
}