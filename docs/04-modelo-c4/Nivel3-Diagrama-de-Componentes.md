# C4 - Nivel 3: Diagrama de componentes - Trabajo Huamanga

Muestra los componentes dentro del contenedor **API Trabajo Huamanga**.

![C4 nivel 3 - componentes](../img/nivel3-componentes.png)

<details>
<summary>Ver codigo Mermaid del diagrama</summary>

```mermaid
flowchart TD
    Web["Aplicacion web y panel admin<br/>Contenedor"]

    subgraph API["API Trabajo Huamanga - ASP.NET Core 8"]
        Rest["Controladores API REST"]
        Pro["Modulo Profesionales"]
        Bus["Modulo Busqueda"]
        Rep["Modulo Reputacion"]
        Con["Modulo Contacto"]
        Sug["Modulo Sugerencia de rubro"]
        Mod["Modulo Moderacion y reportes"]
        Datos["Repositorios EF Core"]
    end

    Redis[("Redis")]
    BD[("SQL Server")]
    WA["WhatsApp"]
    OR["OpenRouter"]

    Web -->|"HTTPS / JSON"| Rest
    Rest --> Pro
    Rest --> Bus
    Rest --> Rep
    Rest --> Con
    Rest --> Sug
    Rest --> Mod
    Bus --> Pro
    Bus --> Rep
    Mod --> Rep
    Pro --> Datos
    Bus --> Datos
    Rep --> Datos
    Mod --> Datos
    Bus --> Redis
    Datos --> BD
    Con --> WA
    Sug --> OR
```

</details>

La descripción de cada componente y sus reglas de interacción están en [componentes-arquitectonicos.md](../02-arquitectura-software/componentes-arquitectonicos.md).
