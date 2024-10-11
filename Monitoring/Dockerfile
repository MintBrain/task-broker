# Используем официальный образ .NET 6 SDK для сборки приложения
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Копируем csproj и восстанавливаем зависимости
COPY *.csproj ./
RUN dotnet restore

# Копируем все файлы и строим приложение
COPY . ./
RUN dotnet publish -c Release -o out

# Используем официальный образ .NET 6 Runtime для запуска приложения
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out ./

# Указываем, что приложение будет прослушивать порт 80
EXPOSE 80

# Указываем команду для запуска приложения
ENTRYPOINT ["dotnet", "Monitoring.dll"]
