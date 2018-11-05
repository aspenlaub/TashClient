using Aspenlaub.Net.GitHub.CSharp.Tash.Model;

namespace Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces {
    public interface IFindIdleProcessResult {
        ControllableProcessStatus BestProcessStatus { get; set; }
        ControllableProcess ControllableProcess { get; set; }
    }
}
