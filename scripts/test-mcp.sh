#!/bin/bash

# Test script for ServiceNow MCP Server

echo "Testing ServiceNow MCP Server..."

# Create a test JSON-RPC request
cat << 'JSON' | dotnet run --project ServiceNowMCP.AzureFunction -- --mcp 2>/dev/null
{"jsonrpc":"2.0","method":"initialize","params":{"protocolVersion":"1.0"},"id":1}
{"jsonrpc":"2.0","method":"tools/list","params":{},"id":2}
JSON
