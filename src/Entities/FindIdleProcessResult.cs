using Aspenlaub.Net.GitHub.CSharp.Tash.Model;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.TashClient.Entities {
    public class FindIdleProcessResult : IFindIdleProcessResult {
        public bool AnyHandshake { get; set; }
        public ControllableProcess ControllableProcess { get; set; }
        public ControllableProcessStatus BestNonIdleProcessStatus { get; set; }
    }
}
