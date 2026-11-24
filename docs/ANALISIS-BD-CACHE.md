# Analisis del Comportamiento de la Base de Datos y Estrategia de Cache

## Proyecto: Trabajo Huamanga
### Directorio de Profesionales de Servicios en Huamanga, Ayacucho

| Campo | Detalle |
|-------|---------|
| **Curso** | IS-488 Arquitectura de Software |
| **Docente** | Ing. Lizbeth Jaico Quispe |
| **Alumno** | Everson Alanya Tinco |
| **Semestre** | 2026-II |
| **Sprint** | 2 |

---

## 1. Estado Actual de la Persistencia

### 1.1 Implementacion Actual: In-Memory Storage

Actualmente el sistema usa almacenamiento en memoria:

```csharp
// Program.cs - Datos en memoria
var profesionales = new List<Profesional> { ... };

// Repositorio en memoria para calificaciones
builder.Services.AddSingleton<ICalificacionRepository, CalificacionRepositoryEnMemoria>();
```

**Caracteristicas del estado actual**:
- Los datos de profesionales se inicializan como `List<Profesional>` en Program.cs (5 profesionales semilla)
- Las calificaciones se almacenan en `CalificacionRepositoryEnMemoria` (ConcurrentDictionary)
- Los datos se pierden al reiniciar la aplicacion
- No hay persistencia entre deploys en Railway

**Ventajas para desarrollo**:
- Cero configuracion de base de datos
- Arranque rapido de la aplicacion
- Tests unitarios sin dependencias externas
- Ideal para prototipado y validacion de la arquitectura

**Limitaciones**:
- Datos volatiles (se pierden al reiniciar)
- No soporta multiples instancias (cada instancia tiene sus propios datos)
- No permite consultas complejas (joins, agregaciones SQL)
- Sin respaldo ni recuperacion ante fallos

---

## 2. Migracion Planificada: PostgreSQL

### 2.1 Por que PostgreSQL

| Criterio | PostgreSQL | SQL Server |
|----------|-----------|------------|
| **Costo en Railway** | Plugin nativo gratuito ($5 trial) | Requiere imagen Docker custom |
| **Consumo de RAM** | ~30-50 MB base | ~500 MB minimo |
| **Compatibilidad Railway** | Plugin integrado, 1 clic | Configuracion manual compleja |
| **Licencia** | Open source (libre) | Licencia comercial (Express limitada) |
| **Soporte .NET** | Npgsql (excelente) | SqlClient (excelente) |
| **Ecosistema** | Amplio, comunidad activa | Empresarial, Microsoft-centric |

**Decision**: PostgreSQL por su integracion nativa con Railway, menor consumo de recursos y costo cero.

### 2.2 Esquema Propuesto

```sql
-- Tabla de profesionales
CREATE TABLE profesionales (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nombre      VARCHAR(200) NOT NULL,
    rubro       VARCHAR(100) NOT NULL,
    distrito    VARCHAR(100) NOT NULL,
    created_at  TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Indices para busquedas frecuentes
CREATE INDEX idx_profesionales_rubro ON profesionales(rubro);
CREATE INDEX idx_profesionales_distrito ON profesionales(distrito);
CREATE INDEX idx_profesionales_rubro_distrito ON profesionales(rubro, distrito);

-- Tabla de calificaciones
CREATE TABLE calificaciones (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    vecino_id       UUID NOT NULL,
    profesional_id  UUID NOT NULL REFERENCES profesionales(id),
    puntaje         INTEGER NOT NULL CHECK (puntaje BETWEEN 1 AND 5),
    comentario      TEXT,
    fecha           TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    oculta          BOOLEAN DEFAULT FALSE,
    UNIQUE(vecino_id, profesional_id)  -- Evita calificaciones duplicadas a nivel BD
);

-- Indices para consultas frecuentes
CREATE INDEX idx_calificaciones_profesional ON calificaciones(profesional_id);
CREATE INDEX idx_calificaciones_vecino_prof ON calificaciones(vecino_id, profesional_id);
CREATE INDEX idx_calificaciones_no_ocultas ON calificaciones(profesional_id) WHERE oculta = FALSE;

-- Datos semilla
INSERT INTO profesionales (nombre, rubro, distrito) VALUES
    ('Juan Perez', 'Electricista', 'Ayacucho'),
    ('Maria Lopez', 'Gasfitero', 'San Juan Bautista'),
    ('Carlos Quispe', 'Tutor de Matematicas', 'Carmen Alto'),
    ('Ana Huaman', 'Electricista', 'Jesus Nazareno'),
    ('Pedro Flores', 'Pintor', 'Andres Avelino Caceres');
```

### 2.3 Implementacion con Clean Architecture

Gracias al principio DIP (Dependency Inversion), la migracion requiere solo:

1. **Nueva clase** `CalificacionRepositoryPostgres` que implemente `ICalificacionRepository`
2. **Cambiar UNA linea** en Program.cs:

