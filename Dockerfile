# Use the official .NET 8 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and restore as distinct layers
COPY *.sln .
COPY src/*/*.csproj ./src/
RUN for file in src/*/*.csproj; do dotnet restore "$file"; done

# Copy everything else and build
COPY . .
RUN dotnet publish "src/CatStash.Api/CatStash.Api.csproj" -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CatStash.Api.dll"]
