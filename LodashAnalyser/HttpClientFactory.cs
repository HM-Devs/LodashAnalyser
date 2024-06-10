using System.Net.Http;

public static class HttpClientFactory
{
    public static HttpClient CreateHttpClient()
    {
        // Create an HttpClient instance with a User-Agent header
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "request");
        return httpClient;
    }
}
