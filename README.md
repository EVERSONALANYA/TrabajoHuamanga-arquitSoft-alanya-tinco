# 💼 Trabajo Huamanga

**Directorio de profesionales de servicios en Huamanga, Ayacucho**

> IS-488 Arquitectura de Software — UNSCH  
> Docente: Ing. Lizbeth Jaico Quispe

---

## 📋 Descripción

Trabajo Huamanga es una aplicación web que permite a los habitantes de Huamanga encontrar y contactar profesionales de servicios locales (electricistas, gasfiteros, albañiles, etc.) de manera rápida y confiable.

## 🏗️ Arquitectura

**Monolito modular con Clean Architecture** (4 capas):

```
┌─────────────────────────────────┐
│   Presentación (Web MVC + API)  │
├─────────────────────────────────┤
│   Aplicación (Casos de Uso)     │
├─────────────────────────────────┤
│   Dominio (Entidades/Interfaces)│
├─────────────────────────────────┤
│   Infraestructura (EF/Redis)    │
└─────────────────────────────────┘
```

### 6 Módulos de Negocio
1. **Profesionales** — Registro y gestión de perfiles
2. **Búsqueda** — Búsqueda por rubro, zona y término (con caché Redis)
3. **Reputación** — Sistema de calificaciones (1-5 estrellas)
4. **Contacto** — Enlace directo a WhatsApp (wa.me)
5. **Sugerencia de Rubro (IA)** — OpenRouter → Gemini Flash
6. **Moderación** — Gestión de calificaciones reportadas

### 3 Roles de Actor
- **Usuario** — Busca profesionales, califica, contacta por WhatsApp
- **Profesional** — Se registra, gestiona su perfil
- **Administrador** — Modera calificaciones reportadas

## 🛠️ Tecnologías

| Componente | Tecnología |
|-----------|-----------|
| Backend | ASP.NET Core 8 |
| Frontend | Razor Pages (MVC) + Bootstrap 5 |
| Base de datos | SQL Server 2022 |
| Caché | Redis 7 |
| ORM | Entity Framework Core 8 |
| IA | OpenRouter → Gemini Flash 1.5 |
| Contacto | WhatsApp (wa.me links) |
| Contenedores | Docker Compose |
| Reverse Proxy | NGINX |

## 🚀 Cómo Ejecutar

### Opción 1: Docker Compose (recomendado)

```bash
# Clonar o descomprimir el proyecto
cd TrabajoHuamanga

# Levantar todos los servicios
docker-compose up -d

# Acceder a:
# - Web: http://localhost (o http://localhost:5000)
# - API + Swagger: http://localhost:5001/swagger
```

### Opción 2: Desarrollo local

**Requisitos:** .NET 8 SDK, SQL Server, Redis

```bash
# Restaurar dependencias
dotnet restore

# Ejecutar la aplicación web
cd src/TrabajoHuamanga.Web
dotnet run

# O ejecutar la API
cd src/TrabajoHuamanga.API
dotnet run
```

### Opción 3: Solo la API

```bash
cd src/TrabajoHuamanga.API
dotnet run
# Swagger: https://localhost:5001/swagger
```

## 📂 Estructura del Proyecto

```
TrabajoHuamanga/
├── TrabajoHuamanga.sln
├── docker-compose.yml
├── Dockerfile
├── nginx.conf
├── src/
│   ├── TrabajoHuamanga.Dominio/          # Capa Dominio
│   │   ├── Entidades/
│   │   │   ├── Profesional.cs
│   │   │   ├── Calificacion.cs
│   │   │   ├── Rubro.cs
│   │   │   └── Zona.cs
│   │   └── Interfaces/
│   │       ├── IProfesionalRepository.cs
│   │       └── IServiciosExternos.cs
│   ├── TrabajoHuamanga.Aplicacion/       # Capa Aplicación
│   │   ├── DTOs/
│   │   └── CasosDeUso/
│   │       ├── Profesionales/
│   │       ├── Busqueda/
│   │       ├── Reputacion/
│   │       ├── Contacto/
│   │       ├── SugerenciaRubro/
│   │       └── Moderacion/
│   ├── TrabajoHuamanga.Infraestructura/  # Capa Infraestructura
│   │   ├── Datos/AppDbContext.cs
│   │   ├── Repositorios/
│   │   ├── Adaptadores/
│   │   └── Cache/
│   ├── TrabajoHuamanga.API/              # API REST
│   │   ├── Controllers/
│   │   └── Program.cs
│   ├── TrabajoHuamanga.Web/              # Frontend MVC
│   │   ├── Controllers/
│   │   ├── Views/
│   │   └── wwwroot/
│   └── TrabajoHuamanga.Tests/            # Tests unitarios
└── README.md
```

## 🧪 Tests

```bash
dotnet test
```

Tests incluidos:
- Entidades del dominio (Profesional, Calificacion, Rubro)
- Caso de uso BuscarProfesional (con mock de caché)
- Adaptador WhatsApp (generación de enlaces)

## ⚙️ Configuración

Editar `appsettings.json` para configurar:

```json
{
  "ConnectionStrings": {
    "SqlServer": "Server=localhost;Database=TrabajoHuamanga;...",
    "Redis": "localhost:6379"
  },
  "OpenRouter": {
    "ApiKey": "TU_API_KEY",
    "Model": "google/gemini-flash-1.5"
  }
}
```

## 📊 Atributos de Calidad

| Atributo | Meta | Mecanismo |
|----------|------|-----------|
| Rendimiento | < 300ms | Redis caché (~2ms vs ~200ms) |
| Escalabilidad | 500 usuarios | Docker horizontal |
| Mantenibilidad | < 8h nuevo módulo | Clean Architecture modular |

---

**Everson Alanya Tinco** — UNSCH, Ingeniería de Sistemas, 2024
