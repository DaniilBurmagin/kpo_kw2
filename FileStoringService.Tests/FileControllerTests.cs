using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FileStoringService.Controllers;
using FileStoringService.Models;
using FileStoringService.Services;

public class FileControllerTests
{
    [Fact]
    public async Task Upload_ReturnsFileId()
    {
        // Arrange
        var mockStorage = new Mock<FileStorageService>();
        mockStorage.Setup(x => x.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<Guid>()))
                   .Returns(Task.CompletedTask);

        var controller = new FilesController(mockStorage.Object);

        var content = "Test file content";
        var bytes = Encoding.UTF8.GetBytes(content);
        using var stream = new MemoryStream(bytes);
        var formFile = new FormFile(stream, 0, bytes.Length, "file", "test.txt")
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/plain"
        };
        var model = new FileUploadModel { File = formFile };

        // Act
        var result = await controller.Upload(model);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetFile_ReturnsNotFound()
    {
        // Arrange
        var mockStorage = new Mock<FileStorageService>();
        mockStorage.Setup(x => x.GetFileAsync(It.IsAny<Guid>()))
                   .ThrowsAsync(new FileNotFoundException());

        var controller = new FilesController(mockStorage.Object);

        // Act
        var result = await controller.GetFile(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
