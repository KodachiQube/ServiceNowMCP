#!/bin/bash

# Navigate to the Functions project directory
cd "$(dirname "$0")/../src/ServiceNow.Functions" || exit

echo "Starting ServiceNow MCP Server..."
echo "================================"

# Build the project first
echo "Building the project..."
dotnet build

if [ $? -ne 0 ]; then
    echo "Build failed!"
    exit 1
fi

echo ""
echo "Starting MCP server..."
echo "Server will listen on stdio for MCP protocol messages"
echo ""

# Run the MCP server
dotnet run --project ServiceNow.Functions.csproj -- --mcp-server

# Keep the script running
wait