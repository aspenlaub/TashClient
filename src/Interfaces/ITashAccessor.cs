using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tash;

namespace Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces {
    public interface ITashAccessor {
        Task<DvinApp> GetTashAppAsync(IErrorsAndInfos errorsAndInfos);
        Task<IErrorsAndInfos> EnsureTashAppIsRunningAsync();

        Task<IEnumerable<ControllableProcess>> GetControllableProcessesAsync();
        Task<ControllableProcess> GetControllableProcessAsync(int processId);
        Task<HttpStatusCode> PutControllableProcessAsync(Process process);
        Task<HttpStatusCode> ConfirmAliveAsync(int processId, DateTime now, ControllableProcessStatus status);
        Task<HttpStatusCode> ConfirmDeadAsync(int processId);
        void ConfirmDeadWhileClosing(int processId);
        bool MarkTaskAsCompleted(ControllableProcessTask theTaskIAmProcessing, int processId, ControllableProcessTaskType type, string controlName, string text);
        Task<ControllableProcessTask> PickRequestedTask(int processId);
        Task AssumeDeath(Func<ControllableProcess, bool> condition);
        Task<IFindIdleProcessResult> FindIdleProcess(Func<ControllableProcess, bool> condition);

        Task<IEnumerable<ControllableProcessTask>> GetControllableProcessTasksAsync();
        Task<ControllableProcessTask> GetControllableProcessTaskAsync(Guid taskId);
        Task<HttpStatusCode> PutControllableProcessTaskAsync(ControllableProcessTask processTask);
        Task<HttpStatusCode> ConfirmStatusAsync(Guid taskId, ControllableProcessTaskStatus status);
        Task<ControllableProcessTask> AwaitCompletionAsync(Guid taskId, int milliSecondsToAttemptWhileRequestedOrProcessing);
    }
}