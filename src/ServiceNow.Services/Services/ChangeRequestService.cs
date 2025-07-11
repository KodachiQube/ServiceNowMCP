using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using ServiceNow.Core.Constants;
using ServiceNow.Core.Interfaces;
using ServiceNow.Core.Models;

namespace ServiceNow.Services.Services;

public class ChangeRequestService : IChangeRequestService
{
    private readonly IServiceNowClient _client;
    private readonly ILogger<ChangeRequestService> _logger;
    private const string TABLE_API = "now/table";

    public ChangeRequestService(IServiceNowClient client, ILogger<ChangeRequestService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<ChangeRequest> CreateChangeRequestAsync(JsonObject arguments)
    {
        _logger.LogInformation("Creating change request");
        
        var change = new
        {
            short_description = arguments["short_description"]?.ToString(),
            description = arguments["description"]?.ToString(),
            type = arguments["type"]?.ToString() ?? "normal",
            category = arguments["category"]?.ToString(),
            priority = arguments["priority"]?.GetValue<int>() ?? ServiceNowConstants.Priority.Moderate,
            risk = arguments["risk"]?.GetValue<int>() ?? ServiceNowConstants.Impact.Medium,
            impact = arguments["impact"]?.GetValue<int>() ?? ServiceNowConstants.Impact.Medium,
            assignment_group = arguments["assignment_group"]?.ToString(),
            start_date = arguments["start_date"]?.ToString(),
            end_date = arguments["end_date"]?.ToString(),
            state = ServiceNowConstants.ChangeState.New
        };
        
        // Mock implementation
        await Task.Delay(150);
        return new ChangeRequest
        {
            SysId = Guid.NewGuid().ToString("N"),
            Number = $"CHG{DateTime.Now:yyyyMMddHHmmss}",
            ShortDescription = change.short_description,
            Type = change.type,
            State = ServiceNowConstants.ChangeState.New,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = "api_user"
        };
    }

    public async Task<ChangeRequest> UpdateChangeRequestAsync(JsonObject arguments)
    {
        var sysId = arguments["sys_id"]?.ToString() 
            ?? throw new ArgumentException("sys_id is required");
        
        _logger.LogInformation($"Updating change request: {sysId}");
        
        // Mock implementation
        await Task.Delay(150);
        return new ChangeRequest
        {
            SysId = sysId,
            Number = "CHG0001234",
            ShortDescription = "Updated change request",
            State = arguments["state"]?.GetValue<int>() ?? ServiceNowConstants.ChangeState.Assess,
            UpdatedOn = DateTime.UtcNow,
            UpdatedBy = "api_user"
        };
    }

    public async Task<ChangeRequest> GetChangeRequestAsync(JsonObject arguments)
    {
        var identifier = arguments["identifier"]?.ToString() 
            ?? throw new ArgumentException("identifier is required");
        
        _logger.LogInformation($"Getting change request: {identifier}");
        
        // Mock implementation
        await Task.Delay(100);
        return new ChangeRequest
        {
            SysId = identifier.Length == 32 ? identifier : "mock_change_001",
            Number = identifier.StartsWith("CHG") ? identifier : "CHG0001234",
            ShortDescription = "Mock change request",
            State = ServiceNowConstants.ChangeState.Scheduled,
            Type = "normal",
            CreatedOn = DateTime.UtcNow.AddDays(-1),
            CreatedBy = "admin"
        };
    }

    public async Task<List<ChangeRequest>> SearchChangeRequestsAsync(JsonObject arguments)
    {
        _logger.LogInformation("Searching change requests");
        
        // Mock implementation
        await Task.Delay(100);
        return new List<ChangeRequest>
        {
            new ChangeRequest
            {
                SysId = "mock_change_001",
                Number = "CHG0001234",
                ShortDescription = "Upgrade database server",
                State = ServiceNowConstants.ChangeState.Scheduled,
                Type = "normal"
            },
            new ChangeRequest
            {
                SysId = "mock_change_002",
                Number = "CHG0001235",
                ShortDescription = "Deploy application update",
                State = ServiceNowConstants.ChangeState.Assess,
                Type = "standard"
            }
        };
    }

    public async Task<List<ChangeRequest>> GetScheduledChangesAsync()
    {
        _logger.LogInformation("Getting scheduled changes");
        
        var queryParams = new Dictionary<string, string>
        {
            ["sysparm_query"] = $"state={ServiceNowConstants.ChangeState.Scheduled}",
            ["sysparm_limit"] = "100"
        };
        
        // Mock implementation
        await Task.Delay(100);
        return new List<ChangeRequest>
        {
            new ChangeRequest
            {
                SysId = "mock_scheduled_001",
                Number = "CHG0001236",
                ShortDescription = "Scheduled maintenance window",
                State = ServiceNowConstants.ChangeState.Scheduled,
                Type = "standard"
            }
        };
    }
}
