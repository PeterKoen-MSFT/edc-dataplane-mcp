using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace DataPlane.McpServer.Tests
{
    public class McpControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public McpControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Mock JWT validation for testing
                    services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        options.TokenValidationParameters = new()
                        {
                            SignatureValidationCallback = (token, key, _, _) => true
                        };
                    });
                });
            });
        }

        [Fact]
        public async Task Post_WithoutAuth_Returns401()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsync("/mcp/stream", new StringContent(""));

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Post_WithInvalidJsonRpc_Returns400()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new("Bearer", "mock-token");
            var invalidJson = "{invalid-json}";

            // Act
            var response = await client.PostAsync("/mcp/stream", 
                new StringContent(invalidJson, Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        [Fact]
        public async Task Post_ValidRequest_SucceedsWithMethodNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new("Bearer", "mock-token");
            var jsonRpc = @"{
                ""jsonrpc"": ""2.0"",
                ""method"": ""mcp.tool.transferStatus"",
                ""params"": { ""transferId"": ""123"" },
                ""id"": 1
            }";

            // Act
            var response = await client.PostAsync("/mcp/stream",
                new StringContent(jsonRpc, Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("method not found", content.ToLowerInvariant());
        }
    }
}
