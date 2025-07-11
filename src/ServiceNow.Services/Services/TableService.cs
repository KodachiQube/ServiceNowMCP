using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using ServiceNow.Core.Interfaces;

namespace ServiceNow.Services.Services;

public class TableService : ITableService
{
    private readonly IServiceNowClient _client;
    private readonly ILogger<TableService> _logger;
    private const string TABLE_API = "now/table";

    public TableService(IServiceNowClient client, ILogger<TableService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<object> QueryTableAsync(JsonObject arguments)
    {
        var tableName = arguments["table"]?.ToString() 
            ?? throw new ArgumentException("table is required");
        
        _logger.LogInformation($"Querying table: {tableName}");
        
        var queryParams = new Dictionary<string, string>();
        
        if (arguments["query"] != null)
            queryParams["sysparm_query"] = arguments["query"].ToString()!;
        
        if (arguments["fields"] != null && arguments["fields"] is JsonArray fieldsArray)
        {
            var fields = fieldsArray.Select(f => f?.ToString()).Where(f => f != null);
            queryParams["sysparm_fields"] = string.Join(",", fields);
        }
        
        queryParams["sysparm_limit"] = arguments["limit"]?.GetValue<int>().ToString() ?? "10";
        queryParams["sysparm_offset"] = arguments["offset"]?.GetValue<int>().ToString() ?? "0";
        
        // Mock response
        await Task.Delay(100);
        return new
        {
            result = new[]
            {
                new { sys_id = "mock_001", name = "Mock Record 1", created_on = DateTime.UtcNow.AddDays(-1) },
                new { sys_id = "mock_002", name = "Mock Record 2", created_on = DateTime.UtcNow.AddDays(-2) }
            }
        };
    }

    public async Task<object> CreateRecordAsync(JsonObject arguments)
    {
        var tableName = arguments["table"]?.ToString() 
            ?? throw new ArgumentException("table is required");
        
        var data = arguments["data"] 
            ?? throw new ArgumentException("data is required");
        
        _logger.LogInformation($"Creating record in table: {tableName}");
        
        // Mock response
        await Task.Delay(150);
        return new
        {
            result = new
            {
                sys_id = Guid.NewGuid().ToString("N"),
                sys_created_on = DateTime.UtcNow,
                sys_created_by = "api_user"
            }
        };
    }

    public async Task<object> UpdateRecordAsync(JsonObject arguments)
    {
        var tableName = arguments["table"]?.ToString() 
            ?? throw new ArgumentException("table is required");
        
        var sysId = arguments["sys_id"]?.ToString() 
            ?? throw new ArgumentException("sys_id is required");
        
        var data = arguments["data"] 
            ?? throw new ArgumentException("data is required");
        
        _logger.LogInformation($"Updating record {sysId} in table: {tableName}");
        
        // Mock response
        await Task.Delay(150);
        return new
        {
            result = new
            {
                sys_id = sysId,
                sys_updated_on = DateTime.UtcNow,
                sys_updated_by = "api_user"
            }
        };
    }

    public async Task<object> DeleteRecordAsync(JsonObject arguments)
    {
        var tableName = arguments["table"]?.ToString() 
            ?? throw new ArgumentException("table is required");
        
        var sysId = arguments["sys_id"]?.ToString() 
            ?? throw new ArgumentException("sys_id is required");
        
        _logger.LogInformation($"Deleting record {sysId} from table: {tableName}");
        
        var success = await _client.DeleteAsync($"{TABLE_API}/{tableName}/{sysId}");
        
        return new { success, sys_id = sysId, table = tableName };
    }
}
