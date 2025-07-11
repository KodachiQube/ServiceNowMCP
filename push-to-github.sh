#!/bin/bash

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Function to print colored output
print_color() {
    echo -e "${1}${2}${NC}"
}

# Navigate to the project directory
cd "$(dirname "$0")" || exit

print_color "$YELLOW" "Starting Git operations for ServiceNow MCP..."
echo ""

# Add all changes
print_color "$GREEN" "Adding all changes..."
git add .

# Show status
print_color "$GREEN" "Current git status:"
git status --short

# Commit with a message
echo ""
read -p "Enter commit message: " commit_message
if [ -z "$commit_message" ]; then
    commit_message="Update ServiceNow MCP implementation"
fi

print_color "$GREEN" "Committing changes..."
git commit -m "$commit_message"

# Push to GitHub
print_color "$GREEN" "Pushing to GitHub..."
git push origin main

if [ $? -eq 0 ]; then
    print_color "$GREEN" "✓ Successfully pushed to GitHub!"
else
    print_color "$RED" "✗ Failed to push to GitHub. Please check your connection and credentials."
fi
