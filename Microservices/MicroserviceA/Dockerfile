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
WORKDIR /src

# Copier le projet et restaurer les dépendances
COPY ["MicroserviceA/MicroserviceA.csproj", "MicroserviceA/"]
RUN dotnet restore "MicroserviceA/MicroserviceA.csproj"

# Copier tout le code
COPY . .
WORKDIR "/src/MicroserviceA"

# Compiler en Release
RUN dotnet build "MicroserviceA.csproj" -c Release -o /app/build

# -----------------------------
# Étape publish
# -----------------------------
FROM build AS publish
RUN dotnet publish "MicroserviceA.csproj" -c Release -o /app/publish /p:UseAppHost=false

# -----------------------------
# Étape finale
# -----------------------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MicroserviceA.dll"]
