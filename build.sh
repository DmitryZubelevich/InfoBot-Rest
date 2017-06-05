#!/usr/bin/env bash
dotnet restore && dotnet build && dotnet publish -c Release
