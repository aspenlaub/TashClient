using Aspenlaub.Net.GitHub.CSharp.Dvin.Components;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Entities;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tash;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Microsoft.OData.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Helpers;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Entities;

// ReSharper disable EmptyGeneralCatchClause

namespace Aspenlaub.Net.GitHub.CSharp.TashClient.Components;

public class TashAccessor : ITashAccessor {
    private const string BaseUrl = "http://localhost:60404", TashAppId = "Tash";
    private const int TimeOutInSeconds = 60;

    protected readonly IDvinRepository DvinRepository;
    private readonly ISimpleLogger _SimpleLogger;
    private readonly bool _DetailedLogging;
    private readonly IMethodNamesFromStackFramesExtractor _MethodNamesFromStackFramesExtractor;

    public TashAccessor(IDvinRepository dvinRepository, ISimpleLogger simpleLogger, ILogConfiguration logConfiguration, IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor) {
        DvinRepository = dvinRepository;
        _SimpleLogger = simpleLogger;
        _DetailedLogging = logConfiguration.DetailedLogging;
        _MethodNamesFromStackFramesExtractor = methodNamesFromStackFramesExtractor;
    }

    public async Task<DvinApp> GetTashAppAsync(IErrorsAndInfos errorsAndInfos) {
        using (_SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(GetTashAppAsync)))) {
            var methodNamesFromStack = _MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            _SimpleLogger.LogInformationWithCallStack("Returning tash app", methodNamesFromStack);
            return await DvinRepository.LoadAsync(TashAppId, errorsAndInfos);
        }
    }

    public async Task<IErrorsAndInfos> EnsureTashAppIsRunningAsync() {
        using (_SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(EnsureTashAppIsRunningAsync)))) {
            var methodNamesFromStack = _MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            _SimpleLogger.LogInformationWithCallStack("Ensuring tash app is running", methodNamesFromStack);
            var errorsAndInfos = new ErrorsAndInfos();
            try {
                var processes = await GetControllableProcessesAsync();
                if (processes != null) {
                    _SimpleLogger.LogInformationWithCallStack("Tash app is running", methodNamesFromStack);
                    return errorsAndInfos;
                }
            } catch (Exception e) {
                LogException("Exception was thrown, tash app probably is not running", e, methodNamesFromStack);
            }
            try {
                var processTasks = await GetControllableProcessTasksAsync();
                if (processTasks != null) {
                    _SimpleLogger.LogInformationWithCallStack("Tash app is running", methodNamesFromStack);
                    return errorsAndInfos;
                }
            } catch (Exception e) {
                LogException("Exception was thrown, tash app probably is not running", e, methodNamesFromStack);
            }

            var tashApp = await GetTashAppAsync(errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                _SimpleLogger.LogInformationWithCallStack("Could not get tash app", methodNamesFromStack);
                return errorsAndInfos;
            }

            var fileSystemService = new FileSystemService();
            tashApp.Start(fileSystemService, errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                _SimpleLogger.LogInformationWithCallStack("Could not start tash app", methodNamesFromStack);
                errorsAndInfos.Errors.ToList().ForEach(e => _SimpleLogger.LogErrorWithCallStack(e, methodNamesFromStack));
                return errorsAndInfos;
            }

            await Task.Delay(TimeSpan.FromSeconds(10));

            try {
                var processes = await GetControllableProcessesAsync();
                if (processes != null) {
                    _SimpleLogger.LogInformationWithCallStack("Tash app is running", methodNamesFromStack);
                    return errorsAndInfos;
                }
            } catch (Exception e) {
                LogException("Tash started but not answering", e, methodNamesFromStack);
            }

            return errorsAndInfos;
        }
    }

    private void LogException(string headline, Exception e, IList<string> methodNamesFromStack) {
        var message = headline + "\r\n" + e.Message;
        if (!string.IsNullOrEmpty(e.StackTrace)) {
            message = message + "\r\n" + e.StackTrace;
        }

        if (e.InnerException != null) {
            message = message + "\r\n" + e.InnerException.Message;
            if (!string.IsNullOrEmpty(e.InnerException.StackTrace)) {
                message = message + "\r\n" + e.InnerException.StackTrace;
            }
        }

        _SimpleLogger.LogInformationWithCallStack(message, methodNamesFromStack);
    }

    public async Task<IList<ControllableProcess>> GetControllableProcessesAsync() {
        using (_SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(GetControllableProcessesAsync)))) {
            var methodNamesFromStack = _MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            _SimpleLogger.LogInformationWithCallStack("Get controllable processes", methodNamesFromStack);
            var context = new DefaultContainer(new Uri(BaseUrl)) { Timeout = TimeOutInSeconds };
            var processes = await context.ControllableProcesses.ExecuteAsync();
            var processList = processes.ToList();
            return processList;
        }
    }

    public async Task<ControllableProcess> GetControllableProcessAsync(int processId) {
        using (_SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(GetControllableProcessAsync)))) {
            var methodNamesFromStack = _MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            _SimpleLogger.LogInformationWithCallStack($"Get controllable process with id={processId}", methodNamesFromStack);
            var context = new DefaultContainer(new Uri(BaseUrl)) { Timeout = TimeOutInSeconds };
            if (!(await ProcessExists(context, processId)).YesNo) {
                _SimpleLogger.LogInformationWithCallStack($"No process found with id={processId}", methodNamesFromStack);
                return null;
            }

            var process = await context.ControllableProcesses.ByKey(processId).GetValueAsync();
            _SimpleLogger.LogInformationWithCallStack($"Returning process with id={processId}", methodNamesFromStack);
            return process;
        }
    }

    public async Task<HttpStatusCode> PutControllableProcessAsync(Process process) {
        using (_SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(PutControllableProcessAsync)))) {
            var methodNamesFromStack = _MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            _SimpleLogger.LogInformationWithCallStack($"Put controllable process with id={process.Id}", methodNamesFromStack);
            var context = new DefaultContainer(new Uri(BaseUrl)) { Timeout = TimeOutInSeconds };
            ControllableProcess controllableProcess;
            if ((await ProcessExists(context, process.Id)).YesNo) {
                _SimpleLogger.LogInformationWithCallStack($"Update controllable process with id={process.Id}", methodNamesFromStack);
                controllableProcess = await context.ControllableProcesses.ByKey(process.Id).GetValueAsync();
                controllableProcess.Title = process.ProcessName;
                controllableProcess.Status = ControllableProcessStatus.Idle;
                controllableProcess.ConfirmedAt = DateTimeOffset.Now;
                controllableProcess.LaunchCommand = process.MainModule?.FileName;
                context.UpdateObject(controllableProcess);
            } else {
                _SimpleLogger.LogInformationWithCallStack($"Insert controllable process with id={process.Id}", methodNamesFromStack);
                controllableProcess = new ControllableProcess {
                    ProcessId = process.Id,
                    Title = process.ProcessName,
                    Status = ControllableProcessStatus.Idle,
                    ConfirmedAt = DateTimeOffset.Now,
                    LaunchCommand = process.MainModule?.FileName
                };
                context.AddToControllableProcesses(controllableProcess);
            }

            var response = await context.SaveChangesAsync(SaveChangesOptions.ReplaceOnUpdate);
            var statusCode = response.Select(r => (HttpStatusCode)r.StatusCode).FirstOrDefault();
            return statusCode;
        }
    }

    public async Task<HttpStatusCode> ConfirmAliveAsync(int processId, DateTime now, ControllableProcessStatus status) {
        using (_SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(ConfirmAliveAsync)))) {
            var methodNamesFromStack = _MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            _SimpleLogger.LogInformationWithCallStack($"Confirm that process with id={processId} is alive", methodNamesFromStack);
            var context = new DefaultContainer(new Uri(BaseUrl)) { Timeout = TimeOutInSeconds };
            var processExists = await ProcessExists(context, processId);
            if (processExists.Inconclusive) {
                _SimpleLogger.LogInformationWithCallStack($"Could not determine if process with id={processId} exists", methodNamesFromStack);
                return HttpStatusCode.InternalServerError;
            }

            if (!processExists.YesNo) {
                _SimpleLogger.LogInformationWithCallStack($"No process exists with id={processId}", methodNamesFromStack);
                return HttpStatusCode.NotFound;
            }

            _SimpleLogger.LogInformationWithCallStack($"Update process with id={processId}", methodNamesFromStack);
            var controllableProcess = await context.ControllableProcesses.ByKey(processId).GetValueAsync();
            controllableProcess.ConfirmedAt = now;
            controllableProcess.Status = status;
            context.UpdateObject(controllableProcess);
            try {
                var response = await context.SaveChangesAsync(SaveChangesOptions.None);
                var statusCode = response.Select(r => (HttpStatusCode)r.StatusCode).FirstOrDefault();
                return statusCode;
            } catch (Exception e) {
                LogException("Exception was thrown while saving changes", e, methodNamesFromStack);
                return HttpStatusCode.InternalServerError;
            }
        }
    }

    public async Task<HttpStatusCode> ConfirmDeadAsync(int processId) {
        using (_SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(ConfirmDeadAsync)))) {
            var methodNamesFromStack = _MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            _SimpleLogger.LogInformationWithCallStack($"Confirm that process with id={processId} is dead", methodNamesFromStack);
            return await ConfirmAliveAsync(processId, DateTime.Now, ControllableProcessStatus.Dead);
        }
    }

    public async Task ConfirmDeadWhileClosingAsync(int processId) {
        await ConfirmDeadAsync(processId);
    }

    public async Task<IList<ControllableProcessTask>> GetControllableProcessTasksAsync() {
        using (_SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(GetControllableProcessTasksAsync)))) {
            var methodNamesFromStack = _MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            _SimpleLogger.LogInformationWithCallStack("Get controllable process tasks", methodNamesFromStack);
            var context = new DefaultContainer(new Uri(BaseUrl)) { Timeout = TimeOutInSeconds };
            var processTasks = await context.ControllableProcessTasks.ExecuteAsync();
            var processList = processTasks.ToList();
            return processList;
        }
    }

    public async Task<ControllableProcessTask> GetControllableProcessTaskAsync(Guid taskId) {
        using (_SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(GetControllableProcessTaskAsync)))) {
            var methodNamesFromStack = _MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            if (_DetailedLogging) {
                _SimpleLogger.LogInformationWithCallStack($"Get controllable process task with id={taskId}", methodNamesFromStack);
            }
            var context = new DefaultContainer(new Uri(BaseUrl)) { Timeout = TimeOutInSeconds };
            if (!await ProcessTaskExists(context, taskId)) {
                _SimpleLogger.LogInformationWithCallStack($"No controllable process task found with id={taskId}", methodNamesFromStack);
                return null;
            }

            if (_DetailedLogging) {
                _SimpleLogger.LogInformationWithCallStack($"Returning controllable process task with id={taskId}", methodNamesFromStack);
            }
            var processTask = await context.ControllableProcessTasks.ByKey(taskId).GetValueAsync();
            return processTask;
        }
    }

    public async Task<HttpStatusCode> PutControllableProcessTaskAsync(ControllableProcessTask processTask) {
        using (_SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(PutControllableProcessTaskAsync)))) {
            var methodNamesFromStack = _MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            _SimpleLogger.LogInformationWithCallStack($"Put controllable process task with id={processTask.Id}", methodNamesFromStack);
            var context = new DefaultContainer(new Uri(BaseUrl)) { Timeout = TimeOutInSeconds };
            ControllableProcessTask controllableProcessTask;
            if (await ProcessTaskExists(context, processTask.Id)) {
                _SimpleLogger.LogInformationWithCallStack($"Update controllable process task with id={processTask.Id}", methodNamesFromStack);
                controllableProcessTask = await context.ControllableProcessTasks.ByKey(processTask.Id).GetValueAsync();
                controllableProcessTask.ProcessId = processTask.ProcessId;
                controllableProcessTask.Type = processTask.Type;
                controllableProcessTask.ControlName = processTask.ControlName;
                controllableProcessTask.Text = processTask.Text;
                controllableProcessTask.Status = processTask.Status;
                context.UpdateObject(controllableProcessTask);
            }
            else {
                _SimpleLogger.LogInformationWithCallStack($"Insert controllable process task with id={processTask.Id}", methodNamesFromStack);
                controllableProcessTask = new ControllableProcessTask {
                    Id = processTask.Id,
                    ProcessId = processTask.ProcessId,
                    Type = processTask.Type,
                    ControlName = processTask.ControlName,
                    Text = processTask.Text,
                    Status = processTask.Status
                };
                context.AddToControllableProcessTasks(controllableProcessTask);
            }

            var response = await context.SaveChangesAsync(SaveChangesOptions.ReplaceOnUpdate);
            var statusCode = response.Select(r => (HttpStatusCode) r.StatusCode).FirstOrDefault();
            return statusCode;
        }
    }

    public async Task<HttpStatusCode> ConfirmStatusAsync(Guid taskId, ControllableProcessTaskStatus status) {
        return await ConfirmStatusAsync(taskId, status, null, null);
    }

    public async Task<HttpStatusCode> ConfirmStatusAsync(Guid taskId, ControllableProcessTaskStatus status, string text, string errorMessage) {
        using (_SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(ConfirmStatusAsync)))) {
            var methodNamesFromStack = _MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            _SimpleLogger.LogInformationWithCallStack($"Confirm status {Enum.GetName(typeof(ControllableProcessStatus), status)} for task id={taskId}", methodNamesFromStack);

            DefaultContainer context;
            ControllableProcessTask controllableProcessTask = null;
            bool wasExceptionThrown;
            do {
                wasExceptionThrown = false;
                context = new DefaultContainer(new Uri(BaseUrl)) { Timeout = TimeOutInSeconds };
                try {
                    if (!await ProcessTaskExists(context, taskId)) {
                        return HttpStatusCode.NotFound;
                    }
                    _SimpleLogger.LogInformationWithCallStack($"Select task with id={taskId} for update", methodNamesFromStack);
                    controllableProcessTask = await context.ControllableProcessTasks.ByKey(taskId).GetValueAsync();
                } catch (Exception e) {
                    LogException($"Could not select task with id={taskId} for update, trying again", e, methodNamesFromStack);
                    wasExceptionThrown = true;
                }
            } while (wasExceptionThrown);

            if (controllableProcessTask == null) {
                _SimpleLogger.LogInformationWithCallStack($"No task found with id={taskId}", methodNamesFromStack);
                return HttpStatusCode.NotFound;
            }

            _SimpleLogger.LogInformationWithCallStack($"Update task with id={taskId}", methodNamesFromStack);
            controllableProcessTask.Status = status;
            controllableProcessTask.Text = text;
            controllableProcessTask.ErrorMessage = errorMessage;
            context.UpdateObject(controllableProcessTask);
            var response = await context.SaveChangesAsync(SaveChangesOptions.None);
            var statusCode = response.Select(r => (HttpStatusCode)r.StatusCode).FirstOrDefault();
            return statusCode;
        }
    }

    private async Task<YesNoInconclusive> ProcessExists(DefaultContainer context, int processId) {
        var query = (DataServiceQuery<ControllableProcess>)context.ControllableProcesses.Where(p => p.ProcessId == processId || p.ProcessId == -4711); // Hack, hack, hack
        try {
            var controllableProcesses = await query.ExecuteAsync();
            return controllableProcesses.Any() ? new YesNoInconclusive { YesNo = true } : new YesNoInconclusive { YesNo = false };
        } catch {
            return new YesNoInconclusive { Inconclusive = true, YesNo = false };
        }
    }

    private async Task<bool> ProcessTaskExists(DefaultContainer context, Guid taskId) {
        var query = (DataServiceQuery<ControllableProcessTask>)context.ControllableProcessTasks.Where(p => p.Id == taskId || p.Id == Guid.NewGuid()); // Hack, hack, hack
        var controllableProcessTasks = await query.ExecuteAsync();
        return controllableProcessTasks.Any();
    }

    public bool MarkTaskAsCompleted(ControllableProcessTask theTaskIAmProcessing, int processId, string type, string controlName, string text) {
        return theTaskIAmProcessing != null
               && theTaskIAmProcessing.Status == ControllableProcessTaskStatus.Processing
               && theTaskIAmProcessing.ProcessId == processId
               && theTaskIAmProcessing.Type == type
               && theTaskIAmProcessing.ControlName == controlName
               && theTaskIAmProcessing.Text == text;
    }

    public async Task<ControllableProcessTask> PickRequestedTask(int processId) {
        using (_SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(PickRequestedTask)))) {
            var methodNamesFromStack = _MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            _SimpleLogger.LogInformationWithCallStack($"Get requested task for process with id={processId}", methodNamesFromStack);
            var tasks = await GetControllableProcessTasksAsync();
            return tasks.FirstOrDefault(t => t.ProcessId == processId && t.Status == ControllableProcessTaskStatus.Requested);
        }
    }

    public async Task AssumeDeath(Func<ControllableProcess, bool> condition) {
        var processes = await GetControllableProcessesAsync();
        foreach (var process in processes.Where(p => condition(p) && p.Status != ControllableProcessStatus.Dead)) {
            await ConfirmAliveAsync(process.ProcessId, DateTime.Now, ControllableProcessStatus.Dead);
        }
    }

    public async Task<IFindIdleProcessResult> FindIdleProcess(Func<ControllableProcess, bool> condition) {
        var processes = (await GetControllableProcessesAsync()).Where(p => condition(p)).ToList();
        if (!processes.Any()) { return new FindIdleProcessResult { BestProcessStatus = ControllableProcessStatus.DoesNotExist }; }

        IFindIdleProcessResult result = new FindIdleProcessResult();

        var process = processes.Where(p => p.Status == ControllableProcessStatus.Idle).MaxBy(p => p.ConfirmedAt);
        if (process != null) {
            result.ControllableProcess = process;
            result.BestProcessStatus = ControllableProcessStatus.Idle;
            return result;
        }

        var nonIdleProcess = processes.FirstOrDefault(p => p.Status == ControllableProcessStatus.Busy);
        result.BestProcessStatus = nonIdleProcess == null ? ControllableProcessStatus.Dead : ControllableProcessStatus.Busy;
        return result;
    }

    public async Task<ControllableProcessTask> AwaitCompletionAsync(Guid taskId, int milliSecondsToAttemptWhileRequestedOrProcessing) {
        const int internalInMilliSeconds = 100;
        ControllableProcessTask task;

        using (_SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(AwaitCompletionAsync)))) {
            var methodNamesFromStack = _MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            _SimpleLogger.LogInformationWithCallStack($"Awaiting completion of task with id={taskId}", methodNamesFromStack);
            do {
                await Wait.UntilAsync(async () => {
                    try {
                        task = await GetControllableProcessTaskAsync(taskId);
                    } catch {
                        task = null;
                    }
                    return task?.Status == ControllableProcessTaskStatus.Completed;
                }, TimeSpan.FromMilliseconds(internalInMilliSeconds));

                try {
                    task = await GetControllableProcessTaskAsync(taskId);
                } catch {
                    task = null;
                }
                if (task != null) {
                    if (task.Status == ControllableProcessTaskStatus.Completed) {
                        _SimpleLogger.LogInformationWithCallStack($"Task with id={taskId} is complete", methodNamesFromStack);
                        return task;
                    }

                    var process = await GetControllableProcessAsync(task.ProcessId);
                    if (process?.Status == ControllableProcessStatus.Dead) {
                        _SimpleLogger.LogInformationWithCallStack($"Process with id={task.ProcessId} is dead for task with id={taskId}", methodNamesFromStack);
                        return task;
                    }
                }

                milliSecondsToAttemptWhileRequestedOrProcessing -= internalInMilliSeconds;
            } while (0 < milliSecondsToAttemptWhileRequestedOrProcessing && (task == null || task.Status == ControllableProcessTaskStatus.Processing || task.Status == ControllableProcessTaskStatus.Requested));

            _SimpleLogger.LogInformationWithCallStack($"Returning incomplete task with id={taskId}", methodNamesFromStack);
            return task;
        }
    }
}