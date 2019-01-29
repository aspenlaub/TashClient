using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tash;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Components;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.TashClient.Test.Components {
    [TestClass]
    public class TashAccessorTest {
        private readonly IComponentProvider vComponentProvider;

        public TashAccessorTest() {
            vComponentProvider = new ComponentProvider();
        }

        [TestMethod]
        public async Task CanGetTashApp() {
            ITashAccessor sut = new TashAccessor(vComponentProvider);
            var errorsAndInfos = new ErrorsAndInfos();
            var tashApp = await sut.GetTashAppAsync(errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
            Assert.IsNotNull(tashApp);
        }

        [TestMethod]
        public async Task CanLaunchTashAppIfNotRunning() {
            ITashAccessor sut = new TashAccessor(vComponentProvider);
            await LaunchTashAppIfNotRunning(sut);
        }

        private static async Task LaunchTashAppIfNotRunning(ITashAccessor sut) {
            var errorsAndInfos = await sut.EnsureTashAppIsRunningAsync();
            Assert.IsFalse(errorsAndInfos.AnyErrors(), string.Join("\r\n", errorsAndInfos.Errors));
        }

        [TestMethod]
        public async Task CanGetControllableProcesses() {
            ITashAccessor sut = new TashAccessor(vComponentProvider);
            await LaunchTashAppIfNotRunning(sut);

            var processes = await sut.GetControllableProcessesAsync();
            Assert.IsNotNull(processes);
        }

        [TestMethod]
        public async Task CanPutAndGetControllableProcess() {
            ITashAccessor sut = new TashAccessor(vComponentProvider);
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
            ITashAccessor sut = new TashAccessor(vComponentProvider);
            await LaunchTashAppIfNotRunning(sut);

            var currentProcess = Process.GetCurrentProcess();
            var statusCode = await sut.PutControllableProcessAsync(currentProcess);
            Assert.IsTrue(statusCode == HttpStatusCode.Created || statusCode == HttpStatusCode.NoContent);

            var now = DateTime.Now.AddHours(1);
            statusCode = await sut.ConfirmAliveAsync(currentProcess.Id, now, ControllableProcessStatus.Busy);
            Assert.AreEqual(HttpStatusCode.NoContent, statusCode);

            var process = await sut.GetControllableProcessAsync(currentProcess.Id);
            Assert.IsNotNull(process);
            Assert.AreEqual(now, process.ConfirmedAt);
            Assert.AreEqual(ControllableProcessStatus.Busy, process.Status);
        }

        [TestMethod]
        public async Task CanGetControllableProcessTasks() {
            ITashAccessor sut = new TashAccessor(vComponentProvider);
            await LaunchTashAppIfNotRunning(sut);

            var processTasks = await sut.GetControllableProcessTasksAsync();
            Assert.IsNotNull(processTasks);
        }

        [TestMethod]
        public async Task CanPutAndGetControllableProcessTask() {
            ITashAccessor sut = new TashAccessor(vComponentProvider);
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
            ITashAccessor sut = new TashAccessor(vComponentProvider);
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

        [TestMethod]
        public async Task CanConfirmStatusWithTextAndErrorMessage() {
            ITashAccessor sut = new TashAccessor(vComponentProvider);
            await LaunchTashAppIfNotRunning(sut);

            var controllableProcessTask = CreateControllableProcessTask();
            var statusCode = await sut.PutControllableProcessTaskAsync(controllableProcessTask);
            Assert.AreEqual(HttpStatusCode.Created, statusCode);

            const string text = "This is not a text";
            const string errorMessage = "This is not an error message";
            statusCode = await sut.ConfirmStatusAsync(controllableProcessTask.Id, ControllableProcessTaskStatus.BadRequest, text, errorMessage);
            Assert.AreEqual(HttpStatusCode.NoContent, statusCode);

            var processTask = await sut.GetControllableProcessTaskAsync(controllableProcessTask.Id);
            Assert.IsNotNull(processTask);
            Assert.AreEqual(ControllableProcessTaskStatus.BadRequest, processTask.Status);
            Assert.AreEqual(text, processTask.Text);
            Assert.AreEqual(errorMessage, processTask.ErrorMessage);
        }

        private ControllableProcessTask CreateControllableProcessTask() {
            return new ControllableProcessTask {
                Id = Guid.NewGuid(),
                ProcessId = new Random().Next(1, 32767),
                Type = ControllableProcessTaskType.SelectComboItem,
                ControlName = "ScriptComboBox",
                Status = ControllableProcessTaskStatus.Processing,
                Text = "This is my selection"
            };
        }

        [TestMethod]
        public async Task CanConfirmDeadAsync() {
            ITashAccessor sut = new TashAccessor(vComponentProvider);
            await LaunchTashAppIfNotRunning(sut);

            var currentProcess = Process.GetCurrentProcess();
            var statusCode = await sut.PutControllableProcessAsync(currentProcess);
            Assert.IsTrue(statusCode == HttpStatusCode.Created || statusCode == HttpStatusCode.NoContent);

            var now = DateTime.Now;
            await sut.ConfirmDeadAsync(currentProcess.Id);

            var process = await sut.GetControllableProcessAsync(currentProcess.Id);
            Assert.IsNotNull(process);
            Assert.AreEqual(ControllableProcessStatus.Dead, process.Status);
            Assert.IsTrue(process.ConfirmedAt >= now);
        }

        [TestMethod]
        public async Task CanConfirmDeadWhileClosing() {
            ITashAccessor sut = new TashAccessor(vComponentProvider);
            await LaunchTashAppIfNotRunning(sut);

            var currentProcess = Process.GetCurrentProcess();
            var statusCode = await sut.PutControllableProcessAsync(currentProcess);
            Assert.IsTrue(statusCode == HttpStatusCode.Created || statusCode == HttpStatusCode.NoContent);

            var now = DateTime.Now;
            sut.ConfirmDeadWhileClosing(currentProcess.Id);

            var process = await sut.GetControllableProcessAsync(currentProcess.Id);
            Assert.IsNotNull(process);
            Assert.AreEqual(ControllableProcessStatus.Dead, process.Status);
            Assert.IsTrue(process.ConfirmedAt >= now);
        }

        [TestMethod]
        public async Task CanGetOkayToMarkTaskAsCompleted() {
            ITashAccessor sut = new TashAccessor(vComponentProvider);
            await LaunchTashAppIfNotRunning(sut);

            var controllableProcessTask = CreateControllableProcessTask();
            var statusCode = await sut.PutControllableProcessTaskAsync(controllableProcessTask);
            Assert.AreEqual(HttpStatusCode.Created, statusCode);

            Assert.IsTrue(sut.MarkTaskAsCompleted(controllableProcessTask, controllableProcessTask.ProcessId, controllableProcessTask.Type, controllableProcessTask.ControlName, controllableProcessTask.Text));
            Assert.IsFalse(sut.MarkTaskAsCompleted(null, controllableProcessTask.ProcessId, controllableProcessTask.Type, controllableProcessTask.ControlName, controllableProcessTask.Text));
            Assert.IsFalse(sut.MarkTaskAsCompleted(controllableProcessTask, controllableProcessTask.ProcessId + 1, controllableProcessTask.Type, controllableProcessTask.ControlName, controllableProcessTask.Text));
            Assert.IsFalse(sut.MarkTaskAsCompleted(controllableProcessTask, controllableProcessTask.ProcessId, controllableProcessTask.Type, controllableProcessTask.ControlName + "X", controllableProcessTask.Text));
            Assert.IsFalse(sut.MarkTaskAsCompleted(controllableProcessTask, controllableProcessTask.ProcessId, controllableProcessTask.Type, controllableProcessTask.ControlName, controllableProcessTask.Text + "X"));
        }

        [TestMethod]
        public async Task CanPickRequestedTask() {
            ITashAccessor sut = new TashAccessor(vComponentProvider);
            await LaunchTashAppIfNotRunning(sut);

            var controllableProcessTask = CreateControllableProcessTask();
            controllableProcessTask.Status = ControllableProcessTaskStatus.Requested;
            var statusCode = await sut.PutControllableProcessTaskAsync(controllableProcessTask);
            Assert.AreEqual(HttpStatusCode.Created, statusCode);

            var pickedProcessTask = await sut.PickRequestedTask(controllableProcessTask.ProcessId);
            Assert.IsNotNull(pickedProcessTask);
            Assert.AreEqual(controllableProcessTask.Id, pickedProcessTask.Id);

            controllableProcessTask.Status = ControllableProcessTaskStatus.Processing;
            statusCode = await sut.PutControllableProcessTaskAsync(controllableProcessTask);
            Assert.AreEqual(HttpStatusCode.NoContent, statusCode);

            pickedProcessTask = await sut.PickRequestedTask(controllableProcessTask.ProcessId);
            Assert.IsNull(pickedProcessTask);
        }

        [TestMethod]
        public async Task CanAssumeDeath() {
            ITashAccessor sut = new TashAccessor(vComponentProvider);
            await LaunchTashAppIfNotRunning(sut);

            var currentProcess = Process.GetCurrentProcess();
            var statusCode = await sut.PutControllableProcessAsync(currentProcess);
            Assert.IsTrue(statusCode == HttpStatusCode.Created || statusCode == HttpStatusCode.NoContent);

            await sut.AssumeDeath(p => p.ProcessId == currentProcess.Id);

            var process = await sut.GetControllableProcessAsync(currentProcess.Id);
            Assert.IsNotNull(process);
            Assert.AreEqual(ControllableProcessStatus.Dead, process.Status);
        }

        [TestMethod]
        public async Task CanFindIdleProcess() {
            ITashAccessor sut = new TashAccessor(vComponentProvider);
            await LaunchTashAppIfNotRunning(sut);

            var currentProcess = Process.GetCurrentProcess();
            var statusCode = await sut.PutControllableProcessAsync(currentProcess);
            Assert.IsTrue(statusCode == HttpStatusCode.Created || statusCode == HttpStatusCode.NoContent);

            var findIdleProcessResult = await sut.FindIdleProcess(p => p.ProcessId == currentProcess.Id);
            Assert.IsNotNull(findIdleProcessResult.ControllableProcess);
            Assert.AreEqual(currentProcess.Id, findIdleProcessResult.ControllableProcess.ProcessId);
        }

        [TestMethod]
        public async Task CanAwaitCompletionAsync() {
            ITashAccessor sut = new TashAccessor(vComponentProvider);
            await LaunchTashAppIfNotRunning(sut);

            var controllableProcessTask = CreateControllableProcessTask();
            controllableProcessTask.Status = ControllableProcessTaskStatus.Processing;
            var statusCode = await sut.PutControllableProcessTaskAsync(controllableProcessTask);
            Assert.AreEqual(HttpStatusCode.Created, statusCode);

            var now = DateTime.Now;
            var task = await sut.AwaitCompletionAsync(controllableProcessTask.Id, 1000);
            var elapsedMilliSeconds = DateTime.Now.Subtract(now).TotalMilliseconds;
            Assert.IsTrue(elapsedMilliSeconds >= 1000);
            Assert.AreEqual(ControllableProcessTaskStatus.Processing, task.Status);

            controllableProcessTask.Status = ControllableProcessTaskStatus.Completed;
            statusCode = await sut.PutControllableProcessTaskAsync(controllableProcessTask);
            Assert.AreEqual(HttpStatusCode.NoContent, statusCode);

            now = DateTime.Now;
            task = await sut.AwaitCompletionAsync(controllableProcessTask.Id, 1000);
            elapsedMilliSeconds = DateTime.Now.Subtract(now).TotalMilliseconds;
            Assert.IsTrue(elapsedMilliSeconds < 100);
            Assert.AreEqual(ControllableProcessTaskStatus.Completed, task.Status);
        }
    }
}
