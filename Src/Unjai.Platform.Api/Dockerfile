# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["Src/Unjai.Platform.Api/Unjai.Platform.Api.csproj", "Src/Unjai.Platform.Api/"]
COPY ["Src/Unjai.Platform.Application/Unjai.Platform.Application.csproj", "Src/Unjai.Platform.Application/"]
COPY ["Src/Unjai.Platform.Contracts/Unjai.Platform.Contracts.csproj", "Src/Unjai.Platform.Contracts/"]
COPY ["Src/Unjai.Platform.Domain/Unjai.Platform.Domain.csproj", "Src/Unjai.Platform.Domain/"]
COPY ["Src/Unjai.Platform.Extensions.ObjectMapper/Unjai.Platform.Extensions.ObjectMapper.csproj", "Src/Unjai.Platform.Extensions.ObjectMapper/"]
COPY ["Src/Unjai.Platform.Infrastructure.Persistent/Unjai.Platform.Infrastructure.Persistent.csproj", "Src/Unjai.Platform.Infrastructure.Persistent/"]
COPY ["Src/Unjai.Platform.Infrastructure/Unjai.Platform.Infrastructure.csproj", "Src/Unjai.Platform.Infrastructure/"]
COPY ["Src/Unjai.Platform.ServiceDefaults/Unjai.Platform.ServiceDefaults.csproj", "Src/Unjai.Platform.ServiceDefaults/"]
RUN dotnet restore "./Src/Unjai.Platform.Api/Unjai.Platform.Api.csproj"
COPY . .
WORKDIR "/src/Src/Unjai.Platform.Api"
RUN dotnet build "./Unjai.Platform.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Unjai.Platform.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Unjai.Platform.Api.dll"]