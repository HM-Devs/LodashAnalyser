using System.Net;
using Moq;
using Moq.Protected;

public class LodashServiceTests
{
    [Fact]
    public async Task GetRepoFiles_ShouldReturnFileList()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                //Simulate response content returned from Github API when GetRepoFiles method makes a request
                Content = new StringContent("{\"tree\": [{\"path\": \"file1.js\", \"type\": \"blob\"}, {\"path\": \"file2.ts\", \"type\": \"blob\"}]}")
            });
        var httpClient = new HttpClient(handlerMock.Object);
        var lodashService = new LodashService(httpClient);

        // Act
        var files = await lodashService.GetRepoFiles("lodash", "lodash");

        // Assert
        Assert.NotNull(files);
        Assert.Contains("file1.js", files);
        Assert.Contains("file2.ts", files);
    }

    [Fact]
    public async Task GetRepoFiles_ShouldHandleEmptyResponse()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"tree\": []}")
            });
        var httpClient = new HttpClient(handlerMock.Object);
        var lodashService = new LodashService(httpClient);

        // Act
        var files = await lodashService.GetRepoFiles("lodash", "lodash");

        // Assert
        Assert.Empty(files);
    }
}
