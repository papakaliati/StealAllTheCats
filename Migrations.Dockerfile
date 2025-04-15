FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copy solution and restore as distinct layers
COPY *.sln .
COPY src/*.csproj ./src/
RUN for file in src/*.csproj; do dotnet restore "$file"; done
COPY setup.sh setup.sh


# Copy everything else and build
COPY . .
RUN dotnet publish "src/StealAllTheCats.csproj"

RUN dotnet tool install --global dotnet-ef

RUN /root/.dotnet/tools/dotnet-ef migrations add InitialMigrations --project "src/StealAllTheCats.csproj"

RUN chmod +x ./setup.sh
CMD /bin/bash ./setup.sh

#RUN /root/.dotnet/tools/dotnet-ef database update --project "src/StealAllTheCats.csproj"