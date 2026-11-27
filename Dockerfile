FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app
COPY src/TrabajoHuamanga.Dominio/TrabajoHuamanga.Dominio.csproj src/TrabajoHuamanga.Dominio/
COPY src/TrabajoHuamanga.Aplicacion/TrabajoHuamanga.Aplicacion.csproj src/TrabajoHuamanga.Aplicacion/
COPY src/TrabajoHuamanga.Infraestructura/TrabajoHuamanga.Infraestructura.csproj src/TrabajoHuamanga.Infraestructura/
COPY src/TrabajoHuamanga.API/TrabajoHuamanga.API.csproj src/TrabajoHuamanga.API/
RUN dotnet restore src/TrabajoHuamanga.API/TrabajoHuamanga.API.csproj
COPY src/ src/
RUN dotnet publish src/TrabajoHuamanga.API/TrabajoHuamanga.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "TrabajoHuamanga.API.dll"]
