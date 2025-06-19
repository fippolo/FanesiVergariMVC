# MVC DOCKERFILE

# Questa fase viene usata durante l'esecuzione da Visual Studio in modalità rapida (impostazione predefinita per la configurazione di debug)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Questa fase viene usata per compilare il progetto di servizio
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FanesiVergariMVC/FanesiVergariMVC.csproj", "FanesiVergariMVC/"]
COPY ["FanesiVergari.Modelli/FanesiVergari.Modelli.csproj", "FanesiVergari.Modelli/"]
COPY ["Soap_FanesiVergari/Soap_FanesiVergari.csproj", "Soap_FanesiVergari/"]
RUN dotnet restore "./FanesiVergariMVC/FanesiVergariMVC.csproj"
COPY . .
WORKDIR "/src/FanesiVergariMVC"
RUN dotnet build "./FanesiVergariMVC.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Questa fase viene usata per pubblicare il progetto di servizio da copiare nella fase finale
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FanesiVergariMVC.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Questa fase viene usata nell'ambiente di produzione o durante l'esecuzione da Visual Studio in modalità normale (impostazione predefinita quando non si usa la configurazione di debug)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FanesiVergariMVC.dll"]