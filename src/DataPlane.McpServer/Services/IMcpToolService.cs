using DataPlane.McpServer.Models;

namespace DataPlane.McpServer.Services;

public interface IMcpToolService
{
    Task<Stream> FetchDataViaEdrAsync(FetchDataRequest request);
    Task<string> ResolveCatalogAsync(ResolveCatalogRequest request);
    Task<TransferStatusResponse> TransferStatusAsync(TransferStatusRequest request);
}
