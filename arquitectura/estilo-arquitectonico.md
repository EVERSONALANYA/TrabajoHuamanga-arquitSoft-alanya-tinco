# Estilo arquitectónico - Trabajo Huamanga

## Estilo seleccionado

**Monolito modular en capas.**

- **Monolito:** la API se despliega como una sola aplicación (ASP.NET Core 8) con una única base de datos. Es la unidad de despliegue.
- **Modular:** internamente se organiza en módulos de negocio independientes (Profesionales, Búsqueda, Reputación, Contacto, Sugerencia de rubro y Moderación/Reportes).
- **En capas:** cada módulo separa presentación, lógica de negocio y datos.

Capas = organización lógica; monolito = unidad de despliegue. Pueden coexistir.

## Justificación

| Driver | Cómo lo responde el estilo |
| --- | --- |
| DA01 - Escalabilidad | La API no guarda estado, por lo que se despliega en varias réplicas detrás de un proxy reverso. |
| DA02 - Rendimiento | Una capa de caché (Redis) entre la lógica de negocio y la base de datos. |
| DA06 - Mantenibilidad | Los módulos se comunican por interfaces y no acceden a los datos de otro módulo. |

## Diagrama

```mermaid
flowchart TD
    subgraph ACTORES["ACTORES"]
        Vecino["Vecino"]
        Profesional["Profesional / Técnico"]
        Admin["Administrador"]
    end

    Web["Aplicación Web y Panel de Administración<br/>ASP.NET Core MVC"]

    subgraph MONOLITO["Monolito Trabajo Huamanga - API ASP.NET Core 8 - un solo despliegue"]
        subgraph PRES["1. CAPA DE PRESENTACION"]
            Ctrl["Controladores API REST"]
        end
        subgraph NEG["2. CAPA DE LOGICA DE NEGOCIO"]
            M1["Modulo Profesionales"]
            M2["Modulo Busqueda"]
            M3["Modulo Reputacion"]
            M4["Modulo Contacto"]
            M5["Modulo Sugerencia de rubro"]
            M6["Modulo Moderacion y reportes"]
        end
        subgraph DAT["3. CAPA DE DATOS"]
            Repo["Repositorios EF Core"]
        end
    end

    Redis[("Redis<br/>cache de busquedas")]
    BD[("SQL Server")]
    WA["WhatsApp<br/>sistema externo"]
    OR["OpenRouter<br/>sistema externo"]

    Vecino --> Web
    Profesional --> Web
    Admin --> Web
    Web -->|"HTTPS / JSON"| Ctrl
    Ctrl --> M1
    Ctrl --> M2
    Ctrl --> M3
    Ctrl --> M4
    Ctrl --> M5
    Ctrl --> M6
    M1 --> Repo
    M2 --> Repo
    M3 --> Repo
    M6 --> Repo
    M2 -->|"lee / escribe"| Redis
    Repo -->|"TDS / SQL"| BD
    M4 -->|"enlace wa.me"| WA
    M5 -->|"API REST"| OR
```

## Reglas de la arquitectura

1. Cada capa solo invoca a la capa inmediatamente inferior.
2. Un módulo no accede a las tablas de otro módulo; usa su servicio.
3. La comunicación entre módulos se hace llamando a su interfaz.
4. Todo se ejecuta en un único proceso con una única base de datos.
5. WhatsApp y OpenRouter son sistemas externos, fuera del monolito.
