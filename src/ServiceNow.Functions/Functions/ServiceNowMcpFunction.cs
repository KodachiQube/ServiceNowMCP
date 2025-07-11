using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ServiceNow.Functions.MCP;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ServiceNow.Functions.Functions
{
    public class ServiceNowMcpFunction
    {
        private readonly ILogger<ServiceNowMcpFunction> _logger;
        private readonly ServiceNowMcpServer _mcpServer;

        public ServiceNowMcpFunction(ILogger<ServiceNowMcpFunction> logger, ServiceNowMcpServer mcpServer)
        {
            _logger = logger;
            _mcpServer = mcpServer;
        }

        [Function("ServiceNowMcp")]
        public async Task<HttpResponseData> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestData req)
        {
            _logger.LogInformation("ServiceNow MCP function triggered");

            try
            {
                // For HTTP-based MCP, we would need to implement a different approach
                // This is a placeholder for the HTTP trigger
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/json");
                await response.WriteStringAsync("{\"status\":\"MCP server is running\"}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing MCP request");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error: {ex.Message}");
                return errorResponse;
            }
        }
    }
}
