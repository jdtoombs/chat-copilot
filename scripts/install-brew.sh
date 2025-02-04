#!/usr/bin/env bash

# Installs the requirements for running Q-Pilot.

set -e

# Install the requirements
brew install yarn;
brew install --cask dotnet-sdk;
brew install nodejs;

echo ""
echo "YARN $(yarn --version) installed."
echo "NODEJS $(node --version) installed."
echo "DOTNET $(dotnet --version) installed."
