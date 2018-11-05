using Aspenlaub.Net.GitHub.CSharp.Dvin.Components;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Entities;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Repositories;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tash;
using Aspenlaub.Net.GitHub.CSharp.Tash.Model;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Microsoft.OData.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Entities;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Helpers;

namespace Aspenlaub.Net.GitHub.CSharp.TashClient.Components {
    public class TashAccessor : ITashAccessor {
        private const string BaseUrl = "http://localhost:60404", TashAppId = "Tash";

        public async Task<DvinApp> GetTashAppAsync() {
            var repository = new DvinRepository();
            return await repository.LoadAsync(TashAppId);
        }

        public async Task<IErrorsAndInfos> EnsureTashAppIsRunningAsync() {
            var errorsAndInfos = new ErrorsAndInfos();
            try {
                var processes = await GetControllableProcessesAsync();
                if (processes != null) { return errorsAndInfos; }
            // ReSharper disable once EmptyGeneralCatchClause
            } catch {
            }
            var tashApp = await GetTashAppAsync();
            var fileSystemService = new FileSystemService();
            tashApp.Start(fileSystemService, errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                return errorsAndInfos;
            }

            await Task.Delay(TimeSpan.FromSeconds(10));

            try {
                var processes = await GetControllableProcessesAsync();
                if (processes != null) { return errorsAndInfos; }
                // ReSharper disable once EmptyGeneralCatchClause
            } catch {
                errorsAndInfos.Errors.Add(@"Tash started but not answering"); // Should this occur regularly, maybe the Tash process can be killed
            }
            return errorsAndInfos;
        }

        public async Task<IEnumerable<ControllableProcess>> GetControllableProcessesAsync() {
            var context = new DefaultContainer(new Uri(BaseUrl));
            var processes = await context.ControllableProcesses.ExecuteAsync();
            return processes;
        }

        public async Task<ControllableProcess> GetControllableProcessAsync(int processId) {
            var context = new DefaultContainer(new Uri(BaseUrl));
            if (!await ProcessExists(context, processId)) {
                return null;
            }

            var process = await context.ControllableProcesses.ByKey(processId).GetValueAsync();
            return process;
        }

        public async Task<HttpStatusCode> PutControllableProcessAsync(Process process) {
            var context = new DefaultContainer(new Uri(BaseUrl));
            ControllableProcess controllableProcess;
            if (await ProcessExists(context, process.Id)) {
                controllableProcess = await context.ControllableProcesses.ByKey(process.Id).GetValueAsync();
                controllableProcess.Title = process.ProcessName;
                controllableProcess.Status = ControllableProcessStatus.Idle;
                controllableProcess.ConfirmedAt = DateTimeOffset.Now;
                controllableProcess.LaunchCommand = process.MainModule.FileName;
                context.UpdateObject(controllableProcess);
            } else {
                controllableProcess = new ControllableProcess {
                    ProcessId = process.Id,
                    Title = process.ProcessName,
                    Status = ControllableProcessStatus.Idle,
                    ConfirmedAt = DateTimeOffset.Now,
                    LaunchCommand = process.MainModule.FileName
                };
                context.AddToControllableProcesses(controllableProcess);
            }

            var response = await context.SaveChangesAsync(SaveChangesOptions.ReplaceOnUpdate);
            var statusCode = response.Select(r => (HttpStatusCode)r.StatusCode).FirstOrDefault();
            return statusCode;
        }

        public async Task<HttpStatusCode> ConfirmAliveAsync(int processId, DateTime now, ControllableProcessStatus status) {
            var context = new DefaultContainer(new Uri(BaseUrl));
            if (!await ProcessExists(context, processId)) {
                return HttpStatusCode.NotFound;
            }

            var controllableProcess = await context.ControllableProcesses.ByKey(processId).GetValueAsync();
            controllableProcess.ConfirmedAt = now;
            controllableProcess.Status = status;
            context.UpdateObject(controllableProcess);
            var response = await context.SaveChangesAsync(SaveChangesOptions.None);
            var statusCode = response.Select(r => (HttpStatusCode)r.StatusCode).FirstOrDefault();
            return statusCode;
        }

        public async Task<HttpStatusCode> ConfirmDeadAsync(int processId) {
            return await ConfirmAliveAsync(processId, DateTime.Now, ControllableProcessStatus.Dead);
        }

        public void ConfirmDeadWhileClosing(int processId) {
            var task = Task.Run(async () => { await ConfirmDeadAsync(processId); });
            task.Wait();
        }

        public async Task<IEnumerable<ControllableProcessTask>> GetControllableProcessTasksAsync() {
            var context = new DefaultContainer(new Uri(BaseUrl));
            var processTasks = await context.ControllableProcessTasks.ExecuteAsync();
            return processTasks;
        }

        public async Task<ControllableProcessTask> GetControllableProcessTaskAsync(Guid taskId) {
            var context = new DefaultContainer(new Uri(BaseUrl));
            if (!await ProcessTaskExists(context, taskId)) {
                return null;
            }

            var processTask = await context.ControllableProcessTasks.ByKey(taskId).GetValueAsync();
            return processTask;
        }

