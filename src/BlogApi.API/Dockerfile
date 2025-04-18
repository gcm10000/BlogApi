# Estágio base, utilizado para rodar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root  # Use o usuário root para configurar permissões
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Crie o diretório de uploads e defina as permissões
RUN mkdir -p /app/wwwroot/uploads && chmod -R 777 /app/wwwroot/uploads

# Usado para build do projeto
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

# Estágio de publicação
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BlogApi.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Estágio final (produção)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlogApi.API.dll"]
