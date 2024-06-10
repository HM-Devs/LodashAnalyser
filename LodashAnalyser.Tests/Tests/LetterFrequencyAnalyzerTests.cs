using Moq;
using System.Collections.Concurrent;

public class LetterFrequencyAnalyzerTests
{
    [Fact]
    public async Task FetchAndFilterFileContents_ShouldReturnNonEmptyContents()
    {
        // Arrange
        var mockLodashService = new Mock<LodashService>(new HttpClient());
        var mockFileProcessor = new Mock<FileProcessorService>();
        var mockAnalyserService = new LetterFrequencyAnalyzerService(mockLodashService.Object, mockFileProcessor.Object);

        mockLodashService.Setup(s => s.GetRepoFiles(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new List<string> { "file1.js", "file2.ts", "file3.txt" });

        mockLodashService.Setup(s => s.GetFileContent(It.IsAny<string>(), It.IsAny<string>(), "file1.js"))
            .ReturnsAsync("var a = 1;");

        mockLodashService.Setup(s => s.GetFileContent(It.IsAny<string>(), It.IsAny<string>(), "file2.ts"))
            .ReturnsAsync("let b = 2;");

        // Sets up empty file contents so we only get   a/b
        mockLodashService.Setup(s => s.GetFileContent(It.IsAny<string>(), It.IsAny<string>(), "file3.txt"))
            .ReturnsAsync("");

        // Act
        var result = await mockAnalyserService.FetchAndFilterFileContents("owner", "repo");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains("var a = 1;", result);
        Assert.Contains("let b = 2;", result);
    }

    [Fact]
    public void ProcessAndDisplayFrequencies_ShouldDisplayCorrectly()
    {
        // Arrange
        var mockLodashService = new Mock<LodashService>(new HttpClient());
        var mockFileProcessor = new Mock<FileProcessorService>();
        var mockAnalyserService = new LetterFrequencyAnalyzerService(mockLodashService.Object, mockFileProcessor.Object);

        // Contents from our file
        var contents = new List<string> { "ccc", "ccc", "ccc", "bbbbb", "aaa" };
        var letterCounts = new ConcurrentDictionary<char, int>
        {
            ['a'] = 3, 
            ['b'] = 5, 
            ['c'] = 9  
        };

        mockFileProcessor.Setup(fp => fp.CountLetterFrequencies(It.IsAny<IEnumerable<string>>()))
            .Returns(letterCounts);

        mockFileProcessor.Setup(fp => fp.GetOrderedFrequencies(It.IsAny<ConcurrentDictionary<char, int>>()))
            .Returns(letterCounts.OrderByDescending(kvp => kvp.Value));

        // Act
        using (var sw = new StringWriter())
        {
            Console.SetOut(sw);
            mockAnalyserService.ProcessAndDisplayFrequencies(contents);

            // Assert
            var expected = string.Join(Environment.NewLine, new[]
            {
            "Letter frequencies in descending order:",
            "c: 9",
            "b: 5", 
            "a: 3",
            ""
        });

            Assert.Equal(expected, sw.ToString());
        }
    }
}