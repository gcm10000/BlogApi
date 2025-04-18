# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Create the directory and set the correct permissions
RUN mkdir -p /app/migrations && chmod -R 777 /app/migrations
RUN mkdir -p /app/wwwroot/uploads && chmod -R 777 /app/wwwroot/uploads



# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/BlogApi.API/BlogApi.API.csproj", "src/BlogApi.API/"]
COPY ["src/BlogApi.Application/BlogApi.Application.csproj", "src/BlogApi.Application/"]
COPY ["src/BlogApi.Domain/BlogApi.Domain.csproj", "src/BlogApi.Domain/"]
RUN dotnet restore "./src/BlogApi.API/BlogApi.API.csproj"
COPY . .
WORKDIR "/src/src/BlogApi.API"
RUN dotnet build "./BlogApi.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BlogApi.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlogApi.API.dll"]