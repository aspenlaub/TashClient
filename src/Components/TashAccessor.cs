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
using Aspenlaub.Net.GitHub.CSharp.Pegh.Helpers;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Entities;
using Microsoft.Extensions.Logging;
// ReSharper disable EmptyGeneralCatchClause

namespace Aspenlaub.Net.GitHub.CSharp.TashClient.Components {
    public class TashAccessor : ITashAccessor {
        private const string BaseUrl = "http://localhost:60404", TashAppId = "Tash";

        protected readonly IDvinRepository DvinRepository;
        private readonly ISimpleLogger vSimpleLogger;
        private readonly string vLogId;
        private readonly bool vDetailedLogging;

        public TashAccessor(IDvinRepository dvinRepository, ISimpleLogger simpleLogger, ILogConfiguration logConfiguration) {
            DvinRepository = dvinRepository;
            vSimpleLogger = simpleLogger;
            vSimpleLogger.LogSubFolder = logConfiguration.LogSubFolder;
            vLogId = logConfiguration.LogId;
            vDetailedLogging = logConfiguration.DetailedLogging;
        }

        public async Task<DvinApp> GetTashAppAsync(IErrorsAndInfos errorsAndInfos) {
            using (vSimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(TashAccessor), vLogId))) {
                vSimpleLogger.LogInformation("Returning tash app");
                return await DvinRepository.LoadAsync(TashAppId, errorsAndInfos);
            }
        }

        public async Task<IErrorsAndInfos> EnsureTashAppIsRunningAsync() {
            using (vSimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(TashAccessor), vLogId))) {
                vSimpleLogger.LogInformation("Ensuring tash app is running");
                var errorsAndInfos = new ErrorsAndInfos();
                try {
                    var processes = await GetControllableProcessesAsync();
                    if (processes != null) {
                        vSimpleLogger.LogInformation("Tash app is running");
                        return errorsAndInfos;
                    }
                } catch {
                    vSimpleLogger.LogInformation("Exception was thrown, tash app probably is not running");
                }

                var tashApp = await GetTashAppAsync(errorsAndInfos);
                if (errorsAndInfos.AnyErrors()) {
                    vSimpleLogger.LogInformation("Could not get tash app");
                    return errorsAndInfos;
                }

                var fileSystemService = new FileSystemService();
                tashApp.Start(fileSystemService, errorsAndInfos);
                if (errorsAndInfos.AnyErrors()) {
                    vSimpleLogger.LogInformation("Could not start tash app");
                    errorsAndInfos.Errors.ToList().ForEach(e => vSimpleLogger.LogError(e));
                    return errorsAndInfos;
                }

                await Task.Delay(TimeSpan.FromSeconds(10));

                try {
                    var processes = await GetControllableProcessesAsync();
                    if (processes != null) {
                        vSimpleLogger.LogInformation("Tash app is running");
                        return errorsAndInfos;
                    }
                } catch {
                    const string errorMessage = "Tash started but not answering";
                    errorsAndInfos.Errors.Add(errorMessage); // Should this occur regularly, maybe the Tash process can be killed
                    vSimpleLogger.LogError(errorMessage);
                }

                return errorsAndInfos;
            }
        }

        public async Task<IList<ControllableProcess>> GetControllableProcessesAsync() {
            using (vSimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(TashAccessor), vLogId))) {
                vSimpleLogger.LogInformation("Get controllable processes");
                var context = new DefaultContainer(new Uri(BaseUrl));
                var processes = await context.ControllableProcesses.ExecuteAsync();
                var processList = processes.ToList();
                return processList;
            }
        }

        public async Task<ControllableProcess> GetControllableProcessAsync(int processId) {
            using (vSimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(TashAccessor), vLogId))) {
                vSimpleLogger.LogInformation($"Get controllable process with id={processId}");
                var context = new DefaultContainer(new Uri(BaseUrl));
                if (!(await ProcessExists(context, processId)).YesNo) {
                    vSimpleLogger.LogInformation($"No process found with id={processId}");
                    return null;
                }

                var process = await context.ControllableProcesses.ByKey(processId).GetValueAsync();
                vSimpleLogger.LogInformation($"Returning process with id={processId}");
                return process;
            }
        }

        public async Task<HttpStatusCode> PutControllableProcessAsync(Process process) {
            using (vSimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(TashAccessor), vLogId))) {
                vSimpleLogger.LogInformation($"Put controllable process with id={process.Id}");
                var context = new DefaultContainer(new Uri(BaseUrl));
                ControllableProcess controllableProcess;
                if ((await ProcessExists(context, process.Id)).YesNo) {
                    vSimpleLogger.LogInformation($"Update controllable process with id={process.Id}");
                    controllableProcess = await context.ControllableProcesses.ByKey(process.Id).GetValueAsync();
                    controllableProcess.Title = process.ProcessName;
                    controllableProcess.Status = ControllableProcessStatus.Idle;
                    controllableProcess.ConfirmedAt = DateTimeOffset.Now;
                    controllableProcess.LaunchCommand = process.MainModule?.FileName;
                    context.UpdateObject(controllableProcess);
                } else {
                    vSimpleLogger.LogInformation($"Insert controllable process with id={process.Id}");
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
            using (vSimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(TashAccessor), vLogId))) {
                vSimpleLogger.LogInformation($"Confirm that process with id={processId} is alive");
                var context = new DefaultContainer(new Uri(BaseUrl));
                var processExists = await ProcessExists(context, processId);
                if (processExists.Inconclusive) {
                    vSimpleLogger.LogInformation($"Could not determine if process with id={processId} exists");
                    return HttpStatusCode.InternalServerError;
                }

                if (!processExists.YesNo) {
                    vSimpleLogger.LogInformation($"No process exists with id={processId}");
                    return HttpStatusCode.NotFound;
                }

                vSimpleLogger.LogInformation($"Update process with id={processId}");
                var controllableProcess = await context.ControllableProcesses.ByKey(processId).GetValueAsync();
                controllableProcess.ConfirmedAt = now;
                controllableProcess.Status = status;
                context.UpdateObject(controllableProcess);
                var response = await context.SaveChangesAsync(SaveChangesOptions.None);
                var statusCode = response.Select(r => (HttpStatusCode)r.StatusCode).FirstOrDefault();
                return statusCode;
            }
        }

        public async Task<HttpStatusCode> ConfirmDeadAsync(int processId) {
            using (vSimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(TashAccessor), vLogId))) {
                vSimpleLogger.LogInformation($"Confirm that process with id={processId} is dead");
                return await ConfirmAliveAsync(processId, DateTime.Now, ControllableProcessStatus.Dead);
            }
        }

        public void ConfirmDeadWhileClosing(int processId) {
            var task = Task.Run(async () => { await ConfirmDeadAsync(processId); });
            task.Wait();
        }

        public async Task<IList<ControllableProcessTask>> GetControllableProcessTasksAsync() {
            using (vSimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(TashAccessor), vLogId))) {
                vSimpleLogger.LogInformation("Get controllable process tasks");
                var context = new DefaultContainer(new Uri(BaseUrl));
                var processTasks = await context.ControllableProcessTasks.ExecuteAsync();
                var processList = processTasks.ToList();
                return processList;
            }
        }

        public async Task<ControllableProcessTask> GetControllableProcessTaskAsync(Guid taskId) {
            using (vSimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(TashAccessor), vLogId))) {
                if (vDetailedLogging) {
                    vSimpleLogger.LogInformation($"Get controllable process task with id={taskId}");
                }
                var context = new DefaultContainer(new Uri(BaseUrl));
                if (!await ProcessTaskExists(context, taskId)) {
                    vSimpleLogger.LogInformation($"No controllable process task found with id={taskId}");
                    return null;
                }

                if (vDetailedLogging) {
                    vSimpleLogger.LogInformation($"Returning controllable process task with id={taskId}");
                }
                var processTask = await context.ControllableProcessTasks.ByKey(taskId).GetValueAsync();
                return processTask;
            }
        }

        public async Task<HttpStatusCode> PutControllableProcessTaskAsync(ControllableProcessTask processTask) {
            using (vSimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(TashAccessor), vLogId))) {
                vSimpleLogger.LogInformation($"Put controllable process task with id={processTask.Id}");
                var context = new DefaultContainer(new Uri(BaseUrl));
                ControllableProcessTask controllableProcessTask;
                if (await ProcessTaskExists(context, processTask.Id)) {
                    vSimpleLogger.LogInformation($"Update controllable process task with id={processTask.Id}");
                    controllableProcessTask = await context.ControllableProcessTasks.ByKey(processTask.Id).GetValueAsync();
                    controllableProcessTask.ProcessId = processTask.ProcessId;
                    controllableProcessTask.Type = processTask.Type;
                    controllableProcessTask.ControlName = processTask.ControlName;
                    controllableProcessTask.Text = processTask.Text;
                    controllableProcessTask.Status = processTask.Status;
                    context.UpdateObject(controllableProcessTask);
                }
                else {
                    vSimpleLogger.LogInformation($"Insert controllable process task with id={processTask.Id}");
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
            using (vSimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(TashAccessor), vLogId))) {
                vSimpleLogger.LogInformation($"Confirm status {Enum.GetName(typeof(ControllableProcessStatus), status)} for task id={taskId}");

                DefaultContainer context;
                ControllableProcessTask controllableProcessTask = null;
                bool wasExceptionThrown;
                do {
                    wasExceptionThrown = false;
                    context = new DefaultContainer(new Uri(BaseUrl));
                    try {
                        if (!await ProcessTaskExists(context, taskId)) {
                            return HttpStatusCode.NotFound;
                        }
                        vSimpleLogger.LogInformation($"Select task with id={taskId} for update");
                        controllableProcessTask = await context.ControllableProcessTasks.ByKey(taskId).GetValueAsync();
                    } catch {
                        vSimpleLogger.LogError($"Could not select task with id={taskId} for update, trying again");
                        wasExceptionThrown = true;
                    }
                } while (wasExceptionThrown);

                if (controllableProcessTask == null) {
                    vSimpleLogger.LogInformation($"No task found with id={taskId}");
                    return HttpStatusCode.NotFound;
                }

                vSimpleLogger.LogInformation($"Update task with id={taskId}");
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
            using (vSimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(TashAccessor), vLogId))) {
                vSimpleLogger.LogInformation($"Get requested task for process with id={processId}");
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

            var process = processes.Where(p => p.Status == ControllableProcessStatus.Idle).OrderByDescending(p => p.ConfirmedAt).FirstOrDefault();
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

            using (vSimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(TashAccessor), vLogId))) {
                vSimpleLogger.LogInformation($"Awaiting completion of task with id={taskId}");
                do {
                    await Wait.UntilAsync(async () => {
                        task = await GetControllableProcessTaskAsync(taskId);
                        return task?.Status == ControllableProcessTaskStatus.Completed;
                    }, TimeSpan.FromMilliseconds(internalInMilliSeconds));

                    task = await GetControllableProcessTaskAsync(taskId);
                    if (task != null) {
                        if (task.Status == ControllableProcessTaskStatus.Completed) {
                            vSimpleLogger.LogInformation($"Task with id={taskId} is complete");
                            return task;
                        }

                        var process = await GetControllableProcessAsync(task.ProcessId);
                        if (process?.Status == ControllableProcessStatus.Dead) {
                            vSimpleLogger.LogInformation($"Process with id={task.ProcessId} is dead for task with id={taskId}");
                            return task;
                        }
                    }

                    milliSecondsToAttemptWhileRequestedOrProcessing -= internalInMilliSeconds;
                } while (0 < milliSecondsToAttemptWhileRequestedOrProcessing && (task == null || task.Status == ControllableProcessTaskStatus.Processing || task.Status == ControllableProcessTaskStatus.Requested));

                vSimpleLogger.LogInformation($"Returning incomplete task with id={taskId}");
                return task;
            }
        }
    }
}
