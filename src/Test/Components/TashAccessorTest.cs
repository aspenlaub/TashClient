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
    private readonly IDvinRepository _DvinRepository;
    private readonly ISimpleLogger _SimpleLogger;
    private readonly ILogConfiguration _LogConfiguration;
    private readonly IMethodNamesFromStackFramesExtractor _MethodNamesFromStackFramesExtractor;
    private TashAccessor _Sut;

    public TashAccessorTest() {
        var builder = new ContainerBuilder().UseDvinAndPegh("TashClient", new DummyCsArgumentPrompter());
        var container = builder.Build();
        _DvinRepository = container.Resolve<IDvinRepository>();
        _SimpleLogger = container.Resolve<ISimpleLogger>();
        _LogConfiguration = container.Resolve<ILogConfiguration>();
        _MethodNamesFromStackFramesExtractor = new MethodNamesFromStackFramesExtractor();
    }

    [TestInitialize]
    public void Initialize() {
        _Sut = new TashAccessor(_DvinRepository, _SimpleLogger, _LogConfiguration, _MethodNamesFromStackFramesExtractor);
    }

    [TestMethod]
    public async Task CanGetTashApp() {
        var errorsAndInfos = new ErrorsAndInfos();
        var tashApp = await _Sut.GetTashAppAsync(errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.IsNotNull(tashApp);
    }

    [TestMethod]
    public async Task CanLaunchTashAppIfNotRunning() {
        await LaunchTashAppIfNotRunning();
    }

    private async Task LaunchTashAppIfNotRunning() {
        var errorsAndInfos = await _Sut.EnsureTashAppIsRunningAsync();
        Assert.IsFalse(errorsAndInfos.AnyErrors(), string.Join("\r\n", errorsAndInfos.Errors));
    }

    [TestMethod]
    public async Task CanGetControllableProcesses() {
        await LaunchTashAppIfNotRunning();

        var processes = await _Sut.GetControllableProcessesAsync();
        Assert.IsNotNull(processes);
    }

    [TestMethod]
    public async Task CanPutAndGetControllableProcess() {
        await LaunchTashAppIfNotRunning();

        var currentProcess = Process.GetCurrentProcess();
        var statusCode = await _Sut.PutControllableProcessAsync(currentProcess);
        Assert.IsTrue(statusCode == HttpStatusCode.Created || statusCode == HttpStatusCode.NoContent);
        var process = await _Sut.GetControllableProcessAsync(currentProcess.Id);
        Assert.IsNotNull(process);
        Assert.AreEqual(currentProcess.ProcessName, process.Title);
    }

    [TestMethod]
    public async Task CanConfirmAliveness() {
        await LaunchTashAppIfNotRunning();

        var currentProcess = Process.GetCurrentProcess();
        var statusCode = await _Sut.PutControllableProcessAsync(currentProcess);
        Assert.IsTrue(statusCode == HttpStatusCode.Created || statusCode == HttpStatusCode.NoContent);

        var now = DateTime.Now.AddHours(1);
        statusCode = await _Sut.ConfirmAliveAsync(currentProcess.Id, now, ControllableProcessStatus.Busy);
        Assert.AreEqual(HttpStatusCode.NoContent, statusCode);

        var process = await _Sut.GetControllableProcessAsync(currentProcess.Id);
        Assert.IsNotNull(process);
        Assert.AreEqual(now, process.ConfirmedAt);
        Assert.AreEqual(ControllableProcessStatus.Busy, process.Status);
    }

    [TestMethod]
    public async Task CanGetControllableProcessTasks() {
        await LaunchTashAppIfNotRunning();

        var processTasks = await _Sut.GetControllableProcessTasksAsync();
        Assert.IsNotNull(processTasks);
    }

    [TestMethod]
    public async Task CanPutAndGetControllableProcessTask() {
        await LaunchTashAppIfNotRunning();

        var controllableProcessTask = CreateControllableProcessTask();
        var statusCode = await _Sut.PutControllableProcessTaskAsync(controllableProcessTask);
        Assert.AreEqual(HttpStatusCode.Created, statusCode);
        var processTask = await _Sut.GetControllableProcessTaskAsync(controllableProcessTask.Id);
        Assert.IsNotNull(processTask);
        Assert.AreEqual(controllableProcessTask.ControlName, processTask.ControlName);
    }


    [TestMethod]
    public async Task CanConfirmStatus() {
        await LaunchTashAppIfNotRunning();

        var controllableProcessTask = CreateControllableProcessTask();
        var statusCode = await _Sut.PutControllableProcessTaskAsync(controllableProcessTask);
        Assert.AreEqual(HttpStatusCode.Created, statusCode);

        statusCode = await _Sut.ConfirmStatusAsync(controllableProcessTask.Id, ControllableProcessTaskStatus.BadRequest);
        Assert.AreEqual(HttpStatusCode.NoContent, statusCode);

        var processTask = await _Sut.GetControllableProcessTaskAsync(controllableProcessTask.Id);
        Assert.IsNotNull(processTask);
        Assert.AreEqual(ControllableProcessTaskStatus.BadRequest, processTask.Status);
    }

    [TestMethod]
    public async Task CanConfirmStatusWithTextAndErrorMessage() {
        await LaunchTashAppIfNotRunning();

        var controllableProcessTask = CreateControllableProcessTask();
        var statusCode = await _Sut.PutControllableProcessTaskAsync(controllableProcessTask);
        Assert.AreEqual(HttpStatusCode.Created, statusCode);

        const string text = "This is not a text";
        const string errorMessage = "This is not an error message";
        statusCode = await _Sut.ConfirmStatusAsync(controllableProcessTask.Id, ControllableProcessTaskStatus.BadRequest, text, errorMessage);
        Assert.AreEqual(HttpStatusCode.NoContent, statusCode);

        var processTask = await _Sut.GetControllableProcessTaskAsync(controllableProcessTask.Id);
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
        var statusCode = await _Sut.PutControllableProcessAsync(currentProcess);
        Assert.IsTrue(statusCode == HttpStatusCode.Created || statusCode == HttpStatusCode.NoContent);

        var now = DateTime.Now;
        await _Sut.ConfirmDeadAsync(currentProcess.Id);

        var process = await _Sut.GetControllableProcessAsync(currentProcess.Id);
        Assert.IsNotNull(process);
        Assert.AreEqual(ControllableProcessStatus.Dead, process.Status);
        Assert.IsTrue(process.ConfirmedAt >= now);
    }

    [TestMethod]
    public async Task CanConfirmDeadWhileClosing() {
        await LaunchTashAppIfNotRunning();

        var currentProcess = Process.GetCurrentProcess();
        var statusCode = await _Sut.PutControllableProcessAsync(currentProcess);
        Assert.IsTrue(statusCode == HttpStatusCode.Created || statusCode == HttpStatusCode.NoContent);

        var now = DateTime.Now;
        await _Sut.ConfirmDeadWhileClosingAsync(currentProcess.Id);

        var process = await _Sut.GetControllableProcessAsync(currentProcess.Id);
        Assert.IsNotNull(process);
        Assert.AreEqual(ControllableProcessStatus.Dead, process.Status);
        Assert.IsTrue(process.ConfirmedAt >= now);
    }

    [TestMethod]
    public async Task CanGetOkayToMarkTaskAsCompleted() {
        await LaunchTashAppIfNotRunning();

        var controllableProcessTask = CreateControllableProcessTask();
        var statusCode = await _Sut.PutControllableProcessTaskAsync(controllableProcessTask);
        Assert.AreEqual(HttpStatusCode.Created, statusCode);

        Assert.IsTrue(_Sut.MarkTaskAsCompleted(controllableProcessTask, controllableProcessTask.ProcessId, controllableProcessTask.Type, controllableProcessTask.ControlName, controllableProcessTask.Text));
        Assert.IsFalse(_Sut.MarkTaskAsCompleted(null, controllableProcessTask.ProcessId, controllableProcessTask.Type, controllableProcessTask.ControlName, controllableProcessTask.Text));
        Assert.IsFalse(_Sut.MarkTaskAsCompleted(controllableProcessTask, controllableProcessTask.ProcessId + 1, controllableProcessTask.Type, controllableProcessTask.ControlName, controllableProcessTask.Text));
        Assert.IsFalse(_Sut.MarkTaskAsCompleted(controllableProcessTask, controllableProcessTask.ProcessId, controllableProcessTask.Type, controllableProcessTask.ControlName + "X", controllableProcessTask.Text));
        Assert.IsFalse(_Sut.MarkTaskAsCompleted(controllableProcessTask, controllableProcessTask.ProcessId, controllableProcessTask.Type, controllableProcessTask.ControlName, controllableProcessTask.Text + "X"));
    }

    [TestMethod]
    public async Task CanPickRequestedTask() {
        await LaunchTashAppIfNotRunning();

        var controllableProcessTask = CreateControllableProcessTask();
        controllableProcessTask.Status = ControllableProcessTaskStatus.Requested;
        var statusCode = await _Sut.PutControllableProcessTaskAsync(controllableProcessTask);
        Assert.AreEqual(HttpStatusCode.Created, statusCode);

        var pickedProcessTask = await _Sut.PickRequestedTask(controllableProcessTask.ProcessId);
        Assert.IsNotNull(pickedProcessTask);
        Assert.AreEqual(controllableProcessTask.Id, pickedProcessTask.Id);

        controllableProcessTask.Status = ControllableProcessTaskStatus.Processing;
        statusCode = await _Sut.PutControllableProcessTaskAsync(controllableProcessTask);
        Assert.AreEqual(HttpStatusCode.NoContent, statusCode);

        pickedProcessTask = await _Sut.PickRequestedTask(controllableProcessTask.ProcessId);
        Assert.IsNull(pickedProcessTask);
    }

    [TestMethod]
    public async Task CanAssumeDeath() {
        await LaunchTashAppIfNotRunning();

        var currentProcess = Process.GetCurrentProcess();
        var statusCode = await _Sut.PutControllableProcessAsync(currentProcess);
        Assert.IsTrue(statusCode == HttpStatusCode.Created || statusCode == HttpStatusCode.NoContent);

        await _Sut.AssumeDeath(p => p.ProcessId == currentProcess.Id);

        var process = await _Sut.GetControllableProcessAsync(currentProcess.Id);
        Assert.IsNotNull(process);
        Assert.AreEqual(ControllableProcessStatus.Dead, process.Status);
    }

    [TestMethod]
    public async Task CanFindIdleProcess() {
        await LaunchTashAppIfNotRunning();

        var currentProcess = Process.GetCurrentProcess();
        var statusCode = await _Sut.PutControllableProcessAsync(currentProcess);
        Assert.IsTrue(statusCode == HttpStatusCode.Created || statusCode == HttpStatusCode.NoContent);

        var findIdleProcessResult = await _Sut.FindIdleProcess(p => p.ProcessId == currentProcess.Id);
        Assert.IsNotNull(findIdleProcessResult.ControllableProcess);
        Assert.AreEqual(currentProcess.Id, findIdleProcessResult.ControllableProcess.ProcessId);
    }

    [TestMethod]
    public async Task CanAwaitCompletionAsync() {
        await LaunchTashAppIfNotRunning();

        var controllableProcessTask = CreateControllableProcessTask();
        controllableProcessTask.Status = ControllableProcessTaskStatus.Processing;
        var statusCode = await _Sut.PutControllableProcessTaskAsync(controllableProcessTask);
        Assert.AreEqual(HttpStatusCode.Created, statusCode);

        var now = DateTime.Now;
        var task = await _Sut.AwaitCompletionAsync(controllableProcessTask.Id, 1000);
        var elapsedMilliSeconds = DateTime.Now.Subtract(now).TotalMilliseconds;
        Assert.IsTrue(elapsedMilliSeconds >= 1000);
        Assert.AreEqual(ControllableProcessTaskStatus.Processing, task.Status);

        controllableProcessTask.Status = ControllableProcessTaskStatus.Completed;
        statusCode = await _Sut.PutControllableProcessTaskAsync(controllableProcessTask);
        Assert.AreEqual(HttpStatusCode.NoContent, statusCode);

        now = DateTime.Now;
        task = await _Sut.AwaitCompletionAsync(controllableProcessTask.Id, 1000);
        elapsedMilliSeconds = DateTime.Now.Subtract(now).TotalMilliseconds;
        Assert.IsTrue(elapsedMilliSeconds < 100);
        Assert.AreEqual(ControllableProcessTaskStatus.Completed, task.Status);
    }
}