# -----------------------------
# Étape runtime (base)
# -----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# -----------------------------
# Étape build
# -----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copier le projet et restaurer les dépendances
COPY ["MicroserviceB/MicroserviceB.csproj", "MicroserviceB/"]
RUN dotnet restore "MicroserviceB/MicroserviceB.csproj"

# Copier tout le code
COPY . .
WORKDIR "/src/MicroserviceB"

# Compiler en Release
RUN dotnet build "MicroserviceB.csproj" -c $BUILD_CONFIGURATION -o /app/build

# -----------------------------
# Étape publish
# -----------------------------
FROM build AS publish
RUN dotnet publish "MicroserviceB.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# -----------------------------
# Étape finale
# -----------------------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MicroserviceB.dll"]
