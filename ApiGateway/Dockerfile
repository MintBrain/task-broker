# Базовый образ для выполнения
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Образ SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ApiGateway/ApiGateway.csproj ./ApiGateway/
COPY Shared/Shared.csproj ./Shared/
RUN dotnet restore ApiGateway/ApiGateway.csproj
COPY . .
WORKDIR "/src"
RUN dotnet build ApiGateway/ApiGateway.csproj -c Release -o /app/build

# Публикация
FROM build AS publish
RUN dotnet publish ApiGateway/ApiGateway.csproj -c Release -o /app/publish

# Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
