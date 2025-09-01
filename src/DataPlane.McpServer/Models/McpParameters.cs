namespace DataPlane.McpServer.Models;

public record FetchDataRequest(string EdrId, string ResourcePath, Dictionary<string, string>? Headers = null);
public record ResolveCatalogRequest(Dictionary<string, string>? Filter = null);
public record TransferStatusRequest(string TransferId);

public record TransferStatusResponse(string State, string Message, Dictionary<string, string> Metrics);
