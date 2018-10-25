using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Entities;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tash.Model;

namespace Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces {
    public interface ITashAccessor {
        Task<DvinApp> GetTashAppAsync();
        Task<IErrorsAndInfos> EnsureTashAppIsRunningAsync();

        Task<IEnumerable<ControllableProcess>> GetControllableProcessesAsync();
        Task<ControllableProcess> GetControllableProcessAsync(int processId);
        Task<HttpStatusCode> PutControllableProcessAsync(Process process);
        Task<HttpStatusCode> ConfirmAliveAsync(int processId, DateTime now, bool busy);

        Task<IEnumerable<ControllableProcessTask>> GetControllableProcessTasksAsync();
        Task<ControllableProcessTask> GetControllableProcessTaskAsync(Guid taskId);
        Task<HttpStatusCode> PutControllableProcessTaskAsync(ControllableProcessTask processTask);
        Task<HttpStatusCode> ConfirmStatusAsync(Guid taskId, ControllableProcessTaskStatus status);
    }
}