```csharp
// ANTES (In-Memory):
builder.Services.AddSingleton<ICalificacionRepository, CalificacionRepositoryEnMemoria>();

// DESPUES (PostgreSQL):
builder.Services.AddScoped<ICalificacionRepository, CalificacionRepositoryPostgres>();
```

3. **Ningun cambio** en Dominio ni Aplicacion (capas internas intactas)

---

## 3. Analisis del Comportamiento de la Base de Datos

### 3.1 Patrones de Acceso Identificados

| Operacion | Endpoint | Tipo | Frecuencia Esperada | Complejidad |
|-----------|----------|------|---------------------|-------------|
| Listar profesionales | GET /api/profesionales | Lectura | Muy alta | Baja (SELECT *) |
| Buscar por rubro/distrito | GET /api/profesionales/buscar | Lectura | Alta | Media (WHERE + filtros) |
| Ver detalle profesional | GET /api/profesionales/{id} | Lectura | Alta | Baja (PK lookup) |
| Consultar reputacion | GET /api/reputacion/{pid} | Lectura | Alta | Media (AVG + COUNT + WHERE) |
| Listar calificaciones | GET /api/calificaciones/profesional/{pid} | Lectura | Media | Baja (INDEX scan) |
| Registrar calificacion | POST /api/calificaciones | Escritura | Baja | Media (INSERT + validacion) |
| Registrar profesional | POST /api/profesionales | Escritura | Muy baja | Baja (INSERT) |
| Actualizar profesional | PUT /api/profesionales/{id} | Escritura | Muy baja | Baja (UPDATE by PK) |
| Ocultar calificacion | PATCH /api/calificaciones/{id}/ocultar | Escritura | Muy baja | Baja (UPDATE boolean) |

**Observacion clave**: El sistema tiene un patron de lectura intensiva (read-heavy). La relacion lectura/escritura estimada es **90% lecturas / 10% escrituras**, lo que lo convierte en un candidato ideal para caching.

### 3.2 Consultas Criticas y su Optimizacion

#### Consulta 1: Busqueda de profesionales (mas frecuente)
```sql
-- Sin optimizar
SELECT * FROM profesionales WHERE rubro ILIKE '%electricista%' AND distrito ILIKE '%ayacucho%';

-- Optimizado con indices
SELECT * FROM profesionales WHERE rubro = 'Electricista' AND distrito = 'Ayacucho';
-- Usa idx_profesionales_rubro_distrito (index scan en lugar de seq scan)
```

#### Consulta 2: Calculo de reputacion (mas costosa)
```sql
-- Consulta de reputacion
SELECT
    profesional_id,
    COALESCE(AVG(puntaje), 0) as promedio,
    COUNT(*) as total
FROM calificaciones
WHERE profesional_id = $1 AND oculta = FALSE
GROUP BY profesional_id;
-- Usa idx_calificaciones_no_ocultas (partial index)
```

### 3.3 Estimacion de Volumen de Datos

| Tabla | Registros estimados (1 anio) | Tamano estimado |
|-------|------------------------------|-----------------|
| profesionales | 500 - 2,000 | < 1 MB |
| calificaciones | 5,000 - 20,000 | < 5 MB |

**Conclusion**: El volumen de datos es pequenio. PostgreSQL en Railway (plan free con $5 trial) es mas que suficiente.

---

## 4. Estrategia de Cache

### 4.1 Justificacion del Cache

Dado el patron read-heavy del sistema:
- Las listas de rubros y zonas son **estaticas** (no cambian)
- La lista de profesionales cambia con **poca frecuencia**
- La reputacion cambia solo cuando se registra una nueva calificacion

El cache reduce la carga en la base de datos y mejora significativamente los tiempos de respuesta.

### 4.2 Estrategia: Cache en Memoria con IMemoryCache

Para un sistema de esta escala, `IMemoryCache` de ASP.NET Core es la solucion ideal:

```csharp
// Configuracion en Program.cs
builder.Services.AddMemoryCache();

// Uso en endpoints
app.MapGet("/api/rubros", (IMemoryCache cache) =>
{
    var rubros = cache.GetOrCreate("rubros", entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
        return new[] { "Electricista", "Gasfitero", "Pintor", ... };
    });
    return Results.Ok(rubros);
});
```

### 4.3 Politicas de Cache por Endpoint

| Endpoint | Estrategia | TTL | Invalidacion |
|----------|-----------|-----|-------------|
| GET /api/rubros | Cache estatico | 24 horas | Manual (raro) |
| GET /api/zonas | Cache estatico | 24 horas | Manual (raro) |
| GET /api/profesionales | Cache con TTL | 5 minutos | Al crear/actualizar profesional |
| GET /api/profesionales/buscar | Cache por query | 5 minutos | Al crear/actualizar profesional |
| GET /api/reputacion/{pid} | Cache por ID | 2 minutos | Al registrar calificacion |
| GET /api/calificaciones/profesional/{pid} | Cache por ID | 2 minutos | Al registrar/ocultar calificacion |
| POST /api/profesionales | Sin cache (escritura) | - | Invalida cache de listado |
| POST /api/calificaciones | Sin cache (escritura) | - | Invalida cache de reputacion |

