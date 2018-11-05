using Aspenlaub.Net.GitHub.CSharp.Tash.Model;

namespace Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces {
    public interface IFindIdleProcessResult {
        bool AnyHandshake { get; set; }
        ControllableProcess ControllableProcess { get; set; }
        ControllableProcessStatus BestNonIdleProcessStatus { get; set; }
    }
}
