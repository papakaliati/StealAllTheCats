# Use the official .NET 8 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

#EXPOSE 80
#EXPOSE 443

WORKDIR /app

# Copy solution and restore as distinct layers
COPY *.sln .
COPY src/*.csproj ./src/
RUN for file in src/*.csproj; do dotnet restore "$file"; done

# Copy everything else and build
COPY . .
RUN dotnet publish "src/StealAllTheCats.csproj" -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

# Expose port 8080 (default for ASP.NET 8)
EXPOSE 8080
EXPOSE 1433

ENTRYPOINT ["dotnet", "StealAllTheCats.dll"]
