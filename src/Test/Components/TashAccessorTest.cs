using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Components;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.TashClient.Test.Components {
    [TestClass]
    public class TashAccessorTest {
        [TestMethod]
        public async Task CanGetTashApp() {
            ITashAccessor sut = new TashAccessor();
            var tashApp = await sut.GetTashAppAsync();
            Assert.IsNotNull(tashApp);
        }

        [TestMethod]
        public async Task CanLaunchTashAppIfNotRunning() {
            ITashAccessor sut = new TashAccessor();
            await LaunchTashAppIfNotRunning(sut);
        }

        private static async Task LaunchTashAppIfNotRunning(ITashAccessor sut) {
            var errorsAndInfos = await sut.EnsureTashAppIsRunningAsync();
            Assert.IsFalse(errorsAndInfos.AnyErrors(), string.Join("\r\n", errorsAndInfos.Errors));
        }

        [TestMethod]
        public async Task CanGetControllableProcesses() {
            ITashAccessor sut = new TashAccessor();
            await LaunchTashAppIfNotRunning(sut);

            var processes = await sut.GetControllableProcessesAsync();
            Assert.IsNotNull(processes);
        }

        [TestMethod]
        public async Task CanPutAndGetControllableProcess() {
            ITashAccessor sut = new TashAccessor();
            await LaunchTashAppIfNotRunning(sut);

            var currentProcess = Process.GetCurrentProcess();
            var statusCode = await sut.PutControllableProcessAsync(currentProcess);
            Assert.AreEqual(HttpStatusCode.Created, statusCode);
            var process = await sut.GetControllableProcessAsync(currentProcess.Id);
            Assert.IsNotNull(process);
            Assert.AreEqual(currentProcess.ProcessName, process.Title);
        }
    }
}
