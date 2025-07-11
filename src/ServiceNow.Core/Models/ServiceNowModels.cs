using System.Text.Json.Serialization;

namespace ServiceNow.Core.Models;

// Base ServiceNow record
public class ServiceNowRecord
{
    [JsonPropertyName("sys_id")]
    public string? SysId { get; set; }
    
    [JsonPropertyName("sys_created_on")]
    public DateTime? CreatedOn { get; set; }
    
    [JsonPropertyName("sys_created_by")]
    public string? CreatedBy { get; set; }
    
    [JsonPropertyName("sys_updated_on")]
    public DateTime? UpdatedOn { get; set; }
    
    [JsonPropertyName("sys_updated_by")]
    public string? UpdatedBy { get; set; }
}

// Incident model
public class Incident : ServiceNowRecord
{
    [JsonPropertyName("number")]
    public string? Number { get; set; }
    
    [JsonPropertyName("short_description")]
    public string? ShortDescription { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("state")]
    public int? State { get; set; }
    
    [JsonPropertyName("urgency")]
    public int? Urgency { get; set; }
    
    [JsonPropertyName("impact")]
    public int? Impact { get; set; }
    
    [JsonPropertyName("priority")]
    public int? Priority { get; set; }
    
    [JsonPropertyName("caller_id")]
    public string? CallerId { get; set; }
    
    [JsonPropertyName("assignment_group")]
    public string? AssignmentGroup { get; set; }
    
    [JsonPropertyName("assigned_to")]
    public string? AssignedTo { get; set; }
    
    [JsonPropertyName("category")]
    public string? Category { get; set; }
    
    [JsonPropertyName("subcategory")]
    public string? Subcategory { get; set; }
}

// Change Request model
public class ChangeRequest : ServiceNowRecord
{
    [JsonPropertyName("number")]
    public string? Number { get; set; }
    
    [JsonPropertyName("short_description")]
    public string? ShortDescription { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("state")]
    public int? State { get; set; }
    
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    
    [JsonPropertyName("category")]
    public string? Category { get; set; }
    
    [JsonPropertyName("priority")]
    public int? Priority { get; set; }
    
    [JsonPropertyName("risk")]
    public int? Risk { get; set; }
    
    [JsonPropertyName("impact")]
    public int? Impact { get; set; }
    
    [JsonPropertyName("assignment_group")]
    public string? AssignmentGroup { get; set; }
}

// User model
public class User : ServiceNowRecord
{
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }
    
    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }
    
    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }
    
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    
    [JsonPropertyName("department")]
    public string? Department { get; set; }
    
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    
    [JsonPropertyName("active")]
    public bool? Active { get; set; }
}

// Configuration Item model
public class ConfigurationItem : ServiceNowRecord
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("serial_number")]
    public string? SerialNumber { get; set; }
    
    [JsonPropertyName("asset_tag")]
    public string? AssetTag { get; set; }
    
    [JsonPropertyName("manufacturer")]
    public string? Manufacturer { get; set; }
    
    [JsonPropertyName("model_id")]
    public string? ModelId { get; set; }
    
    [JsonPropertyName("location")]
    public string? Location { get; set; }
}

// API Response wrappers
public class ServiceNowResponse<T>
{
    [JsonPropertyName("result")]
    public T? Result { get; set; }
}

public class ServiceNowListResponse<T>
{
    [JsonPropertyName("result")]
    public List<T> Result { get; set; } = new();
}

public class ServiceNowError
{
    [JsonPropertyName("error")]
    public ErrorDetail Error { get; set; } = new();
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = "";
}

public class ErrorDetail
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = "";
    
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }
}
