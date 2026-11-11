# ════════════════════════════════════════════════════════
# Trabajo Huamanga — Multi-stage Dockerfile
# ════════════════════════════════════════════════════════

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de solución y proyectos para restaurar dependencias
COPY TrabajoHuamanga.sln .
COPY src/TrabajoHuamanga.Dominio/TrabajoHuamanga.Dominio.csproj src/TrabajoHuamanga.Dominio/
COPY src/TrabajoHuamanga.Aplicacion/TrabajoHuamanga.Aplicacion.csproj src/TrabajoHuamanga.Aplicacion/
COPY src/TrabajoHuamanga.Infraestructura/TrabajoHuamanga.Infraestructura.csproj src/TrabajoHuamanga.Infraestructura/
COPY src/TrabajoHuamanga.API/TrabajoHuamanga.API.csproj src/TrabajoHuamanga.API/
COPY src/TrabajoHuamanga.Web/TrabajoHuamanga.Web.csproj src/TrabajoHuamanga.Web/
COPY src/TrabajoHuamanga.Tests/TrabajoHuamanga.Tests.csproj src/TrabajoHuamanga.Tests/

RUN dotnet restore

# Copiar todo el código fuente
COPY src/ src/

# Publicar Web (incluye API internamente si se necesita)
RUN dotnet publish src/TrabajoHuamanga.Web/TrabajoHuamanga.Web.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "TrabajoHuamanga.Web.dll"]
