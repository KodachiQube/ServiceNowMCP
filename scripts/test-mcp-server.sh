#!/bin/bash

# Navigate to the Functions project directory
cd "$(dirname "$0")/../src/ServiceNow.Functions" || exit

echo "Testing ServiceNow MCP Server..."
echo "================================"

# First, build the project
echo "Building the project..."
dotnet build

if [ $? -ne 0 ]; then
    echo "Build failed!"
    exit 1
fi

echo ""
echo "Running MCP server tests..."
echo ""

# Test initialize request
echo '{"jsonrpc":"2.0","method":"initialize","params":{"protocolVersion":"1.0","capabilities":{},"clientInfo":{"name":"test-client","version":"1.0"}},"id":1}' | dotnet run --project ServiceNow.Functions.csproj -- --mcp-server

echo ""
echo "Test completed!"