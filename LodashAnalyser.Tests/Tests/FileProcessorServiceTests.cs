using System.Collections.Concurrent;

public class FileProcessorServiceTests
{
    [Fact]
    public void CountLetterFrequencies_ShouldCountCorrectly()
    {
        // Arrange
        var fileProcessorService = new FileProcessorService();
        var contents = new List<string> { "aaabbcc", "abc" };

        // Act
        var result = fileProcessorService.CountLetterFrequencies(contents);

        // Assert
        Assert.Equal(4, result['a']);
        Assert.Equal(3, result['b']);
        Assert.Equal(3, result['c']);
    }

    [Fact]
    public void CountLetterFrequencies_ShouldHandleEmptyFiles()
    {
        // Arrange
        var fileProcessorService = new FileProcessorService();
        var contents = new List<string> { "", " " };

        // Act
        var result = fileProcessorService.CountLetterFrequencies(contents);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetOrderedFrequencies_ShouldOrderCorrectly()
    {
        // Arrange
        var fileProcessorService = new FileProcessorService();
        var letterCounts = new ConcurrentDictionary<char, int>();
        letterCounts['a'] = 5;
        letterCounts['b'] = 3;
        letterCounts['c'] = 8;

        // Act
        var result = fileProcessorService.GetOrderedFrequencies(letterCounts);

        // Assert
        var orderedList = result.ToList();
        Assert.Equal('c', orderedList[0].Key);
        Assert.Equal('a', orderedList[1].Key);
        Assert.Equal('b', orderedList[2].Key);
    }
}