namespace Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces {
    public interface ILogConfiguration {
        string LogSubFolder { get; }
        string LogId { get; }
        bool DetailedLogging { get; }
    }
}
