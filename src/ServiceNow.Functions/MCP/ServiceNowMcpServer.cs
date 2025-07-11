using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ServiceNow.Core.Interfaces;
using ServiceNow.Core.Models;
using System.Text.Json;
using System.Text.Json.Nodes;
using StreamJsonRpc;
using System.IO;

namespace ServiceNow.Functions.MCP
{
    public class ServiceNowMcpServer
    {
        private readonly ILogger<ServiceNowMcpServer> _logger;
        private readonly IIncidentService _incidentService;
        private readonly IChangeRequestService _changeRequestService;
        private readonly IUserService _userService;
        private readonly ITableService _tableService;
        private JsonRpc? _rpc;

        public ServiceNowMcpServer(
            ILogger<ServiceNowMcpServer> logger,
            IIncidentService incidentService,
            IChangeRequestService changeRequestService,
            IUserService userService,
            ITableService tableService)
        {
            _logger = logger;
            _incidentService = incidentService;
            _changeRequestService = changeRequestService;
            _userService = userService;
            _tableService = tableService;
        }

        public async Task StartAsync(Stream input, Stream output)
        {
            _rpc = JsonRpc.Attach(output, input, this);
            _rpc.StartListening();
            await _rpc.Completion;
        }

        [JsonRpcMethod("initialize")]
        public Task<object> InitializeAsync(object @params)
        {
            _logger.LogInformation("MCP Initialize called");
            
            return Task.FromResult<object>(new
            {
                protocolVersion = "1.0",
                capabilities = new
                {
                    tools = new
                    {
                        listChanged = true
                    },
                    resources = new
                    {
                        subscribe = true,
                        listChanged = true
                    }
                },
                serverInfo = new
                {
                    name = "servicenow-mcp",
                    version = "1.0.0"
                }
            });
        }

        [JsonRpcMethod("tools/list")]
        public Task<object> ListToolsAsync()
        {
            var tools = new object[]
            {
                new
                {
                    name = "create_incident",
                    description = "Create a new incident in ServiceNow",
                    inputSchema = new
                    {
                        type = "object",
                        properties = new
                        {
                            short_description = new { type = "string", description = "Brief description of the incident" },
                            description = new { type = "string", description = "Detailed description of the incident" },
                            urgency = new { type = "string", description = "Urgency level (1-3)", @enum = new[] { "1", "2", "3" } },
                            impact = new { type = "string", description = "Impact level (1-3)", @enum = new[] { "1", "2", "3" } },
                            assignment_group = new { type = "string", description = "Group to assign the incident to" }
                        },
                        required = new[] { "short_description" }
                    }
                },
                new
                {
                    name = "update_incident",
                    description = "Update an existing incident",
                    inputSchema = new
                    {
                        type = "object",
                        properties = new
                        {
                            sys_id = new { type = "string", description = "System ID of the incident" },
                            state = new { type = "string", description = "Incident state" },
                            work_notes = new { type = "string", description = "Work notes to add" },
                            close_notes = new { type = "string", description = "Resolution notes" }
                        },
                        required = new[] { "sys_id" }
                    }
                },
                new
                {
                    name = "get_incident",
                    description = "Get incident details by number or sys_id",
                    inputSchema = new
                    {
                        type = "object",
                        properties = new
                        {
                            identifier = new { type = "string", description = "Incident number or sys_id" }
                        },
                        required = new[] { "identifier" }
                    }
                },
                new
                {
                    name = "search_users",
                    description = "Search for ServiceNow users",
                    inputSchema = new
                    {
                        type = "object",
                        properties = new
                        {
                            query = new { type = "string", description = "Search query for users" }
                        },
                        required = new[] { "query" }
                    }
                }
            };

            return Task.FromResult<object>(new { tools });
        }

