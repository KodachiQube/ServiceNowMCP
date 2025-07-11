#!/bin/bash

# Navigate to the Functions project directory
cd "$(dirname "$0")/../src/ServiceNow.Functions" || exit

echo "Starting ServiceNow Azure Function..."
echo "=================================="

# Run the Azure Function
func start --verbose

# Keep the script running
wait
