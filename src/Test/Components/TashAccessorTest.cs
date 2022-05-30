using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Components;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tash;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Components;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.TashClient.Test.Components;

[TestClass]
public class TashAccessorTest {
    private readonly IDvinRepository DvinRepository;
    private readonly ISimpleLogger SimpleLogger;
    private readonly ILogConfiguration LogConfiguration;
    private TashAccessor Sut;

    public TashAccessorTest() {
        var builder = new ContainerBuilder().UseDvinAndPegh("TashClient", new DummyCsArgumentPrompter());
        var container = builder.Build();
        DvinRepository = container.Resolve<IDvinRepository>();
        SimpleLogger = container.Resolve<ISimpleLogger>();
        LogConfiguration = container.Resolve<ILogConfiguration>();
    }

    [TestInitialize]
    public void Initialize() {
        Sut = new TashAccessor(DvinRepository, SimpleLogger, LogConfiguration);
    }

    [TestMethod]
    public async Task CanGetTashApp() {
        var errorsAndInfos = new ErrorsAndInfos();
        var tashApp = await Sut.GetTashAppAsync(errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.IsNotNull(tashApp);
    }

    [TestMethod]
    public async Task CanLaunchTashAppIfNotRunning() {
        await LaunchTashAppIfNotRunning();
    }

    private async Task LaunchTashAppIfNotRunning() {
        var errorsAndInfos = await Sut.EnsureTashAppIsRunningAsync();
        Assert.IsFalse(errorsAndInfos.AnyErrors(), string.Join("\r\n", errorsAndInfos.Errors));
    }

    [TestMethod]
    public async Task CanGetControllableProcesses() {
        await LaunchTashAppIfNotRunning();

        var processes = await Sut.GetControllableProcessesAsync();
        Assert.IsNotNull(processes);
    }

    [TestMethod]
    public async Task CanPutAndGetControllableProcess() {
        await LaunchTashAppIfNotRunning();

        var currentProcess = Process.GetCurrentProcess();
        var statusCode = await Sut.PutControllableProcessAsync(currentProcess);
        Assert.IsTrue(statusCode == HttpStatusCode.Created || statusCode == HttpStatusCode.NoContent);
        var process = await Sut.GetControllableProcessAsync(currentProcess.Id);
        Assert.IsNotNull(process);
        Assert.AreEqual(currentProcess.ProcessName, process.Title);
    }

    [TestMethod]
    public async Task CanConfirmAliveness() {
        await LaunchTashAppIfNotRunning();

        var currentProcess = Process.GetCurrentProcess();
        var statusCode = await Sut.PutControllableProcessAsync(currentProcess);
        Assert.IsTrue(statusCode == HttpStatusCode.Created || statusCode == HttpStatusCode.NoContent);

        var now = DateTime.Now.AddHours(1);
        statusCode = await Sut.ConfirmAliveAsync(currentProcess.Id, now, ControllableProcessStatus.Busy);
        Assert.AreEqual(HttpStatusCode.NoContent, statusCode);

        var process = await Sut.GetControllableProcessAsync(currentProcess.Id);
        Assert.IsNotNull(process);
        Assert.AreEqual(now, process.ConfirmedAt);
        Assert.AreEqual(ControllableProcessStatus.Busy, process.Status);
    }

    [TestMethod]
    public async Task CanGetControllableProcessTasks() {
        await LaunchTashAppIfNotRunning();

        var processTasks = await Sut.GetControllableProcessTasksAsync();
        Assert.IsNotNull(processTasks);
    }

    [TestMethod]
    public async Task CanPutAndGetControllableProcessTask() {
        await LaunchTashAppIfNotRunning();

        var controllableProcessTask = CreateControllableProcessTask();
        var statusCode = await Sut.PutControllableProcessTaskAsync(controllableProcessTask);
        Assert.AreEqual(HttpStatusCode.Created, statusCode);
        var processTask = await Sut.GetControllableProcessTaskAsync(controllableProcessTask.Id);
        Assert.IsNotNull(processTask);
        Assert.AreEqual(controllableProcessTask.ControlName, processTask.ControlName);
    }


    [TestMethod]
    public async Task CanConfirmStatus() {
        await LaunchTashAppIfNotRunning();

        var controllableProcessTask = CreateControllableProcessTask();
        var statusCode = await Sut.PutControllableProcessTaskAsync(controllableProcessTask);
        Assert.AreEqual(HttpStatusCode.Created, statusCode);

        statusCode = await Sut.ConfirmStatusAsync(controllableProcessTask.Id, ControllableProcessTaskStatus.BadRequest);
        Assert.AreEqual(HttpStatusCode.NoContent, statusCode);

        var processTask = await Sut.GetControllableProcessTaskAsync(controllableProcessTask.Id);
        Assert.IsNotNull(processTask);
        Assert.AreEqual(ControllableProcessTaskStatus.BadRequest, processTask.Status);
    }

    [TestMethod]
    public async Task CanConfirmStatusWithTextAndErrorMessage() {
        await LaunchTashAppIfNotRunning();

        var controllableProcessTask = CreateControllableProcessTask();
        var statusCode = await Sut.PutControllableProcessTaskAsync(controllableProcessTask);
        Assert.AreEqual(HttpStatusCode.Created, statusCode);

        const string text = "This is not a text";
        const string errorMessage = "This is not an error message";
        statusCode = await Sut.ConfirmStatusAsync(controllableProcessTask.Id, ControllableProcessTaskStatus.BadRequest, text, errorMessage);
        Assert.AreEqual(HttpStatusCode.NoContent, statusCode);

        var processTask = await Sut.GetControllableProcessTaskAsync(controllableProcessTask.Id);
        Assert.IsNotNull(processTask);
        Assert.AreEqual(ControllableProcessTaskStatus.BadRequest, processTask.Status);
        Assert.AreEqual(text, processTask.Text);
        Assert.AreEqual(errorMessage, processTask.ErrorMessage);
    }

    private ControllableProcessTask CreateControllableProcessTask() {
        return new ControllableProcessTask {
            Id = Guid.NewGuid(),
            ProcessId = new Random().Next(1, 32767),
            Type = "SelectComboItem",
            ControlName = "ScriptComboBox",
            Status = ControllableProcessTaskStatus.Processing,
            Text = "This is my selection"
        };
    }

    [TestMethod]
    public async Task CanConfirmDeadAsync() {
        await LaunchTashAppIfNotRunning();

        var currentProcess = Process.GetCurrentProcess();
        var statusCode = await Sut.PutControllableProcessAsync(currentProcess);
        Assert.IsTrue(statusCode == HttpStatusCode.Created || statusCode == HttpStatusCode.NoContent);

        var now = DateTime.Now;
        await Sut.ConfirmDeadAsync(currentProcess.Id);

        var process = await Sut.GetControllableProcessAsync(currentProcess.Id);
        Assert.IsNotNull(process);
        Assert.AreEqual(ControllableProcessStatus.Dead, process.Status);
        Assert.IsTrue(process.ConfirmedAt >= now);
    }

    [TestMethod]
    public async Task CanConfirmDeadWhileClosing() {
        await LaunchTashAppIfNotRunning();

        var currentProcess = Process.GetCurrentProcess();
        var statusCode = await Sut.PutControllableProcessAsync(currentProcess);
        Assert.IsTrue(statusCode == HttpStatusCode.Created || statusCode == HttpStatusCode.NoContent);

        var now = DateTime.Now;
        await Sut.ConfirmDeadWhileClosingAsync(currentProcess.Id);

        var process = await Sut.GetControllableProcessAsync(currentProcess.Id);
        Assert.IsNotNull(process);
        Assert.AreEqual(ControllableProcessStatus.Dead, process.Status);
        Assert.IsTrue(process.ConfirmedAt >= now);
    }

    [TestMethod]
    public async Task CanGetOkayToMarkTaskAsCompleted() {
        await LaunchTashAppIfNotRunning();

        var controllableProcessTask = CreateControllableProcessTask();
        var statusCode = await Sut.PutControllableProcessTaskAsync(controllableProcessTask);
        Assert.AreEqual(HttpStatusCode.Created, statusCode);

        Assert.IsTrue(Sut.MarkTaskAsCompleted(controllableProcessTask, controllableProcessTask.ProcessId, controllableProcessTask.Type, controllableProcessTask.ControlName, controllableProcessTask.Text));
        Assert.IsFalse(Sut.MarkTaskAsCompleted(null, controllableProcessTask.ProcessId, controllableProcessTask.Type, controllableProcessTask.ControlName, controllableProcessTask.Text));
        Assert.IsFalse(Sut.MarkTaskAsCompleted(controllableProcessTask, controllableProcessTask.ProcessId + 1, controllableProcessTask.Type, controllableProcessTask.ControlName, controllableProcessTask.Text));
        Assert.IsFalse(Sut.MarkTaskAsCompleted(controllableProcessTask, controllableProcessTask.ProcessId, controllableProcessTask.Type, controllableProcessTask.ControlName + "X", controllableProcessTask.Text));
        Assert.IsFalse(Sut.MarkTaskAsCompleted(controllableProcessTask, controllableProcessTask.ProcessId, controllableProcessTask.Type, controllableProcessTask.ControlName, controllableProcessTask.Text + "X"));
    }

    [TestMethod]
    public async Task CanPickRequestedTask() {
        await LaunchTashAppIfNotRunning();

        var controllableProcessTask = CreateControllableProcessTask();
        controllableProcessTask.Status = ControllableProcessTaskStatus.Requested;
        var statusCode = await Sut.PutControllableProcessTaskAsync(controllableProcessTask);
        Assert.AreEqual(HttpStatusCode.Created, statusCode);

        var pickedProcessTask = await Sut.PickRequestedTask(controllableProcessTask.ProcessId);
        Assert.IsNotNull(pickedProcessTask);
        Assert.AreEqual(controllableProcessTask.Id, pickedProcessTask.Id);

        controllableProcessTask.Status = ControllableProcessTaskStatus.Processing;
        statusCode = await Sut.PutControllableProcessTaskAsync(controllableProcessTask);
        Assert.AreEqual(HttpStatusCode.NoContent, statusCode);

        pickedProcessTask = await Sut.PickRequestedTask(controllableProcessTask.ProcessId);
        Assert.IsNull(pickedProcessTask);
    }

    [TestMethod]
    public async Task CanAssumeDeath() {
        await LaunchTashAppIfNotRunning();

        var currentProcess = Process.GetCurrentProcess();
        var statusCode = await Sut.PutControllableProcessAsync(currentProcess);
        Assert.IsTrue(statusCode == HttpStatusCode.Created || statusCode == HttpStatusCode.NoContent);

        await Sut.AssumeDeath(p => p.ProcessId == currentProcess.Id);

        var process = await Sut.GetControllableProcessAsync(currentProcess.Id);
        Assert.IsNotNull(process);
        Assert.AreEqual(ControllableProcessStatus.Dead, process.Status);
    }

    [TestMethod]
    public async Task CanFindIdleProcess() {
        await LaunchTashAppIfNotRunning();

        var currentProcess = Process.GetCurrentProcess();
        var statusCode = await Sut.PutControllableProcessAsync(currentProcess);
        Assert.IsTrue(statusCode == HttpStatusCode.Created || statusCode == HttpStatusCode.NoContent);

        var findIdleProcessResult = await Sut.FindIdleProcess(p => p.ProcessId == currentProcess.Id);
        Assert.IsNotNull(findIdleProcessResult.ControllableProcess);
        Assert.AreEqual(currentProcess.Id, findIdleProcessResult.ControllableProcess.ProcessId);
    }

    [TestMethod]
    public async Task CanAwaitCompletionAsync() {
        await LaunchTashAppIfNotRunning();

        var controllableProcessTask = CreateControllableProcessTask();
        controllableProcessTask.Status = ControllableProcessTaskStatus.Processing;
        var statusCode = await Sut.PutControllableProcessTaskAsync(controllableProcessTask);
        Assert.AreEqual(HttpStatusCode.Created, statusCode);

        var now = DateTime.Now;
        var task = await Sut.AwaitCompletionAsync(controllableProcessTask.Id, 1000);
        var elapsedMilliSeconds = DateTime.Now.Subtract(now).TotalMilliseconds;
        Assert.IsTrue(elapsedMilliSeconds >= 1000);
        Assert.AreEqual(ControllableProcessTaskStatus.Processing, task.Status);

        controllableProcessTask.Status = ControllableProcessTaskStatus.Completed;
        statusCode = await Sut.PutControllableProcessTaskAsync(controllableProcessTask);
        Assert.AreEqual(HttpStatusCode.NoContent, statusCode);

        now = DateTime.Now;
        task = await Sut.AwaitCompletionAsync(controllableProcessTask.Id, 1000);
        elapsedMilliSeconds = DateTime.Now.Subtract(now).TotalMilliseconds;
        Assert.IsTrue(elapsedMilliSeconds < 100);
        Assert.AreEqual(ControllableProcessTaskStatus.Completed, task.Status);
    }
}