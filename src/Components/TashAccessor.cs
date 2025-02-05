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
using System.Net.Http;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Helpers;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Entities;

// ReSharper disable EmptyGeneralCatchClause

namespace Aspenlaub.Net.GitHub.CSharp.TashClient.Components;

public class TashAccessor(IDvinRepository dvinRepository, ISimpleLogger simpleLogger,
        ILogConfiguration logConfiguration,
        IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor) : ITashAccessor {
    private const string _baseUrl = "http://localhost:60404", _tashAppId = "Tash";

    protected readonly IDvinRepository DvinRepository = dvinRepository;
    private readonly bool _DetailedLogging = logConfiguration.DetailedLogging;

    private readonly IHttpClientFactory _HttpClientFactory = new HttpClientFactory();

    public async Task<DvinApp> GetTashAppAsync(IErrorsAndInfos errorsAndInfos) {
        using (simpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(GetTashAppAsync)))) {
            IList<string> methodNamesFromStack = methodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            simpleLogger.LogInformationWithCallStack("Returning tash app", methodNamesFromStack);
            return await DvinRepository.LoadAsync(_tashAppId, errorsAndInfos);
        }
    }

    public async Task<IErrorsAndInfos> EnsureTashAppIsRunningAsync() {
        using (simpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(EnsureTashAppIsRunningAsync)))) {
            IList<string> methodNamesFromStack = methodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            simpleLogger.LogInformationWithCallStack("Ensuring tash app is running", methodNamesFromStack);
            var errorsAndInfos = new ErrorsAndInfos();
            try {
                IList<ControllableProcess> processes = await GetControllableProcessesAsync();
                if (processes != null) {
                    simpleLogger.LogInformationWithCallStack("Tash app is running", methodNamesFromStack);
                    return errorsAndInfos;
                }
            } catch (Exception e) {
                LogException("Exception was thrown, tash app probably is not running", e, methodNamesFromStack);
            }
            try {
                IList<ControllableProcessTask> processTasks = await GetControllableProcessTasksAsync();
                if (processTasks != null) {
                    simpleLogger.LogInformationWithCallStack("Tash app is running", methodNamesFromStack);
                    return errorsAndInfos;
                }
            } catch (Exception e) {
                LogException("Exception was thrown, tash app probably is not running", e, methodNamesFromStack);
            }

            DvinApp tashApp = await GetTashAppAsync(errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                simpleLogger.LogInformationWithCallStack("Could not get tash app", methodNamesFromStack);
                return errorsAndInfos;
            }

            var fileSystemService = new FileSystemService();
            tashApp.Start(fileSystemService, errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                simpleLogger.LogInformationWithCallStack("Could not start tash app", methodNamesFromStack);
                errorsAndInfos.Errors.ToList().ForEach(e => simpleLogger.LogErrorWithCallStack(e, methodNamesFromStack));
                return errorsAndInfos;
            }

            await Task.Delay(TimeSpan.FromSeconds(10));

            try {
                IList<ControllableProcess> processes = await GetControllableProcessesAsync();
                if (processes != null) {
                    simpleLogger.LogInformationWithCallStack("Tash app is running", methodNamesFromStack);
                    return errorsAndInfos;
                }
            } catch (Exception e) {
                LogException("Tash started but not answering", e, methodNamesFromStack);
            }

            return errorsAndInfos;
        }
    }

    private void LogException(string headline, Exception e, IList<string> methodNamesFromStack) {
        string message = headline + "\r\n" + e.Message;
        if (!string.IsNullOrEmpty(e.StackTrace)) {
            message = message + "\r\n" + e.StackTrace;
        }

        if (e.InnerException != null) {
            message = message + "\r\n" + e.InnerException.Message;
            if (!string.IsNullOrEmpty(e.InnerException.StackTrace)) {
                message = message + "\r\n" + e.InnerException.StackTrace;
            }
        }

        simpleLogger.LogInformationWithCallStack(message, methodNamesFromStack);
    }

    public async Task<IList<ControllableProcess>> GetControllableProcessesAsync() {
        using (simpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(GetControllableProcessesAsync)))) {
            IList<string> methodNamesFromStack = methodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            simpleLogger.LogInformationWithCallStack("Get controllable processes", methodNamesFromStack);
            var context = new DefaultContainer(new Uri(_baseUrl)) { HttpClientFactory = _HttpClientFactory };
            IEnumerable<ControllableProcess> processes = await context.ControllableProcesses.ExecuteAsync();
            var processList = processes.ToList();
            return processList;
        }
    }

    public async Task<ControllableProcess> GetControllableProcessAsync(int processId) {
        using (simpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(GetControllableProcessAsync)))) {
            IList<string> methodNamesFromStack = methodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            simpleLogger.LogInformationWithCallStack($"Get controllable process with id={processId}", methodNamesFromStack);
            var context = new DefaultContainer(new Uri(_baseUrl)) { HttpClientFactory = _HttpClientFactory };
            if (!(await ProcessExists(context, processId)).YesNo) {
                simpleLogger.LogInformationWithCallStack($"No process found with id={processId}", methodNamesFromStack);
                return null;
            }

            ControllableProcess process = await context.ControllableProcesses.ByKey(processId).GetValueAsync();
            simpleLogger.LogInformationWithCallStack($"Returning process with id={processId}", methodNamesFromStack);
            return process;
        }
    }

    public async Task<HttpStatusCode> PutControllableProcessAsync(Process process) {
        using (simpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(PutControllableProcessAsync)))) {
            IList<string> methodNamesFromStack = methodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            simpleLogger.LogInformationWithCallStack($"Put controllable process with id={process.Id}", methodNamesFromStack);
            var context = new DefaultContainer(new Uri(_baseUrl)) { HttpClientFactory = _HttpClientFactory };
            ControllableProcess controllableProcess;
            if ((await ProcessExists(context, process.Id)).YesNo) {
                simpleLogger.LogInformationWithCallStack($"Update controllable process with id={process.Id}", methodNamesFromStack);
                controllableProcess = await context.ControllableProcesses.ByKey(process.Id).GetValueAsync();
                controllableProcess.Title = process.ProcessName;
                controllableProcess.Status = ControllableProcessStatus.Idle;
                controllableProcess.ConfirmedAt = DateTimeOffset.Now;
                controllableProcess.LaunchCommand = process.MainModule?.FileName;
                context.UpdateObject(controllableProcess);
            } else {
                simpleLogger.LogInformationWithCallStack($"Insert controllable process with id={process.Id}", methodNamesFromStack);
                controllableProcess = new ControllableProcess {
                    ProcessId = process.Id,
                    Title = process.ProcessName,
                    Status = ControllableProcessStatus.Idle,
                    ConfirmedAt = DateTimeOffset.Now,
                    LaunchCommand = process.MainModule?.FileName
                };
                context.AddToControllableProcesses(controllableProcess);
            }

            DataServiceResponse response = await context.SaveChangesAsync(SaveChangesOptions.ReplaceOnUpdate);
            HttpStatusCode statusCode = response.Select(r => (HttpStatusCode)r.StatusCode).FirstOrDefault();
            return statusCode;
        }
    }

    public async Task<HttpStatusCode> ConfirmAliveAsync(int processId, DateTime now, ControllableProcessStatus status) {
        using (simpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(ConfirmAliveAsync)))) {
            IList<string> methodNamesFromStack = methodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            simpleLogger.LogInformationWithCallStack($"Confirm that process with id={processId} is alive", methodNamesFromStack);
            var context = new DefaultContainer(new Uri(_baseUrl)) { HttpClientFactory = _HttpClientFactory };
            YesNoInconclusive processExists = await ProcessExists(context, processId);
            if (processExists.Inconclusive) {
                simpleLogger.LogInformationWithCallStack($"Could not determine if process with id={processId} exists", methodNamesFromStack);
                return HttpStatusCode.InternalServerError;
            }

            if (!processExists.YesNo) {
                simpleLogger.LogInformationWithCallStack($"No process exists with id={processId}", methodNamesFromStack);
                return HttpStatusCode.NotFound;
            }

            simpleLogger.LogInformationWithCallStack($"Update process with id={processId}", methodNamesFromStack);
            ControllableProcess controllableProcess = await context.ControllableProcesses.ByKey(processId).GetValueAsync();
            controllableProcess.ConfirmedAt = now;
            controllableProcess.Status = status;
            context.UpdateObject(controllableProcess);
            try {
                DataServiceResponse response = await context.SaveChangesAsync(SaveChangesOptions.None);
                HttpStatusCode statusCode = response.Select(r => (HttpStatusCode)r.StatusCode).FirstOrDefault();
                return statusCode;
            } catch (Exception e) {
                LogException("Exception was thrown while saving changes", e, methodNamesFromStack);
                return HttpStatusCode.InternalServerError;
            }
        }
    }

    public async Task<HttpStatusCode> ConfirmDeadAsync(int processId) {
        using (simpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(ConfirmDeadAsync)))) {
            IList<string> methodNamesFromStack = methodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            simpleLogger.LogInformationWithCallStack($"Confirm that process with id={processId} is dead", methodNamesFromStack);
            return await ConfirmAliveAsync(processId, DateTime.Now, ControllableProcessStatus.Dead);
        }
    }

    public async Task ConfirmDeadWhileClosingAsync(int processId) {
        await ConfirmDeadAsync(processId);
    }

    public async Task<IList<ControllableProcessTask>> GetControllableProcessTasksAsync() {
        using (simpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(GetControllableProcessTasksAsync)))) {
            IList<string> methodNamesFromStack = methodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            simpleLogger.LogInformationWithCallStack("Get controllable process tasks", methodNamesFromStack);
            var context = new DefaultContainer(new Uri(_baseUrl)) { HttpClientFactory = _HttpClientFactory };
            IEnumerable<ControllableProcessTask> processTasks = await context.ControllableProcessTasks.ExecuteAsync();
            var processList = processTasks.ToList();
            return processList;
        }
    }

    public async Task<ControllableProcessTask> GetControllableProcessTaskAsync(Guid taskId) {
        using (simpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(GetControllableProcessTaskAsync)))) {
            IList<string> methodNamesFromStack = methodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            if (_DetailedLogging) {
                simpleLogger.LogInformationWithCallStack($"Get controllable process task with id={taskId}", methodNamesFromStack);
            }
            var context = new DefaultContainer(new Uri(_baseUrl)) { HttpClientFactory = _HttpClientFactory };
            if (!await ProcessTaskExists(context, taskId)) {
                simpleLogger.LogInformationWithCallStack($"No controllable process task found with id={taskId}", methodNamesFromStack);
                return null;
            }

            if (_DetailedLogging) {
                simpleLogger.LogInformationWithCallStack($"Returning controllable process task with id={taskId}", methodNamesFromStack);
            }
            ControllableProcessTask processTask = await context.ControllableProcessTasks.ByKey(taskId).GetValueAsync();
            return processTask;
        }
    }

    public async Task<HttpStatusCode> PutControllableProcessTaskAsync(ControllableProcessTask processTask) {
        using (simpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(PutControllableProcessTaskAsync)))) {
            IList<string> methodNamesFromStack = methodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            simpleLogger.LogInformationWithCallStack($"Put controllable process task with id={processTask.Id}", methodNamesFromStack);
            var context = new DefaultContainer(new Uri(_baseUrl)) { HttpClientFactory = _HttpClientFactory };
            ControllableProcessTask controllableProcessTask;
            if (await ProcessTaskExists(context, processTask.Id)) {
                simpleLogger.LogInformationWithCallStack($"Update controllable process task with id={processTask.Id}", methodNamesFromStack);
                controllableProcessTask = await context.ControllableProcessTasks.ByKey(processTask.Id).GetValueAsync();
                controllableProcessTask.ProcessId = processTask.ProcessId;
                controllableProcessTask.Type = processTask.Type;
                controllableProcessTask.ControlName = processTask.ControlName;
                controllableProcessTask.Text = processTask.Text;
                controllableProcessTask.Status = processTask.Status;
                context.UpdateObject(controllableProcessTask);
            }
            else {
                simpleLogger.LogInformationWithCallStack($"Insert controllable process task with id={processTask.Id}", methodNamesFromStack);
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

            DataServiceResponse response = await context.SaveChangesAsync(SaveChangesOptions.ReplaceOnUpdate);
            HttpStatusCode statusCode = response.Select(r => (HttpStatusCode) r.StatusCode).FirstOrDefault();
            return statusCode;
        }
    }

    public async Task<HttpStatusCode> ConfirmStatusAsync(Guid taskId, ControllableProcessTaskStatus status) {
        return await ConfirmStatusAsync(taskId, status, null, null);
    }

    public async Task<HttpStatusCode> ConfirmStatusAsync(Guid taskId, ControllableProcessTaskStatus status, string text, string errorMessage) {
        using (simpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(ConfirmStatusAsync)))) {
            IList<string> methodNamesFromStack = methodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            simpleLogger.LogInformationWithCallStack($"Confirm status {Enum.GetName(typeof(ControllableProcessStatus), status)} for task id={taskId}", methodNamesFromStack);

            DefaultContainer context;
            ControllableProcessTask controllableProcessTask = null;
            bool wasExceptionThrown;
            do {
                wasExceptionThrown = false;
                context = new DefaultContainer(new Uri(_baseUrl)) { HttpClientFactory = _HttpClientFactory };
                try {
                    if (!await ProcessTaskExists(context, taskId)) {
                        return HttpStatusCode.NotFound;
                    }
                    simpleLogger.LogInformationWithCallStack($"Select task with id={taskId} for update", methodNamesFromStack);
                    controllableProcessTask = await context.ControllableProcessTasks.ByKey(taskId).GetValueAsync();
                } catch (Exception e) {
                    LogException($"Could not select task with id={taskId} for update, trying again", e, methodNamesFromStack);
                    wasExceptionThrown = true;
                }
            } while (wasExceptionThrown);

            if (controllableProcessTask == null) {
                simpleLogger.LogInformationWithCallStack($"No task found with id={taskId}", methodNamesFromStack);
                return HttpStatusCode.NotFound;
            }

            simpleLogger.LogInformationWithCallStack($"Update task with id={taskId}", methodNamesFromStack);
            controllableProcessTask.Status = status;
            controllableProcessTask.Text = text;
            controllableProcessTask.ErrorMessage = errorMessage;
            context.UpdateObject(controllableProcessTask);
            DataServiceResponse response = await context.SaveChangesAsync(SaveChangesOptions.None);
            HttpStatusCode statusCode = response.Select(r => (HttpStatusCode)r.StatusCode).FirstOrDefault();
            return statusCode;
        }
    }

    private static async Task<YesNoInconclusive> ProcessExists(DefaultContainer context, int processId) {
        var query = (DataServiceQuery<ControllableProcess>)context.ControllableProcesses.Where(p => p.ProcessId == processId || p.ProcessId == -4711); // Hack, hack, hack
        try {
            IEnumerable<ControllableProcess> controllableProcesses = await query.ExecuteAsync();
            return controllableProcesses.Any() ? new YesNoInconclusive { YesNo = true } : new YesNoInconclusive { YesNo = false };
        } catch {
            return new YesNoInconclusive { Inconclusive = true, YesNo = false };
        }
    }

    private static async Task<bool> ProcessTaskExists(DefaultContainer context, Guid taskId) {
        var query = (DataServiceQuery<ControllableProcessTask>)context.ControllableProcessTasks.Where(p => p.Id == taskId || p.Id == Guid.NewGuid()); // Hack, hack, hack
        IEnumerable<ControllableProcessTask> controllableProcessTasks = await query.ExecuteAsync();
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
        using (simpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(PickRequestedTask)))) {
            IList<string> methodNamesFromStack = methodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            simpleLogger.LogInformationWithCallStack($"Get requested task for process with id={processId}", methodNamesFromStack);
            IList<ControllableProcessTask> tasks = await GetControllableProcessTasksAsync();
            return tasks.FirstOrDefault(t => t.ProcessId == processId && t.Status == ControllableProcessTaskStatus.Requested);
        }
    }

    public async Task AssumeDeath(Func<ControllableProcess, bool> condition) {
        IList<ControllableProcess> processes = await GetControllableProcessesAsync();
        foreach (ControllableProcess process in processes.Where(p => condition(p) && p.Status != ControllableProcessStatus.Dead)) {
            await ConfirmAliveAsync(process.ProcessId, DateTime.Now, ControllableProcessStatus.Dead);
        }
    }

    public async Task<IFindIdleProcessResult> FindIdleProcess(Func<ControllableProcess, bool> condition) {
        var processes = (await GetControllableProcessesAsync()).Where(p => condition(p)).ToList();
        if (processes.Count == 0) { return new FindIdleProcessResult { BestProcessStatus = ControllableProcessStatus.DoesNotExist }; }

        IFindIdleProcessResult result = new FindIdleProcessResult();

        ControllableProcess process = processes.Where(p => p.Status == ControllableProcessStatus.Idle).MaxBy(p => p.ConfirmedAt);
        if (process != null) {
            result.ControllableProcess = process;
            result.BestProcessStatus = ControllableProcessStatus.Idle;
            return result;
        }

        ControllableProcess nonIdleProcess = processes.FirstOrDefault(p => p.Status == ControllableProcessStatus.Busy);
        result.BestProcessStatus = nonIdleProcess == null ? ControllableProcessStatus.Dead : ControllableProcessStatus.Busy;
        return result;
    }

    public async Task<ControllableProcessTask> AwaitCompletionAsync(Guid taskId, int milliSecondsToAttemptWhileRequestedOrProcessing) {
        const int internalInMilliSeconds = 100;
        ControllableProcessTask task;

        using (simpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(AwaitCompletionAsync)))) {
            IList<string> methodNamesFromStack = methodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            simpleLogger.LogInformationWithCallStack($"Awaiting completion of task with id={taskId}", methodNamesFromStack);
            var tryUntil = DateTime.Now.AddMilliseconds(milliSecondsToAttemptWhileRequestedOrProcessing);
            do {
                simpleLogger.LogInformationWithCallStack($"Wait until {tryUntil:HH:mm:ss} for completion of task with id={taskId}", methodNamesFromStack);
                int waitCounter = 0;
                await Wait.UntilAsync(async () => {
                    if (waitCounter ++ % 10 == 0) {
                        simpleLogger.LogInformationWithCallStack("Checking for completion", methodNamesFromStack);
                    }
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

                if (task == null) {
                    continue;
                }

                if (task.Status == ControllableProcessTaskStatus.Completed) {
                    simpleLogger.LogInformationWithCallStack($"Task with id={taskId} is complete", methodNamesFromStack);
                    return task;
                }

                ControllableProcess process = await GetControllableProcessAsync(task.ProcessId);
                if (process?.Status != ControllableProcessStatus.Dead) {
                    continue;
                }

                simpleLogger.LogInformationWithCallStack($"Process with id={task.ProcessId} is dead for task with id={taskId}", methodNamesFromStack);
                return task;
            } while (DateTime.Now < tryUntil
                     && (task == null || task.Status == ControllableProcessTaskStatus.Processing || task.Status == ControllableProcessTaskStatus.Requested));

            simpleLogger.LogInformationWithCallStack($"Returning incomplete task with id={taskId}", methodNamesFromStack);
            return task;
        }
    }
}