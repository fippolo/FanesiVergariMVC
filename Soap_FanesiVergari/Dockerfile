#	SOAP DOCKERFILE

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
COPY ["Soap_FanesiVergari/Soap_FanesiVergari.csproj", "Soap_FanesiVergari/"]
COPY ["FanesiVergari.Modelli/FanesiVergari.Modelli.csproj", "FanesiVergari.Modelli/"]
RUN dotnet restore "./Soap_FanesiVergari/Soap_FanesiVergari.csproj"
COPY . .
WORKDIR "/src/Soap_FanesiVergari"
RUN dotnet build "./Soap_FanesiVergari.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Questa fase viene usata per pubblicare il progetto di servizio da copiare nella fase finale
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Soap_FanesiVergari.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Questa fase viene usata nell'ambiente di produzione o durante l'esecuzione da Visual Studio in modalità normale (impostazione predefinita quando non si usa la configurazione di debug)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Soap_FanesiVergari.dll"]