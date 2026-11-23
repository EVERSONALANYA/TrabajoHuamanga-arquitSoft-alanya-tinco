FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY tecnologia/boilerplate-clean-arquitecture/TrabajoHuamanga.sln .
COPY tecnologia/boilerplate-clean-arquitecture/src/TrabajoHuamanga.Dominio/TrabajoHuamanga.Dominio.csproj src/TrabajoHuamanga.Dominio/
COPY tecnologia/boilerplate-clean-arquitecture/src/TrabajoHuamanga.Aplicacion/TrabajoHuamanga.Aplicacion.csproj src/TrabajoHuamanga.Aplicacion/
COPY tecnologia/boilerplate-clean-arquitecture/src/TrabajoHuamanga.Infraestructura/TrabajoHuamanga.Infraestructura.csproj src/TrabajoHuamanga.Infraestructura/
COPY tecnologia/boilerplate-clean-arquitecture/src/TrabajoHuamanga.Api/TrabajoHuamanga.Api.csproj src/TrabajoHuamanga.Api/
COPY tecnologia/boilerplate-clean-arquitecture/tests/TrabajoHuamanga.Pruebas/TrabajoHuamanga.Pruebas.csproj tests/TrabajoHuamanga.Pruebas/

RUN dotnet restore TrabajoHuamanga.sln

COPY tecnologia/boilerplate-clean-arquitecture/ .
RUN dotnet publish src/TrabajoHuamanga.Api/TrabajoHuamanga.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "TrabajoHuamanga.Api.dll"]
