# ==========================================
# Stage 1: Build & Publish
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files first to leverage Docker layer caching
COPY ["SkinInventoryApp.sln", "./"]
COPY ["Web/Web.csproj", "Web/"]
COPY ["Service/Service.csproj", "Service/"]
COPY ["Repository/Repository.csproj", "Repository/"]
COPY ["Domain/Domain.csproj", "Domain/"]

# Restore dependencies
RUN dotnet restore "Web/Web.csproj"

# Copy the rest of the source code
COPY . .

# Build and publish the Web project in Release mode
WORKDIR "/src/Web"
RUN dotnet publish "Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ==========================================
# Stage 2: Runtime Image
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Create a data directory for SQLite database persistence
RUN mkdir -p /app/data

# Copy published build artifacts from build stage
COPY --from=build /app/publish .

# Expose HTTP ports
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Set entry point
ENTRYPOINT ["dotnet", "Web.dll"]