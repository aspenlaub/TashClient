using Aspenlaub.Net.GitHub.CSharp.Dvin.Components;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Entities;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Repositories;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Entities;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Interfaces;
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
                controllableProcess.Busy = false;
                controllableProcess.ConfirmedAt = DateTimeOffset.Now;
                controllableProcess.LaunchCommand = process.MainModule.FileName;
                context.UpdateObject(controllableProcess);
            } else {
                controllableProcess = new ControllableProcess {
                    ProcessId = process.Id,
                    Title = process.ProcessName,
                    Busy = false,
                    ConfirmedAt = DateTimeOffset.Now,
                    LaunchCommand = process.MainModule.FileName
                };
                context.AddToControllableProcesses(controllableProcess);
            }

            var response = await context.SaveChangesAsync(SaveChangesOptions.ReplaceOnUpdate);
            var statusCode = response.Select(r => (HttpStatusCode)r.StatusCode).FirstOrDefault();
            return statusCode;
        }

        public async Task<HttpStatusCode> ConfirmAliveAsync(int processId, DateTime now, bool busy) {
            var context = new DefaultContainer(new Uri(BaseUrl));
            if (!await ProcessExists(context, processId)) {
                return HttpStatusCode.NotFound;
            }

            var controllableProcess = await context.ControllableProcesses.ByKey(processId).GetValueAsync();
            controllableProcess.ConfirmedAt = now;
            controllableProcess.Busy = busy;
            context.UpdateObject(controllableProcess);
            var response = await context.SaveChangesAsync(SaveChangesOptions.None);
            var statusCode = response.Select(r => (HttpStatusCode)r.StatusCode).FirstOrDefault();
            return statusCode;
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
    }
}
