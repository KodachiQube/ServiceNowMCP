using ServiceNow.Services.Configuration;using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceNow.Core.Constants;
using ServiceNow.Core.Interfaces;
using ServiceNow.Core.Models;
using Polly;

namespace ServiceNow.Services.Clients;

public class ServiceNowClient : IServiceNowClient
{
    private readonly HttpClient _httpClient;
    private readonly ServiceNowConfiguration _config;
    private readonly ILogger<ServiceNowClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;

    public ServiceNowClient(
        HttpClient httpClient,
        IOptions<ServiceNowConfiguration> options,
        ILogger<ServiceNowClient> logger)
    {
        _httpClient = httpClient;
        _config = options.Value;
        _logger = logger;
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        _retryPolicy = HttpPolicies.GetRetryPolicy();
        ConfigureHttpClient();
    }

    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri($"{_config.InstanceUrl}/api/");
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        
        // Basic authentication
        var authString = $"{_config.Username}:{_config.Password}";
        var base64Auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(authString));
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Basic", base64Auth);
        
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<T?> GetAsync<T>(string endpoint, Dictionary<string, string>? queryParams = null)
    {
        try
        {
            var url = BuildUrl(endpoint, queryParams);
            _logger.LogDebug($"GET request to: {url}");
            
            // MOCK: Return mock data instead of real API call
            await Task.Delay(100); // Simulate network delay
            return GetMockData<T>(endpoint, queryParams);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in GET request to {endpoint}");
            throw;
        }
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        try
        {
            _logger.LogDebug($"POST request to: {endpoint}");
            
            // MOCK: Return mock data instead of real API call
            await Task.Delay(150); // Simulate network delay
            return CreateMockData<T>(endpoint, data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in POST request to {endpoint}");
            throw;
        }
    }

    public async Task<T?> PutAsync<T>(string endpoint, object data)
    {
        return await PatchAsync<T>(endpoint, data);
    }

    public async Task<T?> PatchAsync<T>(string endpoint, object data)
    {
        try
        {
            _logger.LogDebug($"PATCH request to: {endpoint}");
            
            // MOCK: Return mock data instead of real API call
            await Task.Delay(150); // Simulate network delay
            return UpdateMockData<T>(endpoint, data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in PATCH request to {endpoint}");
            throw;
        }
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            _logger.LogDebug($"DELETE request to: {endpoint}");
            
            // MOCK: Simulate successful deletion
            await Task.Delay(100);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in DELETE request to {endpoint}");
            throw;
        }
    }

    public async Task<byte[]> DownloadFileAsync(string endpoint)
    {
        try
        {
            _logger.LogDebug($"Downloading file from: {endpoint}");
            
            // MOCK: Return dummy file content
            await Task.Delay(200);
            return Encoding.UTF8.GetBytes("Mock file content");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error downloading file from {endpoint}");
            throw;
        }
    }

    public async Task<T?> UploadFileAsync<T>(string endpoint, string fileName, byte[] content, string contentType)
    {
        try
        {
            _logger.LogDebug($"Uploading file to: {endpoint}");
            
            // MOCK: Return mock attachment data
            await Task.Delay(200);
            return GetMockData<T>(endpoint, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error uploading file to {endpoint}");
            throw;
        }
    }

    private string BuildUrl(string endpoint, Dictionary<string, string>? queryParams)
    {
        if (queryParams == null || queryParams.Count == 0)
            return endpoint;
        
        var query = HttpUtility.ParseQueryString(string.Empty);
        foreach (var param in queryParams)
        {
            query[param.Key] = param.Value;
        }
        
        return $"{endpoint}?{query}";
    }

    // Mock data generation methods
    private T? GetMockData<T>(string endpoint, Dictionary<string, string>? queryParams)
    {
        var type = typeof(T);
        
        if (type == typeof(List<Incident>))
        {
            var incidents = new List<Incident>
            {
                new Incident
                {
                    SysId = "mock_incident_001",
                    Number = "INC0001234",
                    ShortDescription = "Mock incident - Server down",
                    Description = "Production server is not responding",
                    State = ServiceNowConstants.IncidentState.New,
                    Urgency = ServiceNowConstants.Impact.High,
                    Impact = ServiceNowConstants.Impact.High,
                    Priority = ServiceNowConstants.Priority.Critical,
                    CreatedOn = DateTime.UtcNow.AddHours(-2),
                    CreatedBy = "admin"
                },
                new Incident
                {
                    SysId = "mock_incident_002",
                    Number = "INC0001235",
                    ShortDescription = "Mock incident - Application error",
                    Description = "Users reporting 500 errors",
                    State = ServiceNowConstants.IncidentState.InProgress,
                    Urgency = ServiceNowConstants.Impact.Medium,
                    Impact = ServiceNowConstants.Impact.Medium,
                    Priority = ServiceNowConstants.Priority.High,
                    CreatedOn = DateTime.UtcNow.AddHours(-1),
                    CreatedBy = "user1"
                }
            };
            return (T)(object)incidents;
        }
        
        if (type == typeof(Incident))
        {
            var incident = new Incident
            {
                SysId = endpoint.Contains("/") ? endpoint.Split('/').Last() : "mock_incident_001",
                Number = "INC0001234",
                ShortDescription = "Mock incident - Retrieved",
                State = ServiceNowConstants.IncidentState.New,
                CreatedOn = DateTime.UtcNow.AddHours(-1),
                CreatedBy = "admin"
            };
            return (T)(object)incident;
        }
        
        if (type == typeof(List<User>))
        {
            var users = new List<User>
            {
                new User
                {
                    SysId = "mock_user_001",
                    UserName = "john.doe",
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    Department = "IT",
                    Active = true
                }
            };
            return (T)(object)users;
        }
        
        if (type == typeof(User))
        {
            var user = new User
            {
                SysId = "mock_user_001",
                UserName = "john.doe",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Active = true
            };
            return (T)(object)user;
        }
        
        return default;
    }

    private T? CreateMockData<T>(string endpoint, object data)
    {
        var type = typeof(T);
        
        if (type == typeof(Incident))
        {
            var incident = new Incident
            {
                SysId = Guid.NewGuid().ToString("N"),
                Number = $"INC{DateTime.Now:yyyyMMddHHmmss}",
                ShortDescription = "Mock created incident",
                State = ServiceNowConstants.IncidentState.New,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "api_user"
            };
            return (T)(object)incident;
        }
        
        return default;
    }

    private T? UpdateMockData<T>(string endpoint, object data)
    {
        var type = typeof(T);
        
        if (type == typeof(Incident))
        {
            var incident = new Incident
            {
                SysId = endpoint.Split('/').Last(),
                Number = "INC0001234",
                ShortDescription = "Mock updated incident",
                State = ServiceNowConstants.IncidentState.InProgress,
                UpdatedOn = DateTime.UtcNow,
                UpdatedBy = "api_user"
            };
            return (T)(object)incident;
        }
        
        return default;
    }
}
