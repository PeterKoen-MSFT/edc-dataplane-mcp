using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StreamJsonRpc;

namespace DataPlane.McpServer.Controllers;

[ApiController]
[Route("mcp")]
[Authorize]
public class McpController : ControllerBase
{
    private readonly ILogger<McpController> _logger;
    private readonly Services.IMcpToolService _mcpService;

    public McpController(ILogger<McpController> logger, Services.IMcpToolService mcpService)
    {
        _logger = logger;
        _mcpService = mcpService;
    }

    [HttpPost("stream")]
    public IActionResult HandleMcpRequest()
    {
        // Set up JSON-RPC handler
        var messageHandler = new HeaderDelimitedMessageHandler(Request.Body, Response.Body);
        var rpc = new JsonRpc(messageHandler);
        
        rpc.AddLocalRpcTarget(_mcpService, new JsonRpcTargetOptions 
        { 
            MethodNameTransform = name => $"mcp.tool.{name.ToLowerFirst()}" 
        });

        try
        {
            rpc.StartListening();
            return new EmptyResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing MCP request");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}
