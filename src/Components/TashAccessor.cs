using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Entities;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Repositories;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Entities;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tash;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tash.Model;
using Microsoft.OData.Client;

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
            tashApp.Start(errorsAndInfos);
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
            var process = await context.ControllableProcesses.ByKey(processId).GetValueAsync();
            return process;
        }

        public async Task<HttpStatusCode> PutControllableProcessAsync(Process process) {
            var controllableProcess = new ControllableProcess {
                ProcessId = process.Id,
                Title = process.ProcessName,
                Busy = false,
                ConfirmedAt = DateTimeOffset.Now,
                LaunchCommand = process.MainModule.FileName
            };
            var context = new DefaultContainer(new Uri(BaseUrl));
            context.AddToControllableProcesses(controllableProcess);
            var response = await context.SaveChangesAsync(SaveChangesOptions.ReplaceOnUpdate);
            var statusCode = response.Select(r => (HttpStatusCode)r.StatusCode).FirstOrDefault();
            return statusCode;
        }
    }
}
