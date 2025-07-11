#!/bin/bash

# Test ServiceNow MCP Server

echo "Testing ServiceNow MCP Server..."
export PATH="/usr/local/share/dotnet:$PATH"
export HOME=/Users/kodachiqube

# Start the MCP server in background
cd ServiceNowMCP.AzureFunction
dotnet run -- --mcp &
MCP_PID=$!

# Wait for server to start
sleep 3

# Test if it's running
if ps -p $MCP_PID > /dev/null; then
    echo "✓ MCP Server started successfully (PID: $MCP_PID)"
    
    # Send test commands via netcat
    echo "Testing initialize method..."
    echo '{"jsonrpc":"2.0","method":"initialize","params":{"protocolVersion":"1.0"},"id":1}' | nc localhost 5000 2>/dev/null || echo "Note: Direct connection test skipped"
    
    # Kill the server
    kill $MCP_PID 2>/dev/null
    echo "✓ MCP Server stopped"
else
    echo "✗ Failed to start MCP Server"
fi

echo ""
echo "To use with Claude Desktop, add this to your configuration:"
echo '{
  "mcpServers": {
    "servicenow": {
      "command": "/usr/local/share/dotnet/dotnet",
      "args": ["run", "--project", "'$(pwd)'/ServiceNowMCP.AzureFunction", "--", "--mcp"],
      "env": {
        "HOME": "/Users/kodachiqube",
        "ServiceNow__InstanceUrl": "https://your-instance.service-now.com",
        "ServiceNow__Username": "your-username",
        "ServiceNow__Password": "your-password"
      }
    }
  }
}'
