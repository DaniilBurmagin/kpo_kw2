using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Protected;
using Xunit;
using FileAnalysisService.Controllers;
using FileAnalysisService.Services;
using FileAnalysisService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class AnalysisControllerTests
{
    private AnalysisDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AnalysisDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AnalysisDbContext(options);
    }

    [Fact]
    public async Task Analyze_FileNotFound()
    {
        // Arrange
        var mockService = new Mock<AnalysisService>();
        var mockFactory = new Mock<IHttpClientFactory>();
        var db = GetInMemoryDb();

        // Подделка клиента, возвращает 404
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });
        var client = new HttpClient(handler.Object);
        mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

        var controller = new AnalysisController(mockService.Object, mockFactory.Object, db);

        // Act
        var result = await controller.Analyze(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundObjectResult>(result); // тк вернёт NotFound("File not found in storage service.")
    }

    [Fact]
    public async Task GetAnalysis_NotFound()
    {
        // Arrange
        var mockService = new Mock<AnalysisService>();
        var mockFactory = new Mock<IHttpClientFactory>();
        var db = GetInMemoryDb();

        var controller = new AnalysisController(mockService.Object, mockFactory.Object, db);

        // Act
        var result = await controller.GetAnalysis(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}
