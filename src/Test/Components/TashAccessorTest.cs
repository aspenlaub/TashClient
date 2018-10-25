using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Tash.Model;
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
            Assert.IsTrue(statusCode == HttpStatusCode.Created || statusCode == HttpStatusCode.NoContent);
            var process = await sut.GetControllableProcessAsync(currentProcess.Id);
            Assert.IsNotNull(process);
            Assert.AreEqual(currentProcess.ProcessName, process.Title);
        }

        [TestMethod]
        public async Task CanConfirmAliveness() {
            ITashAccessor sut = new TashAccessor();
            await LaunchTashAppIfNotRunning(sut);

            var currentProcess = Process.GetCurrentProcess();
            var statusCode = await sut.PutControllableProcessAsync(currentProcess);
            Assert.IsTrue(statusCode == HttpStatusCode.Created || statusCode == HttpStatusCode.NoContent);

            var now = DateTime.Now.AddHours(1);
            statusCode = await sut.ConfirmAliveAsync(currentProcess.Id, now, true);
            Assert.AreEqual(HttpStatusCode.NoContent, statusCode);

            var process = await sut.GetControllableProcessAsync(currentProcess.Id);
            Assert.IsNotNull(process);
            Assert.AreEqual(now, process.ConfirmedAt);
            Assert.IsTrue(process.Busy);
        }

        [TestMethod]
        public async Task CanGetControllableProcessTasks() {
            ITashAccessor sut = new TashAccessor();
            await LaunchTashAppIfNotRunning(sut);

            var processTasks = await sut.GetControllableProcessTasksAsync();
            Assert.IsNotNull(processTasks);
        }

        [TestMethod]
        public async Task CanPutAndGetControllableProcessTask() {
            ITashAccessor sut = new TashAccessor();
            await LaunchTashAppIfNotRunning(sut);

            var controllableProcessTask = CreateControllableProcessTask();
            var statusCode = await sut.PutControllableProcessTaskAsync(controllableProcessTask);
            Assert.AreEqual(HttpStatusCode.Created, statusCode);
            var processTask = await sut.GetControllableProcessTaskAsync(controllableProcessTask.Id);
            Assert.IsNotNull(processTask);
            Assert.AreEqual(controllableProcessTask.ControlName, processTask.ControlName);
        }


        [TestMethod]
        public async Task CanConfirmStatus() {
            ITashAccessor sut = new TashAccessor();
            await LaunchTashAppIfNotRunning(sut);

            var controllableProcessTask = CreateControllableProcessTask();
            var statusCode = await sut.PutControllableProcessTaskAsync(controllableProcessTask);
            Assert.AreEqual(HttpStatusCode.Created, statusCode);

            statusCode = await sut.ConfirmStatusAsync(controllableProcessTask.Id, ControllableProcessTaskStatus.BadRequest);
            Assert.AreEqual(HttpStatusCode.NoContent, statusCode);

            var processTask = await sut.GetControllableProcessTaskAsync(controllableProcessTask.Id);
            Assert.IsNotNull(processTask);
            Assert.AreEqual(ControllableProcessTaskStatus.BadRequest, processTask.Status);
        }

        private ControllableProcessTask CreateControllableProcessTask() {
            return new ControllableProcessTask {
                Id = Guid.NewGuid(),
                ProcessId = 4711,
                Type = ControllableProcessTaskType.SelectComboItem,
                ControlName = "ScriptComboBox",
                Status = ControllableProcessTaskStatus.Processing,
                Text = "This is my selection"
            };
        }
    }
}
