using System;
using System.Net.Http;

namespace Aspenlaub.Net.GitHub.CSharp.TashClient.Components;

public class HttpClientFactory : IHttpClientFactory {
    private const int _timeoutInSeconds = 60;

    public HttpClient CreateClient(string name) {
        return new HttpClient { Timeout = TimeSpan.FromSeconds(_timeoutInSeconds) };
    }
}
