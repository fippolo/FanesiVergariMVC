# Dockerfile for FanesiVergariMVC
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all project files for restore (MVC depends on both Modelli and SOAP)
COPY ["FanesiVergariMVC/FanesiVergariMVC.csproj", "FanesiVergariMVC/"]
COPY ["FanesiVergari.Modelli/FanesiVergari.Modelli.csproj", "FanesiVergari.Modelli/"]
COPY ["Soap_FanesiVergari/Soap_FanesiVergari.csproj", "Soap_FanesiVergari/"]
RUN dotnet restore "./FanesiVergariMVC/FanesiVergariMVC.csproj"

# Copy all necessary source files
COPY ["FanesiVergariMVC/", "FanesiVergariMVC/"]
COPY ["FanesiVergari.Modelli/", "FanesiVergari.Modelli/"]
COPY ["Soap_FanesiVergari/", "Soap_FanesiVergari/"]

# Remove appsettings files from SOAP project to avoid conflicts
RUN rm -f /src/Soap_FanesiVergari/appsettings*.json

WORKDIR "/src/FanesiVergariMVC"
RUN dotnet build "./FanesiVergariMVC.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FanesiVergariMVC.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FanesiVergariMVC.dll"]