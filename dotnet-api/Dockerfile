# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Copy csproj and restore dependencies
COPY BuildDeploy.Demo.sln .
COPY BuildDeploy.Demo/BuildDeploy.Demo.csproj ./BuildDeploy.Demo/BuildDeploy.Demo.csproj 
COPY BuildDeploy.Demo.Tests/BuildDeploy.Demo.Tests.csproj ./BuildDeploy.Demo.Tests/BuildDeploy.Demo.Tests.csproj

RUN dotnet restore

# Copy the rest of the files and build
COPY . .
RUN dotnet build -c Release
RUN dotnet test -c Release

# Publish stage
FROM build AS publish
RUN dotnet publish BuildDeploy.Demo/BuildDeploy.Demo.csproj -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

# Create a non-root user
RUN adduser --disabled-password --gecos "" --home /app appuser && \
    chown -R appuser:appuser /app

# Copy published files from publish stage
COPY --from=publish --chown=appuser:appuser /app/publish .

# Switch to non-root user
USER appuser

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8020
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8020

ENTRYPOINT ["dotnet", "BuildDeploy.Demo.dll"]