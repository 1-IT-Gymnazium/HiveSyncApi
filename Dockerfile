FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and all project files first (for layer caching)
COPY src/HiveSync.sln src/
COPY src/CodeHive.Utility/CodeHive.Utility.csproj src/CodeHive.Utility/
COPY src/HiveSync.Api/HiveSync.Api.csproj src/HiveSync.Api/
COPY src/HiveSync.Application/HiveSync.Application.csproj src/HiveSync.Application/
COPY src/HiveSync.Application.Contracts/HiveSync.Application.Contracts.csproj src/HiveSync.Application.Contracts/
COPY src/HiveSync.Application.Contracts.Interfaces/HiveSync.Application.Contracts.Interfaces.csproj src/HiveSync.Application.Contracts.Interfaces/
COPY src/HiveSync.Auth/HiveSync.Auth.csproj src/HiveSync.Auth/
COPY src/HiveSync.Configurations/HiveSync.Configurations.csproj src/HiveSync.Configurations/
COPY src/HiveSync.Data/HiveSync.Data.csproj src/HiveSync.Data/
COPY src/HiveSync.Utilities/HiveSync.Utilities.csproj src/HiveSync.Utilities/

RUN dotnet restore src/HiveSync.sln

# Copy everything else
COPY . .

RUN dotnet publish src/HiveSync.Api/HiveSync.Api.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HiveSync.Api.dll"]