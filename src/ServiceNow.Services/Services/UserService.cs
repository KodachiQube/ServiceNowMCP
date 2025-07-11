using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using ServiceNow.Core.Constants;
using ServiceNow.Core.Interfaces;
using ServiceNow.Core.Models;

namespace ServiceNow.Services.Services;

public class UserService : IUserService
{
    private readonly IServiceNowClient _client;
    private readonly ILogger<UserService> _logger;
    private const string TABLE_API = "now/table";

    public UserService(IServiceNowClient client, ILogger<UserService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<User> GetUserAsync(JsonObject arguments)
    {
        var identifier = arguments["identifier"]?.ToString() 
            ?? throw new ArgumentException("identifier is required");
        
        _logger.LogInformation($"Getting user: {identifier}");
        
        // Mock implementation
        await Task.Delay(100);
        return new User
        {
            SysId = "mock_user_001",
            UserName = identifier.Contains("@") ? identifier.Split('@')[0] : identifier,
            FirstName = "John",
            LastName = "Doe",
            Email = identifier.Contains("@") ? identifier : $"{identifier}@example.com",
            Department = "IT",
            Title = "Developer",
            Active = true
        };
    }

    public async Task<List<User>> SearchUsersAsync(JsonObject arguments)
    {
        _logger.LogInformation("Searching users");
        
        // Mock implementation
        await Task.Delay(100);
        return new List<User>
        {
            new User
            {
                SysId = "mock_user_001",
                UserName = "john.doe",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Department = arguments["department"]?.ToString() ?? "IT",
                Active = true
            },
            new User
            {
                SysId = "mock_user_002",
                UserName = "jane.smith",
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Department = arguments["department"]?.ToString() ?? "HR",
                Active = true
            }
        };
    }

    public async Task<User> CreateUserAsync(JsonObject arguments)
    {
        _logger.LogInformation("Creating user");
        
        // Mock implementation
        await Task.Delay(150);
        return new User
        {
            SysId = Guid.NewGuid().ToString("N"),
            UserName = arguments["user_name"]?.ToString() ?? "new.user",
            FirstName = arguments["first_name"]?.ToString() ?? "New",
            LastName = arguments["last_name"]?.ToString() ?? "User",
            Email = arguments["email"]?.ToString() ?? "new.user@example.com",
            Department = arguments["department"]?.ToString(),
            Title = arguments["title"]?.ToString(),
            Active = true,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = "api_user"
        };
    }

    public async Task<User> UpdateUserAsync(JsonObject arguments)
    {
        var sysId = arguments["sys_id"]?.ToString() 
            ?? throw new ArgumentException("sys_id is required");
        
        _logger.LogInformation($"Updating user: {sysId}");
        
        // Mock implementation
        await Task.Delay(150);
        return new User
        {
            SysId = sysId,
            UserName = "updated.user",
            FirstName = "Updated",
            LastName = "User",
            Email = "updated.user@example.com",
            UpdatedOn = DateTime.UtcNow,
            UpdatedBy = "api_user"
        };
    }
}
