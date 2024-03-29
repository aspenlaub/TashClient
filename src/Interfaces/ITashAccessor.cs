using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tash;

namespace Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;

public interface ITashAccessor {
    Task<DvinApp> GetTashAppAsync(IErrorsAndInfos errorsAndInfos);
    Task<IErrorsAndInfos> EnsureTashAppIsRunningAsync();

    Task<IList<ControllableProcess>> GetControllableProcessesAsync();
    Task<ControllableProcess> GetControllableProcessAsync(int processId);
    Task<HttpStatusCode> PutControllableProcessAsync(Process process);
    Task<HttpStatusCode> ConfirmAliveAsync(int processId, DateTime now, ControllableProcessStatus status);
    Task<HttpStatusCode> ConfirmDeadAsync(int processId);
    Task ConfirmDeadWhileClosingAsync(int processId);
    bool MarkTaskAsCompleted(ControllableProcessTask theTaskIAmProcessing, int processId, string type, string controlName, string text);
    Task<ControllableProcessTask> PickRequestedTask(int processId);
    Task AssumeDeath(Func<ControllableProcess, bool> condition);
    Task<IFindIdleProcessResult> FindIdleProcess(Func<ControllableProcess, bool> condition);

    Task<IList<ControllableProcessTask>> GetControllableProcessTasksAsync();
    Task<ControllableProcessTask> GetControllableProcessTaskAsync(Guid taskId);
    Task<HttpStatusCode> PutControllableProcessTaskAsync(ControllableProcessTask processTask);
    Task<HttpStatusCode> ConfirmStatusAsync(Guid taskId, ControllableProcessTaskStatus status);
    Task<HttpStatusCode> ConfirmStatusAsync(Guid taskId, ControllableProcessTaskStatus status, string text, string errorMessage);
    Task<ControllableProcessTask> AwaitCompletionAsync(Guid taskId, int milliSecondsToAttemptWhileRequestedOrProcessing);
}