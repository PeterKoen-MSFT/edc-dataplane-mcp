# DataPlane.McpServer

This component implements the Model Context Protocol (MCP) server, providing JSON-RPC 2.0 endpoints over HTTP for data plane operations.

## Features

- JSON-RPC 2.0 over HTTP streaming endpoint (`/mcp/stream`)
- Bearer token authentication against Keycloak
- Required scopes: `edr.read`, `transfer.read`
- OpenTelemetry integration for traces, metrics, and logs
- Methods implemented:
  - `mcp.tool.fetchDataViaEdr`
  - `mcp.tool.resolveCatalog`
  - `mcp.tool.transferStatus`

## Configuration

The following settings can be configured via `appsettings.json` or environment variables:

### Authentication

```json
{
  "Authentication": {
    "Authority": "http://localhost:8080/auth/realms/dataplane",
    "Audience": "dataplane-mcp",
    "RequiredScopes": ["edr.read", "transfer.read"]
  }
}
```

Environment variables:
- `Authentication__Authority`
- `Authentication__Audience`
- `Authentication__RequiredScopes`

### OpenTelemetry

```json
{
  "OpenTelemetry": {
    "ServiceName": "DataPlane.McpServer",
    "OtlpExporter": {
      "Endpoint": "http://localhost:4317"
    }
  }
}
```

Environment variables:
- `OpenTelemetry__ServiceName`
- `OpenTelemetry__OtlpExporter__Endpoint`

## Local Testing

You can test the MCP server using curl. Here are some example commands:

1. Test authentication failure (401):
```bash
curl -i -X POST http://localhost:5000/mcp/stream
```

2. Test with invalid token (403):
```bash
curl -i -X POST http://localhost:5000/mcp/stream \
  -H "Authorization: Bearer invalid-token"
```

3. Test successful JSON-RPC call:
```bash
curl -i -X POST http://localhost:5000/mcp/stream \
  -H "Authorization: Bearer <valid-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "jsonrpc": "2.0",
    "method": "mcp.tool.transferStatus",
    "params": {
      "transferId": "123"
    },
    "id": 1
  }'
```
