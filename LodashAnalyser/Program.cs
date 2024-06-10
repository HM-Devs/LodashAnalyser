using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        //defines the owner and repository itself we are searching against
        var owner = "lodash";
        var repo = "lodash";

        var httpClient = HttpClientFactory.CreateHttpClient();
        var lodashService = new LodashService(httpClient);
        var fileProcessorService = new FileProcessorService();
        var analyzer = new LetterFrequencyAnalyzerService(lodashService, fileProcessorService);

        var fileContents = await analyzer.FetchAndFilterFileContents(owner, repo);
        analyzer.ProcessAndDisplayFrequencies(fileContents);
    }
}