# Est�gio base, utilizado para rodar a aplica��o
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Usar root para configurar permiss�es
USER root 
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Criar o diret�rio de uploads e definir permiss�es para garantir que o app tenha acesso
RUN mkdir -p /app/wwwroot/uploads && chmod -R 777 /app/wwwroot && chmod -R 777 /app/wwwroot/uploads

## Voltar para o usu�rio padr�o 'app'
#USER app


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

# Est�gio de publica��o
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BlogApi.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Est�gio final (produ��o)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlogApi.API.dll"]
