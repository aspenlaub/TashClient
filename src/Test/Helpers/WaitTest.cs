using System;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.TashClient.Test.Helpers {
    [TestClass]
    public class WaitTest {
        [TestMethod]
        public void CanWait() {
            var now = DateTime.Now;
            Wait.Until(() => true, TimeSpan.FromSeconds(1));
            var elapsedMilliSeconds = DateTime.Now.Subtract(now).TotalMilliseconds;
            Assert.IsTrue(elapsedMilliSeconds < 100);

            var now2 = DateTime.Now;
            Wait.Until(() => DateTime.Now.Subtract(now2).TotalMilliseconds > 500, TimeSpan.FromSeconds(1));
            elapsedMilliSeconds = DateTime.Now.Subtract(now2).TotalMilliseconds;
            Assert.IsTrue(elapsedMilliSeconds < 600);

           var now3 = DateTime.Now;
            Wait.Until(() => DateTime.Now.Subtract(now3).TotalMilliseconds > 2000, TimeSpan.FromSeconds(1));
            elapsedMilliSeconds = DateTime.Now.Subtract(now3).TotalMilliseconds;
            Assert.IsTrue(elapsedMilliSeconds < 1100);
        }

        [TestMethod]
        public async Task CanWaitAsync() {
            var now = DateTime.Now;
            await Wait.UntilAsync(async () => await Task.FromResult(true), TimeSpan.FromSeconds(1));
            var elapsedMilliSeconds = DateTime.Now.Subtract(now).TotalMilliseconds;
            Assert.IsTrue(elapsedMilliSeconds < 100);

            var now2 = DateTime.Now;
            await Wait.UntilAsync(async () => await Task.FromResult(DateTime.Now.Subtract(now2).TotalMilliseconds > 500), TimeSpan.FromSeconds(1));
            elapsedMilliSeconds = DateTime.Now.Subtract(now2).TotalMilliseconds;
            Assert.IsTrue(elapsedMilliSeconds < 600);

            var now3 = DateTime.Now;
            await Wait.UntilAsync(async () => await Task.FromResult(DateTime.Now.Subtract(now3).TotalMilliseconds > 2000), TimeSpan.FromSeconds(1));
            elapsedMilliSeconds = DateTime.Now.Subtract(now3).TotalMilliseconds;
            Assert.IsTrue(elapsedMilliSeconds < 1100);
        }
    }
}
