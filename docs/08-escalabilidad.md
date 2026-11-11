# 8-9. Escalabilidad del Frontend, Backend y Base de Datos

## 8.1 Escalabilidad del Backend

### Estrategia: Escalado horizontal con Docker Compose

```yaml
# docker-compose.scale.yml — Escalar a múltiples instancias
services:
  web:
    deploy:
      replicas: 3    # 3 instancias del frontend
    # NGINX balancea entre las 3 réplicas

  api:
    deploy:
      replicas: 2    # 2 instancias de la API
```

Comando para escalar:
```bash
docker-compose up -d --scale web=3 --scale api=2
```

### Arquitectura escalada:

```
                    ┌─────────┐
                    │  NGINX  │ (Load Balancer)
                    │ :80     │
                    └────┬────┘
                    ┌────┼────────────┐
                    ▼    ▼            ▼
              ┌─────┐ ┌─────┐   ┌─────┐
              │Web 1│ │Web 2│   │Web 3│  (Horizontal scaling)
              └──┬──┘ └──┬──┘   └──┬──┘
                 │       │         │
                 ▼       ▼         ▼
              ┌──────────────────────┐
              │     Redis Cache      │ (Compartido)
              └──────────┬───────────┘
                         │
              ┌──────────▼───────────┐
              │     SQL Server       │ (Compartido)
              └──────────────────────┘
```

### Mecanismos de escalabilidad backend:
1. **Stateless**: Las instancias Web/API no guardan estado en memoria, usan Redis compartido
2. **Caché distribuido**: Redis como caché compartido entre todas las instancias
3. **Health checks**: Docker monitorea la salud de cada instancia
4. **NGINX round-robin**: Distribuye peticiones entre instancias disponibles

## 8.2 Escalabilidad del Frontend

### Estrategias implementadas:
1. **CDN para assets estáticos**: Bootstrap y Font Awesome desde CDN (jsdelivr, cdnjs)
2. **Razor pre-compilado**: Las vistas se compilan en build time, no en runtime
3. **Assets minificados**: CSS optimizado, sin JavaScript pesado
4. **Responsive design**: Una sola versión sirve desktop y móvil (Bootstrap 5)

### Configuración NGINX para caché de estáticos:
```nginx
location /css/ {
    proxy_pass http://web_app;
    expires 7d;
    add_header Cache-Control "public, immutable";
}
location /js/ {
    proxy_pass http://web_app;
    expires 7d;
    add_header Cache-Control "public, immutable";
}
```

## 9.1 Escalabilidad de Base de Datos

### Estrategia 1: Read Replicas (SQL Server Always On)

```
                    ┌───────────────┐
     Escrituras ───▶│  PRIMARY      │
                    │  SQL Server   │
                    └───────┬───────┘
                            │ Replicación
                    ┌───────┴───────┐
                    ▼               ▼
             ┌──────────┐   ┌──────────┐
  Lecturas──▶│ REPLICA 1│   │ REPLICA 2│
             │ (Read)   │   │ (Read)   │
             └──────────┘   └──────────┘
```

### Estrategia 2: Particionamiento por zona

```sql
-- Particionar tabla Profesionales por zona para consultas más rápidas
CREATE PARTITION FUNCTION pf_Zona (UNIQUEIDENTIFIER)
AS RANGE LEFT FOR VALUES (
    -- IDs de zonas: cada zona en su partición
);
```

### Estrategia 3: Índices optimizados

```sql
-- Índices compuestos para las consultas más frecuentes
CREATE INDEX IX_Profesionales_Busqueda
    ON Profesionales(RubroId, ZonaId, Activo)
    INCLUDE (NombreCompleto, Descripcion, Telefono);

CREATE INDEX IX_Calificaciones_Visibles
    ON Calificaciones(ProfesionalId, Visible)
    INCLUDE (Puntaje);
```

### Estrategia 4: Connection pooling

```json
// appsettings.json — Pool de conexiones
{
  "ConnectionStrings": {
    "SqlServer": "Server=sqlserver;...;Min Pool Size=5;Max Pool Size=100;"
  }
}
```

### Métricas de escalabilidad objetivo:

| Métrica | Actual | Meta |
|---------|--------|------|
| Usuarios concurrentes | 50 | 500 |
| Tiempo respuesta (caché) | ~2ms | <5ms |
| Tiempo respuesta (BD) | ~200ms | <300ms |
| Instancias Web | 1 | 3 |
| Instancias API | 1 | 2 |
