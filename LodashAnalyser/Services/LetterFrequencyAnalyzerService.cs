using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class LetterFrequencyAnalyzerService
{
    private readonly LodashService _lodashService;
    private readonly FileProcessorService _fileProcessor;

    public LetterFrequencyAnalyzerService(LodashService lodashService, FileProcessorService fileProcessor)
    {
        _lodashService = lodashService;
        _fileProcessor = fileProcessor;
    }

    public async Task<List<string>> FetchAndFilterFileContents(string owner, string repo)
    {
        var files = await _lodashService.GetRepoFiles(owner, repo);

        // Filter for the JS/TS files in our given repository
        var jsTsFiles = files.Where(file => file.EndsWith(".js", StringComparison.OrdinalIgnoreCase) ||
                                            file.EndsWith(".ts", StringComparison.OrdinalIgnoreCase))
                             .ToList();

        if (!jsTsFiles.Any())
        {
            Console.WriteLine("No JavaScript or TypeScript files found in the repository.");
            return new List<string>();
        }

        var tasks = jsTsFiles.Select(file => _lodashService.GetFileContent(owner, repo, file)).ToList();

        var contents = await Task.WhenAll(tasks);

        //Filter out empty/whitespace only file contents
        return contents.Where(content => !string.IsNullOrWhiteSpace(content)).ToList();
    }


    public void ProcessAndDisplayFrequencies(List<string> fileContents)
    {
        if (!fileContents.Any())
        {
            Console.WriteLine("No JS/TS files with content found.");
            return;
        }

        var letterCounts = _fileProcessor.CountLetterFrequencies(fileContents);
        var orderedCounts = _fileProcessor.GetOrderedFrequencies(letterCounts);


        //Returns output as a set of key value pairs
        Console.WriteLine("Letter frequencies in descending order:");
        foreach (var kvp in orderedCounts)
        {
            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
        }
    }
}
