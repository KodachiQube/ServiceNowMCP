using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using ServiceNow.Core.Constants;
using ServiceNow.Core.Interfaces;
using ServiceNow.Core.Models;

namespace ServiceNow.Services.Services;

public class IncidentService : IIncidentService
{
    private readonly IServiceNowClient _client;
    private readonly ILogger<IncidentService> _logger;
    private const string TABLE_API = "now/table";

    public IncidentService(IServiceNowClient client, ILogger<IncidentService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<Incident> CreateIncidentAsync(JsonObject arguments)
    {
        _logger.LogInformation("Creating new incident");
        
        var incident = new
        {
            short_description = arguments["short_description"]?.ToString(),
            description = arguments["description"]?.ToString(),
            urgency = arguments["urgency"]?.GetValue<int>() ?? ServiceNowConstants.Impact.Medium,
            impact = arguments["impact"]?.GetValue<int>() ?? ServiceNowConstants.Impact.Medium,
            caller_id = arguments["caller_id"]?.ToString(),
            assignment_group = arguments["assignment_group"]?.ToString(),
            category = arguments["category"]?.ToString(),
            subcategory = arguments["subcategory"]?.ToString(),
            state = ServiceNowConstants.IncidentState.New
        };
        
        var result = await _client.PostAsync<Incident>($"{TABLE_API}/{ServiceNowConstants.IncidentTable}", incident);
        
        if (result == null)
            throw new InvalidOperationException("Failed to create incident");
        
        _logger.LogInformation($"Created incident: {result.Number}");
        return result;
    }

    public async Task<Incident> UpdateIncidentAsync(JsonObject arguments)
    {
        var sysId = arguments["sys_id"]?.ToString() 
            ?? throw new ArgumentException("sys_id is required");
        
        _logger.LogInformation($"Updating incident: {sysId}");
        
        var updates = new Dictionary<string, object>();
        
        if (arguments["state"] != null)
            updates["state"] = arguments["state"].GetValue<int>();
        
        if (arguments["work_notes"] != null)
            updates["work_notes"] = arguments["work_notes"].ToString();
        
        if (arguments["comments"] != null)
            updates["comments"] = arguments["comments"].ToString();
        
        if (arguments["resolution_code"] != null)
            updates["resolution_code"] = arguments["resolution_code"].ToString();
        
        if (arguments["resolution_notes"] != null)
            updates["resolution_notes"] = arguments["resolution_notes"].ToString();
        
        var result = await _client.PatchAsync<Incident>(
            $"{TABLE_API}/{ServiceNowConstants.IncidentTable}/{sysId}", 
            updates);
        
        if (result == null)
            throw new InvalidOperationException($"Failed to update incident {sysId}");
        
        _logger.LogInformation($"Updated incident: {result.Number}");
        return result;
    }

    public async Task<Incident> GetIncidentAsync(JsonObject arguments)
    {
        var identifier = arguments["identifier"]?.ToString() 
            ?? throw new ArgumentException("identifier is required");
        
        _logger.LogInformation($"Getting incident: {identifier}");
        
        if (identifier.Length == 32 && !identifier.StartsWith("INC"))
        {
            var result = await _client.GetAsync<Incident>(
                $"{TABLE_API}/{ServiceNowConstants.IncidentTable}/{identifier}");
            
            if (result == null)
                throw new InvalidOperationException($"Incident not found: {identifier}");
            
            return result;
        }
        else
        {
            var queryParams = new Dictionary<string, string>
            {
                ["sysparm_query"] = $"number={identifier}",
                ["sysparm_limit"] = "1"
            };
            
            var results = await _client.GetAsync<List<Incident>>(
                $"{TABLE_API}/{ServiceNowConstants.IncidentTable}", 
                queryParams);
            
            if (results == null || results.Count == 0)
                throw new InvalidOperationException($"Incident not found: {identifier}");
            
            return results[0];
        }
    }

    public async Task<List<Incident>> SearchIncidentsAsync(JsonObject arguments)
    {
        _logger.LogInformation("Searching incidents");
        
        var queryParams = new Dictionary<string, string>();
        
        if (arguments["query"] != null)
            queryParams["sysparm_query"] = arguments["query"].ToString()!;
        
        queryParams["sysparm_limit"] = arguments["limit"]?.GetValue<int>().ToString() ?? "10";
        queryParams["sysparm_offset"] = arguments["offset"]?.GetValue<int>().ToString() ?? "0";
        
        if (arguments["order_by"] != null)
        {
            var orderBy = arguments["order_by"].ToString();
            var orderDesc = arguments["order_desc"]?.GetValue<bool>() ?? false;
            queryParams["sysparm_query"] += $"^ORDERBY{(orderDesc ? "DESC" : "")}{orderBy}";
        }
        
        var results = await _client.GetAsync<List<Incident>>(
            $"{TABLE_API}/{ServiceNowConstants.IncidentTable}", 
            queryParams);
        
        return results ?? new List<Incident>();
    }

    public async Task<List<Incident>> GetActiveIncidentsAsync()
    {
        _logger.LogInformation("Getting active incidents");
        
        var queryParams = new Dictionary<string, string>
        {
            ["sysparm_query"] = $"stateNOT IN{ServiceNowConstants.IncidentState.Resolved},{ServiceNowConstants.IncidentState.Closed}",
            ["sysparm_limit"] = "100"
        };
        
        var results = await _client.GetAsync<List<Incident>>(
            $"{TABLE_API}/{ServiceNowConstants.IncidentTable}", 
            queryParams);
        
        return results ?? new List<Incident>();
    }
}
