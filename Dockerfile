# -------------------------------------------------------------
# Stage 1: Base Runtime (.NET 10)
# -------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# -------------------------------------------------------------
# Stage 2: SDK & Restore Dependencies (.NET 10)
# -------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy file .csproj dan restore dependencies
COPY ["MS_USER.csproj", "./"]
RUN dotnet restore "MS_USER.csproj"

# Copy seluruh file project
COPY . .
RUN dotnet build "MS_USER.csproj" -c Release -o /app/build

# -------------------------------------------------------------
# Stage 3: Publish App
# -------------------------------------------------------------
FROM build AS publish
RUN dotnet publish "MS_USER.csproj" -c Release -o /app/publish /p:UseAppHost=false

# -------------------------------------------------------------
# Stage 4: Final Image
# -------------------------------------------------------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MS_USER.dll"]