        [JsonRpcMethod("tools/call")]
        public async Task<object> CallToolAsync(JsonObject @params)
        {
            var name = @params["name"]?.ToString();
            var arguments = @params["arguments"]?.AsObject();

            _logger.LogInformation($"Tool call: {name}");

            try
            {
                switch (name)
                {
                    case "create_incident":
                        return await CreateIncidentAsync(arguments);
                    case "update_incident":
                        return await UpdateIncidentAsync(arguments);
                    case "get_incident":
                        return await GetIncidentAsync(arguments);
                    case "search_users":
                        return await SearchUsersAsync(arguments);
                    default:
                        throw new ArgumentException($"Unknown tool: {name}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling tool {name}");
                return new
                {
                    content = new[]
                    {
                        new
                        {
                            type = "text",
                            text = $"Error: {ex.Message}"
                        }
                    },
                    isError = true
                };
            }
        }

        private async Task<object> CreateIncidentAsync(JsonObject? arguments)
        {
            if (arguments == null)
                throw new ArgumentException("Arguments required");

            var incident = await _incidentService.CreateIncidentAsync(arguments);

            return new
            {
                content = new[]
                {
                    new
                    {
                        type = "text",
                        text = $"Created incident {incident.Number} (sys_id: {incident.SysId})"
                    }
                }
            };
        }

        private async Task<object> UpdateIncidentAsync(JsonObject? arguments)
        {
            if (arguments == null)
                throw new ArgumentException("Arguments required");

            var incident = await _incidentService.UpdateIncidentAsync(arguments);

            return new
            {
                content = new[]
                {
                    new
                    {
                        type = "text",
                        text = $"Updated incident {incident.Number}"
                    }
                }
            };
        }

        private async Task<object> GetIncidentAsync(JsonObject? arguments)
        {
            if (arguments == null)
                throw new ArgumentException("Arguments required");

            var incident = await _incidentService.GetIncidentAsync(arguments);

            return new
            {
                content = new[]
                {
                    new
                    {
                        type = "text",
                        text = JsonSerializer.Serialize(incident, new JsonSerializerOptions { WriteIndented = true })
                    }
                }
            };
        }

        private async Task<object> SearchUsersAsync(JsonObject? arguments)
        {
            if (arguments == null)
                throw new ArgumentException("Arguments required");

            var users = await _userService.SearchUsersAsync(arguments);

            return new
            {
                content = new[]
                {
                    new
                    {
                        type = "text",
                        text = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true })
                    }
                }
            };
        }

        [JsonRpcMethod("resources/list")]
        public Task<object> ListResourcesAsync()
        {
            var resources = new[]
            {
                new
                {
                    uri = "servicenow://incidents/recent",
                    name = "Recent Incidents",
                    description = "List of recent incidents",
                    mimeType = "application/json"
                },
                new
                {
                    uri = "servicenow://changes/pending",
                    name = "Pending Changes",
                    description = "List of pending change requests",
                    mimeType = "application/json"
                }
            };

            return Task.FromResult<object>(new { resources });
        }

        [JsonRpcMethod("resources/read")]
        public async Task<object> ReadResourceAsync(JsonObject @params)
        {
            var uri = @params["uri"]?.ToString();

            switch (uri)
            {
                case "servicenow://incidents/recent":
                    var incidentParams = new JsonObject
                    {
                        ["sysparm_limit"] = "10",
                        ["sysparm_orderby"] = "sys_created_on DESC"
                    };
                    var incidents = await _incidentService.SearchIncidentsAsync(incidentParams);
                    
                    return new
                    {
                        contents = new[]
                        {
                            new
                            {
                                uri = uri,
                                mimeType = "application/json",
                                text = JsonSerializer.Serialize(incidents, new JsonSerializerOptions { WriteIndented = true })
                            }
                        }
                    };

                case "servicenow://changes/pending":
                    var changeParams = new JsonObject
                    {
                        ["state"] = "assess",
                        ["sysparm_limit"] = "10"
                    };
                    var changes = await _changeRequestService.SearchChangeRequestsAsync(changeParams);
                    
                    return new
                    {
                        contents = new[]
                        {
                            new
                            {
                                uri = uri,
                                mimeType = "application/json",
                                text = JsonSerializer.Serialize(changes, new JsonSerializerOptions { WriteIndented = true })
                            }
                        }
                    };

                default:
                    throw new ArgumentException($"Unknown resource: {uri}");
            }
        }

        [JsonRpcMethod("notifications/initialized")]
        public void NotificationsInitialized()
        {
            _logger.LogInformation("Notifications initialized");
        }
    }
}
