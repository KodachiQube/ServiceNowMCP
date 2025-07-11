# ServiceNow MCP (Model Context Protocol) Server

An Azure Functions-based MCP server for interacting with ServiceNow instances. This server provides a standardized interface for managing incidents, change requests, users, and other ServiceNow resources through the Model Context Protocol.

## Architecture

The solution follows a clean architecture pattern with the following projects:

- **ServiceNow.Core**: Core domain models, interfaces, and constants
- **ServiceNow.Services**: Business logic and ServiceNow API client implementations
- **ServiceNow.Functions**: Azure Functions host and MCP server implementation
- **ServiceNow.Tests**: Unit and integration tests

## Features

- **Incident Management**: Create, update, and query incidents
- **Change Request Management**: Manage change requests and their lifecycle
- **User Management**: Search and manage ServiceNow users
- **Table Operations**: Generic CRUD operations on ServiceNow tables
- **MCP Protocol Support**: Full implementation of the Model Context Protocol

## Prerequisites

- .NET 8.0 SDK
- Azure Functions Core Tools v4
- ServiceNow instance with API access
- Valid ServiceNow credentials

## Configuration

1. Copy `src/ServiceNow.Functions/local.settings.json.example` to `local.settings.json`
2. Update the configuration values:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "ServiceNow:InstanceUrl": "https://your-instance.service-now.com",
    "ServiceNow:Username": "your-username",
    "ServiceNow:Password": "your-password"
  }
}
```

## Running the Server

### As an Azure Function

```bash
./scripts/run-azure-function.sh
```

### As an MCP Server

```bash
./scripts/run-mcp-server.sh
```

### Testing the MCP Server

```bash
./scripts/test-mcp-server.sh
```

## Development

### Building the Solution

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

### Project Structure

```
ServiceNowMCP/
├── src/
│   ├── ServiceNow.Core/           # Core domain layer
│   ├── ServiceNow.Services/       # Business logic layer
│   ├── ServiceNow.Functions/      # Azure Functions and MCP server
│   └── ServiceNow.Tests/          # Test projects
├── scripts/                       # Utility scripts
├── docs/                         # Documentation
└── ServiceNow.sln               # Solution file
```

## MCP Protocol Implementation

The server implements the following MCP capabilities:

- **Tools**: ServiceNow operations exposed as MCP tools
- **Resources**: ServiceNow data exposed as MCP resources
- **Prompts**: Pre-configured prompts for common operations

### Available Tools

- `create_incident`: Create a new incident
- `update_incident`: Update an existing incident
- `get_incident`: Retrieve incident details
- `create_change_request`: Create a new change request
- `search_users`: Search for ServiceNow users
- And more...

## Migration Notes

This project has been restructured from a single-project solution to follow the AzureFunctionMCPTemplate standards. The new structure provides:

- Better separation of concerns
- Improved testability
- Clear project boundaries
- Standard .NET solution patterns

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Run tests
5. Submit a pull request

## License

[Specify your license here]
