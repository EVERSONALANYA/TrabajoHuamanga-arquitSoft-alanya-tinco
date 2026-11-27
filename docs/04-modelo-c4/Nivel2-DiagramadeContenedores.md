# C4 - Nivel 2: Diagrama de contenedores - Trabajo Huamanga

Muestra las aplicaciones y almacenes de datos que componen el sistema.

![C4 nivel 2 - contenedores](../img/nivel2-contenedores.png)

<details>
<summary>Ver codigo Mermaid del diagrama</summary>

```mermaid
flowchart TD
    Vecino["Vecino<br/>Persona"]
    Prof["Profesional<br/>Persona"]
    Admin["Administrador<br/>Persona"]

    subgraph SIS["Trabajo Huamanga"]
        Proxy["NGINX<br/>Proxy reverso"]
        Web["Aplicacion web y panel admin<br/>ASP.NET Core MVC"]
        API["API Trabajo Huamanga<br/>ASP.NET Core 8 - monolito modular"]
        Redis[("Redis<br/>Cache de busquedas")]
        BD[("SQL Server<br/>Datos del sistema")]
    end

    WA["WhatsApp<br/>Sistema externo"]
    OR["OpenRouter<br/>Sistema externo"]

    Vecino -->|"HTTPS"| Proxy
    Prof -->|"HTTPS"| Proxy
    Admin -->|"HTTPS"| Proxy
    Proxy --> Web
    Web -->|"HTTPS / JSON"| API
    API -->|"lee y escribe"| Redis
    API -->|"SQL"| BD
    API -->|"enlace wa.me"| WA
    API -->|"API REST"| OR
```

</details>

| Contenedor | Tecnología | Responsabilidad |
| --- | --- | --- |
| NGINX | NGINX en Docker | Recibe las solicitudes y las reparte entre las réplicas. |
| Aplicación web y panel admin | ASP.NET Core MVC | Interfaz de vecinos, profesionales y administrador. |
| API Trabajo Huamanga | ASP.NET Core 8, EF Core | Reglas de negocio y acceso a datos. Sin estado, escalable en réplicas. |
| Redis | Redis | Caché de las búsquedas frecuentes. |
| SQL Server | SQL Server | Profesionales, calificaciones, rubros y zonas. |

Todos los contenedores propios se despliegan con Docker.