        public async Task<HttpStatusCode> PutControllableProcessTaskAsync(ControllableProcessTask processTask) {
            var context = new DefaultContainer(new Uri(BaseUrl));
            ControllableProcessTask controllableProcessTask;
            if (await ProcessTaskExists(context, processTask.Id)) {
                controllableProcessTask = await context.ControllableProcessTasks.ByKey(processTask.Id).GetValueAsync();
                controllableProcessTask.ProcessId = processTask.ProcessId;
                controllableProcessTask.Type = processTask.Type;
                controllableProcessTask.ControlName = processTask.ControlName;
                controllableProcessTask.Text = processTask.Text;
                controllableProcessTask.Status = processTask.Status;
                context.UpdateObject(controllableProcessTask);
            } else {
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
            var statusCode = response.Select(r => (HttpStatusCode)r.StatusCode).FirstOrDefault();
            return statusCode;
        }

        public async Task<HttpStatusCode> ConfirmStatusAsync(Guid taskId, ControllableProcessTaskStatus status) {
            var context = new DefaultContainer(new Uri(BaseUrl));
            if (!await ProcessTaskExists(context, taskId)) {
                return HttpStatusCode.NotFound;
            }

            var controllableProcessTask = await context.ControllableProcessTasks.ByKey(taskId).GetValueAsync();
            controllableProcessTask.Status = status;
            context.UpdateObject(controllableProcessTask);
            var response = await context.SaveChangesAsync(SaveChangesOptions.None);
            var statusCode = response.Select(r => (HttpStatusCode)r.StatusCode).FirstOrDefault();
            return statusCode;
        }

        private async Task<bool> ProcessExists(DefaultContainer context, int processId) {
            var query = (DataServiceQuery<ControllableProcess>)context.ControllableProcesses.Where(p => p.ProcessId == processId || p.ProcessId == -4711); // Hack, hack, hack
            var controllableProcesses = await query.ExecuteAsync();
            return controllableProcesses.Any();
        }

        private async Task<bool> ProcessTaskExists(DefaultContainer context, Guid taskId) {
            var query = (DataServiceQuery<ControllableProcessTask>)context.ControllableProcessTasks.Where(p => p.Id == taskId || p.Id == Guid.NewGuid()); // Hack, hack, hack
            var controllableProcessTasks = await query.ExecuteAsync();
            return controllableProcessTasks.Any();
        }

        public bool MarkTaskAsCompleted(ControllableProcessTask theTaskIAmProcessing, int processId, ControllableProcessTaskType type, string controlName, string text) {
            return theTaskIAmProcessing != null
                   && theTaskIAmProcessing.Status == ControllableProcessTaskStatus.Processing
                   && theTaskIAmProcessing.ProcessId == processId
                   && theTaskIAmProcessing.Type == type
                   && theTaskIAmProcessing.ControlName == controlName
                   && theTaskIAmProcessing.Text == text;
        }

        public async Task<ControllableProcessTask> PickRequestedTask(int processId) {
            var tasks = await GetControllableProcessTasksAsync();
            return tasks.FirstOrDefault(t => t.ProcessId == processId && t.Status == ControllableProcessTaskStatus.Requested);
        }

        public async Task AssumeDeath(Func<ControllableProcess, bool> condition) {
            var processes = await GetControllableProcessesAsync();
            foreach (var process in processes.Where(p => condition(p) && p.Status != ControllableProcessStatus.Dead)) {
                await ConfirmAliveAsync(process.ProcessId, DateTime.Now, ControllableProcessStatus.Dead);
            }
        }

        public async Task<IFindIdleProcessResult> FindIdleProcess(Func<ControllableProcess, bool> condition) {
            var processes = (await GetControllableProcessesAsync()).Where(p => condition(p)).ToList();
            if (!processes.Any()) { return new FindIdleProcessResult { AnyHandshake = false };}

            IFindIdleProcessResult result = new FindIdleProcessResult { AnyHandshake = true };

            var process = processes.Where(p => p.Status == ControllableProcessStatus.Idle).OrderByDescending(p => p.ConfirmedAt).FirstOrDefault();
            if (process != null) {
                result.ControllableProcess = process;
                result.BestNonIdleProcessStatus = ControllableProcessStatus.Idle;
                return result;
            }

            var nonIdleProcess = processes.FirstOrDefault(p => p.Status == ControllableProcessStatus.Busy);
            result.BestNonIdleProcessStatus = nonIdleProcess == null ? ControllableProcessStatus.Dead : ControllableProcessStatus.Busy;
            return result;
        }

        public async Task<ControllableProcessTask> AwaitCompletionAsync(Guid taskId, int milliSecondsToAttemptWhileRequestedOrProcessing) {
            const int internalInMilliSeconds = 100;
            ControllableProcessTask task;

            do {
                await Wait.UntilAsync(async () => {
                    task = await GetControllableProcessTaskAsync(taskId);
                    return task?.Status == ControllableProcessTaskStatus.Completed;
                }, TimeSpan.FromMilliseconds(internalInMilliSeconds));

                task = await GetControllableProcessTaskAsync(taskId);
                if (task.Status == ControllableProcessTaskStatus.Completed) {
                    return task;
                }

                var process = await GetControllableProcessAsync(task.ProcessId);
                if (process?.Status == ControllableProcessStatus.Dead) {
                    return task;
                }

                milliSecondsToAttemptWhileRequestedOrProcessing -= internalInMilliSeconds;
            } while (0 < milliSecondsToAttemptWhileRequestedOrProcessing && (task.Status == ControllableProcessTaskStatus.Processing || task.Status == ControllableProcessTaskStatus.Requested));

            return task;
        }
    }
}
