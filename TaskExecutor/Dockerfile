FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY TaskExecutor.csproj ./
RUN dotnet restore "TaskExecutor.csproj"
# Копируем все файлы и строим приложение
COPY . ./
WORKDIR "/src"
RUN dotnet build "TaskExecutor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskExecutor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskExecutor.dll"]