### 4.4 Patron de Invalidacion: Cache-Aside

```
                    ┌─────────────┐
                    │   Cliente   │
                    └──────┬──────┘
                           │
                    ┌──────▼──────┐
                    │   API       │
                    └──────┬──────┘
                           │
              ┌────────────┼────────────┐
              │            │            │
       ┌──────▼──────┐     │     ┌──────▼──────┐
       │    Cache     │     │     │  PostgreSQL │
       │ (IMemoryCache)│    │     │             │
       └──────────────┘     │     └─────────────┘
                            │
                    Flujo de lectura:
                    1. Buscar en cache
                    2. Si existe → retornar (cache HIT)
                    3. Si no existe → consultar BD
                    4. Guardar en cache
                    5. Retornar resultado

                    Flujo de escritura:
                    1. Escribir en BD
                    2. Invalidar cache relacionado
```

### 4.5 Implementacion Propuesta del Cache

```csharp
// Ejemplo: Cache para reputacion de profesional
app.MapGet("/api/reputacion/{pid}", async (Guid pid, ICalificacionRepository repo, IMemoryCache cache) =>
{
    var cacheKey = $"reputacion_{pid}";

    if (!cache.TryGetValue(cacheKey, out object? cached))
    {
        var cal = await repo.ObtenerPorProfesionalAsync(pid);
        var prom = Profesional.CalcularReputacion(cal);
        var result = new { profesionalId = pid, promedio = prom, total = cal.Count(c => !c.Oculta) };

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(2));

        cache.Set(cacheKey, result, cacheOptions);
        cached = result;
    }

    return Results.Ok(cached);
});

// Invalidar cache al registrar calificacion
app.MapPost("/api/calificaciones", async (...) =>
{
    // ... registrar calificacion ...
    cache.Remove($"reputacion_{cmd.ProfesionalId}");
    cache.Remove($"calificaciones_{cmd.ProfesionalId}");
    // ...
});
```

### 4.6 Por que NO Redis (para este proyecto)

| Criterio | IMemoryCache | Redis |
|----------|-------------|-------|
| Configuracion | Cero (incluido en ASP.NET Core) | Requiere servidor adicional |
| Costo en Railway | $0 (en el mismo proceso) | $5+ adicional/mes |
| Latencia | ~0ms (misma memoria del proceso) | ~1-5ms (red) |
| Escalabilidad | 1 instancia | Multiples instancias |
| Complejidad | Ninguna | Serializacion, conexion, manejo de errores |

**Decision**: IMemoryCache es suficiente para una aplicacion de instancia unica en Railway. Redis se justificaria si el sistema escalara a multiples instancias.

---

## 5. Metricas de Rendimiento Esperadas

### 5.1 Sin Cache (solo BD)

| Operacion | Tiempo estimado |
|-----------|----------------|
| Lectura simple (PK) | 5-15 ms |
| Busqueda con filtros | 10-30 ms |
| Calculo de reputacion | 15-50 ms |
| Escritura (INSERT) | 10-30 ms |

### 5.2 Con Cache

| Operacion | Cache HIT | Cache MISS |
|-----------|-----------|------------|
| Lectura simple | < 1 ms | 5-15 ms |
| Busqueda con filtros | < 1 ms | 10-30 ms |
| Calculo de reputacion | < 1 ms | 15-50 ms |

**Mejora estimada**: Reduccion de ~90% en tiempo de respuesta para operaciones de lectura frecuentes (cache hit rate esperado: 80-95%).

---

## 6. Plan de Implementacion

### Fase 1: PostgreSQL (Sprint 3 - actual)
- [ ] Crear plugin PostgreSQL en Railway
- [ ] Implementar `CalificacionRepositoryPostgres` con Npgsql
- [ ] Implementar `ProfesionalRepositoryPostgres`
- [ ] Ejecutar migraciones SQL (crear tablas + indices + datos semilla)
- [ ] Cambiar DI en Program.cs
- [ ] Verificar que todos los tests Postman siguen pasando

### Fase 2: Cache (Sprint 3 - posterior)
- [ ] Agregar `AddMemoryCache()` en Program.cs
- [ ] Implementar cache para rubros y zonas (TTL 24h)
- [ ] Implementar cache para listado de profesionales (TTL 5min)
- [ ] Implementar cache para reputacion (TTL 2min)
- [ ] Implementar invalidacion en endpoints de escritura
- [ ] Ejecutar k6 con cache activo y comparar metricas

---

## 7. Conclusion

El analisis demuestra que:

1. **El patron read-heavy** (90/10) del sistema favorece fuertemente el uso de cache
2. **PostgreSQL** es la opcion optima para Railway por costo, rendimiento y facilidad de integracion
3. **IMemoryCache** es suficiente para la escala actual del proyecto (instancia unica)
4. **Clean Architecture** facilita la migracion gracias a DIP — solo se cambia la capa de Infraestructura
5. **Los indices propuestos** optimizan las consultas mas frecuentes (busqueda por rubro/distrito, reputacion)
6. **La mejora esperada** con cache es de ~90% en tiempos de respuesta para lecturas
