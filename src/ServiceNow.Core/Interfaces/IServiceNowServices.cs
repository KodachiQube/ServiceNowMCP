using System.Text.Json.Nodes;
using ServiceNow.Core.Models;

namespace ServiceNow.Core.Interfaces;

public interface IServiceNowClient
{
    Task<T?> GetAsync<T>(string endpoint, Dictionary<string, string>? queryParams = null);
    Task<T?> PostAsync<T>(string endpoint, object data);
    Task<T?> PutAsync<T>(string endpoint, object data);
    Task<T?> PatchAsync<T>(string endpoint, object data);
    Task<bool> DeleteAsync(string endpoint);
    Task<byte[]> DownloadFileAsync(string endpoint);
    Task<T?> UploadFileAsync<T>(string endpoint, string fileName, byte[] content, string contentType);
}

public interface ITableService
{
    Task<object> QueryTableAsync(JsonObject arguments);
    Task<object> CreateRecordAsync(JsonObject arguments);
    Task<object> UpdateRecordAsync(JsonObject arguments);
    Task<object> DeleteRecordAsync(JsonObject arguments);
}

public interface IIncidentService
{
    Task<Incident> CreateIncidentAsync(JsonObject arguments);
    Task<Incident> UpdateIncidentAsync(JsonObject arguments);
    Task<Incident> GetIncidentAsync(JsonObject arguments);
    Task<List<Incident>> SearchIncidentsAsync(JsonObject arguments);
    Task<List<Incident>> GetActiveIncidentsAsync();
}

public interface IChangeRequestService
{
    Task<ChangeRequest> CreateChangeRequestAsync(JsonObject arguments);
    Task<ChangeRequest> UpdateChangeRequestAsync(JsonObject arguments);
    Task<ChangeRequest> GetChangeRequestAsync(JsonObject arguments);
    Task<List<ChangeRequest>> SearchChangeRequestsAsync(JsonObject arguments);
    Task<List<ChangeRequest>> GetScheduledChangesAsync();
}

public interface IUserService
{
    Task<User> GetUserAsync(JsonObject arguments);
    Task<List<User>> SearchUsersAsync(JsonObject arguments);
    Task<User> CreateUserAsync(JsonObject arguments);
    Task<User> UpdateUserAsync(JsonObject arguments);
}

public interface ICMDBService
{
    Task<ConfigurationItem> GetConfigurationItemAsync(JsonObject arguments);
    Task<List<ConfigurationItem>> SearchConfigurationItemsAsync(JsonObject arguments);
    Task<ConfigurationItem> CreateConfigurationItemAsync(JsonObject arguments);
    Task<object> GetRelationshipsAsync(JsonObject arguments);
    Task<List<ConfigurationItem>> GetServersAsync();
}

public interface IAttachmentService
{
    Task<object> UploadAttachmentAsync(JsonObject arguments);
    Task<object> DownloadAttachmentAsync(JsonObject arguments);
    Task<List<object>> ListAttachmentsAsync(JsonObject arguments);
}

public interface IKnowledgeService
{
    Task<List<object>> SearchArticlesAsync(JsonObject arguments);
    Task<object> GetArticleAsync(JsonObject arguments);
}

public interface ICatalogService
{
    Task<List<object>> GetCatalogItemsAsync(JsonObject arguments);
    Task<object> RequestCatalogItemAsync(JsonObject arguments);
    Task<object> GetRequestStatusAsync(JsonObject arguments);
}

public interface IAggregateService
{
    Task<object> GetAggregateStatsAsync(JsonObject arguments);
}

public interface IImportSetService
{
    Task<object> CreateImportSetAsync(JsonObject arguments);
}
