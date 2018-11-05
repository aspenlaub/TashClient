using Aspenlaub.Net.GitHub.CSharp.Tash.Model;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.TashClient.Entities {
    public class FindIdleProcessResult : IFindIdleProcessResult {
        public ControllableProcessStatus BestProcessStatus { get; set; }
        public ControllableProcess ControllableProcess { get; set; }
    }
}
