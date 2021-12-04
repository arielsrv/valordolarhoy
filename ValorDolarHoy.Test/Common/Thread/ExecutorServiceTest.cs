using System.Threading;
using NUnit.Framework;
using ValorDolarHoy.Common.Thread;

namespace ValorDolarHoy.Test.Common
{
    [TestFixture]
    public class ExecutorServiceTest
    {
        private ExecutorService executorService;
        
        [SetUp]
        public void Setup()
        {
            this.executorService = ExecutorService.NewFixedThreadPool(10);
        }
        
        [Test]
        public void Fire_And_Forget()
        {
            int value = 0;
            this.executorService.Run(() =>
            {
                Thread.Sleep(100);
                value = int.MaxValue;
            });
            
            Assert.AreEqual(0, value);
        }
        
        [Test]
        public void Fire_And_Forget_Expected_Value()
        {
            int value = 0;
            this.executorService.Run(() =>
            {
                Thread.Sleep(100);
                value = int.MaxValue;
            });
            
            Thread.Sleep(200);
            Assert.AreEqual(int.MaxValue, value);
        }
    }
}