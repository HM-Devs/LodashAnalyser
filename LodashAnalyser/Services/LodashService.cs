using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;

public class LodashService
{
    private readonly HttpClient _httpClient;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

    public LodashService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        //Use Polly to define a retry police which will handle our transient errors (if any)
        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (outcome, timespan, retryAttempt, context) =>
                {
                    Console.WriteLine($"Retry {retryAttempt} due to {outcome.Exception?.Message ?? outcome.Result.ReasonPhrase}");
                });
    }

    //Follow TAP pattern for return types
    public virtual async Task<List<string>> GetRepoFiles(string owner, string repo)
    {
        //'?recursive=1' returns all files/directories traversed under the root/subdirectories
        var url = $"https://api.github.com/repos/{owner}/{repo}/git/trees/main?recursive=1";
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "request");

        var response = await _retryPolicy.ExecuteAsync(() => _httpClient.GetAsync(url));
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(responseBody);

        //Extract tree array from JSON response 
        var files = json["tree"]
            .Where(t => t["type"].ToString() == "blob")
            //Projects each item to its 'path' property, converts to a string
            .Select(t => t["path"].ToString())
            .ToList();

        return files;
    }

    public virtual async Task<string> GetFileContent(string owner, string repo, string filePath)
    {
        var url = $"https://raw.githubusercontent.com/{owner}/{repo}/main/{filePath}";
        var response = await _retryPolicy.ExecuteAsync(() => _httpClient.GetAsync(url));
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return content;
    }
}
