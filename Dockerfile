# Используем базовый образ .NET для сборки
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Копируем csproj и восстанавливаем зависимости
COPY *.csproj ./
RUN dotnet restore

# Копируем всё остальное и собираем проект
COPY . ./
RUN dotnet publish -c Release -o out

# Используем базовый образ ASP.NET для запуска приложения
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .

# Экспонируем порт, на котором будет работать приложение
EXPOSE 80

# Запуск приложения
ENTRYPOINT ["dotnet", "YourAppName.dll"]
