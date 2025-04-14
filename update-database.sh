#!/bin/bash

PROJECT="./src/StealAllTheCats.csproj"

echo "Running database update..."
dotnet ef database update --project $PROJECT