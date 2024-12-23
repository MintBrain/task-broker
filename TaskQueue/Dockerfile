# !!RUN FROM Parent directory /task-broker
# Use official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory in the container
WORKDIR /app

# Copy the csproj files and restore any dependencies (via 'dotnet restore')
COPY TaskQueue/TaskQueue.csproj ./TaskQueue/
COPY Shared/Shared.csproj ./Shared/

# Restore NuGet packages
RUN dotnet restore TaskQueue/TaskQueue.csproj

# Copy the entire source code into the container
COPY . .

# Build the application
RUN dotnet build TaskQueue/TaskQueue.csproj -c Release -o /app/build

# Publish the application to the /app/publish directory
RUN dotnet publish TaskQueue/TaskQueue.csproj -c Release -o /app/publish

# Set the working directory for the final stage
WORKDIR /app/publish

# Use a smaller image to run the application (runtime image)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Set the working directory in the container
WORKDIR /app

# Copy the published app from the build stage
COPY --from=build /app/publish .

# Expose the port the app will run on
EXPOSE 5001

# Run the application
ENTRYPOINT ["dotnet", "TaskQueue.dll"]
