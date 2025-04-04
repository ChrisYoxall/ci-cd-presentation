using System.Net;
using System.Text.Json;
using BuildDeploy.Demo.Controllers;
using Moq;
using Moq.Protected;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace BuildDeploy.Demo.Tests
{
    public class GitHubUserControllerTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;

        public GitHubUserControllerTests()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        }
        
        private void ConfigureHttpClientFactory()
        {
            var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://api.github.com/")
            };

            _httpClientFactoryMock
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);
        }
        
        [Fact]
        public async Task ValidateGitHubUser_ReturnsOkWithUser_WhenUserExists()
        {
            // Arrange
            const string username = "existingUser";
            var gitHubUser = new GitHubUser
            {
                Login = username,
                Id = 12345,
                Name = "John Doe",
                Company = "GitHub",
                PublicRepos = 5
            };

            var handlerResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(gitHubUser))
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(handlerResponse);

            ConfigureHttpClientFactory();

            var controller = new GitHubUserController(_httpClientFactoryMock.Object);

            // Act
            var result = await controller.ValidateGitHubUser(username);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ValidateResponse>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var responseValue = Assert.IsType<ValidateResponse>(okResult.Value);

            Assert.True(responseValue.IsValid);
            Assert.Equal(gitHubUser.Login, responseValue.User?.Login);
            Assert.Equal(gitHubUser.Id, responseValue.User?.Id);
            Assert.Equal(gitHubUser.Name, responseValue.User?.Name);
            Assert.Equal(gitHubUser.Company, responseValue.User?.Company);
        }

        [Fact]
        public async Task ValidateGitHubUser_ReturnsOkWithInvalidFlag_WhenUserNotFound()
        {
            // Arrange
            const string username = "nonExistentUser";

            var handlerResponse = new HttpResponseMessage(HttpStatusCode.NotFound);

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(handlerResponse);

            ConfigureHttpClientFactory();

            var controller = new GitHubUserController(_httpClientFactoryMock.Object);

            // Act
            var result = await controller.ValidateGitHubUser(username);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ValidateResponse>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var responseValue = Assert.IsType<ValidateResponse>(okResult.Value);

            Assert.False(responseValue.IsValid);
            Assert.Null(responseValue.User);
        }

        [Fact]
        public async Task ValidateGitHubUser_ReturnsError_WhenApiRateLimited()
        {
            // Arrange
            const string username = "rateLimitedUser";

            var handlerResponse = new HttpResponseMessage((HttpStatusCode)429); // Too Many Requests

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(handlerResponse);

            ConfigureHttpClientFactory();

            var controller = new GitHubUserController(_httpClientFactoryMock.Object);

            // Act
            var result = await controller.ValidateGitHubUser(username);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ValidateResponse>>(result);
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(429, objectResult.StatusCode);

            var errorResponse = Assert.IsType<ApiErrorResponse>(objectResult.Value);
            Assert.Contains("GitHub API returned status code", errorResponse.Error);
        }

        [Fact]
        public async Task ValidateGitHubUser_ReturnsServerError_WhenExceptionThrown()
        {
            // Arrange
            const string username = "errorUser";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Connection failed"));

            ConfigureHttpClientFactory();

            var controller = new GitHubUserController(_httpClientFactoryMock.Object);

            // Act
            var result = await controller.ValidateGitHubUser(username);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ValidateResponse>>(result);
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(500, objectResult.StatusCode);

            var errorResponse = Assert.IsType<ApiErrorResponse>(objectResult.Value);
            Assert.Contains("Error connecting to GitHub API", errorResponse.Error);
        }
    }
}