using System.Text;
using DataPlane.McpServer.Models;

namespace DataPlane.McpServer.Services;

public class MockMcpToolService : IMcpToolService
{
    public Task<Stream> FetchDataViaEdrAsync(FetchDataRequest request)
    {
        // Mock implementation - returns a simple stream
        var data = $"Mock data for EDR {request.EdrId}, path: {request.ResourcePath}";
        return Task.FromResult<Stream>(new MemoryStream(Encoding.UTF8.GetBytes(data)));
    }

    public Task<string> ResolveCatalogAsync(ResolveCatalogRequest request)
    {
        // Mock implementation - returns a simple catalog
        var catalog = @"{
            ""@context"": { ""dcat"": ""http://www.w3.org/ns/dcat#"" },
            ""@type"": ""dcat:Catalog"",
            ""datasets"": []
        }";
        return Task.FromResult(catalog);
    }

    public Task<TransferStatusResponse> TransferStatusAsync(TransferStatusRequest request)
    {
        // Mock implementation
        return Task.FromResult(new TransferStatusResponse(
            State: "COMPLETED",
            Message: $"Transfer {request.TransferId} completed successfully",
            Metrics: new Dictionary<string, string>
            {
                { "bytesTransferred", "1024" },
                { "duration", "1.5" }
            }
        ));
    }
}
