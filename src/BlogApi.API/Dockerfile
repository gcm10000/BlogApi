# Estágio base, utilizado para rodar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Usar root para configurar permissões
USER root
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Criar diretório e dar permissão para o usuário 'app' (UID 1000)
RUN mkdir -p /app/wwwroot/uploads \
    && chown -R 1000:1000 /app/wwwroot

# Voltar para o usuário padrão 'app' (UID 1000)
USER app


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
