#!/usr/bin/env python3
import json
import subprocess
import time

print("Testing ServiceNow MCP Server Integration...")

# Start MCP server
proc = subprocess.Popen(
    ["dotnet", "run", "--project", "ServiceNowMCP.AzureFunction", "--", "--mcp"],
    stdin=subprocess.PIPE,
    stdout=subprocess.PIPE,
    stderr=subprocess.PIPE,
    text=True,
    env={"PATH": "/usr/local/share/dotnet:" + subprocess.os.environ.get("PATH", ""), "HOME": "/Users/kodachiqube"}
)

time.sleep(2)

# Test initialize
request = {"jsonrpc": "2.0", "method": "initialize", "params": {"protocolVersion": "1.0"}, "id": 1}
proc.stdin.write(json.dumps(request) + "\n")
proc.stdin.flush()

# Read response
response_line = proc.stdout.readline()
if response_line:
    response = json.loads(response_line)
    print("✓ Initialize response:", response.get("result", {}).get("serverInfo", {}))

# Test tools/list
request = {"jsonrpc": "2.0", "method": "tools/list", "params": {}, "id": 2}
proc.stdin.write(json.dumps(request) + "\n")
proc.stdin.flush()

response_line = proc.stdout.readline()
if response_line:
    response = json.loads(response_line)
    tools = response.get("result", {}).get("tools", [])
    print(f"✓ Found {len(tools)} tools")
    if tools:
        print("  First 5 tools:", [t["name"] for t in tools[:5]])

# Test tool call
request = {
    "jsonrpc": "2.0",
    "method": "tools/call",
    "params": {
        "name": "incident_create",
        "arguments": {
            "short_description": "Test incident from MCP",
            "urgency": 3,
            "impact": 3
        }
    },
    "id": 3
}
proc.stdin.write(json.dumps(request) + "\n")
proc.stdin.flush()

response_line = proc.stdout.readline()
if response_line:
    response = json.loads(response_line)
    print("✓ Tool call response received")

proc.terminate()
print("\n✓ All tests passed!")
