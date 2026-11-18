# Informe Final — Trabajo Huamanga

## Información del Proyecto

| Campo | Detalle |
|-------|---------|
| **Proyecto** | Trabajo Huamanga — Directorio de Profesionales de Servicios |
| **Curso** | IS-488 Arquitectura de Software |
| **Docente** | Ing. Lizbeth Jaico Quispe |
| **Alumno** | Everson Alanya Tinco |
| **Universidad** | Universidad Nacional de San Cristóbal de Huamanga (UNSCH) |
| **Escuela** | Ingeniería de Sistemas |
| **Fecha** | Noviembre 2024 |

---

## 1. Resumen Ejecutivo

Trabajo Huamanga es una plataforma web que resuelve la falta de un directorio centralizado de profesionales de servicios en la ciudad de Huamanga. La aplicación permite a los usuarios buscar, calificar y contactar profesionales locales (electricistas, gasfiteros, albañiles, etc.) de manera rápida, con un sistema de reputación transparente y contacto directo vía WhatsApp.

El sistema se desarrolló con **Clean Architecture** sobre **ASP.NET Core 8**, utilizando **SQL Server** como base de datos, **Redis** como caché de búsquedas, e integración con **OpenRouter (Gemini Flash)** para sugerencia de rubro por IA.

## 2. Problema Identificado

Los habitantes de Huamanga no cuentan con un medio digital centralizado para encontrar profesionales de servicios confiables. Dependen del boca a boca, lo que resulta en:
- Dificultad para encontrar al profesional adecuado
- Falta de información sobre la calidad del servicio
- Pérdida de tiempo buscando contactos
- Los profesionales independientes no tienen visibilidad

## 3. Solución Propuesta

Una plataforma web con 6 módulos funcionales y 3 roles de usuario:

### Módulos:
1. **Profesionales**: Registro y gestión de perfiles
2. **Búsqueda**: Filtrado por rubro, zona y término (con caché Redis ~2ms)
3. **Reputación**: Calificaciones 1-5 estrellas con comentarios
4. **Contacto**: Enlace directo a WhatsApp (gratis, sin API)
5. **Sugerencia de Rubro**: IA sugiere qué profesional necesitas
6. **Moderación**: Gestión administrativa de contenido reportado

### Roles:
- **Usuario**: Busca, califica, contacta
- **Profesional**: Se registra, gestiona perfil
- **Administrador**: Modera calificaciones

## 4. Arquitectura

### Estilo: Monolito Modular con Clean Architecture

4 capas con regla de dependencia hacia adentro:
- **Presentación**: Web MVC + API REST
- **Aplicación**: Casos de uso y DTOs
- **Dominio**: Entidades e interfaces (sin dependencias externas)
- **Infraestructura**: EF Core, Redis, OpenRouter, WhatsApp

### Stack Tecnológico:
- ASP.NET Core 8 (Backend + Frontend MVC)
- Entity Framework Core 8 (ORM)
- SQL Server 2022 (Base de datos)
- Redis 7 (Caché distribuido)
- Bootstrap 5 + Font Awesome (UI)
- Docker Compose (Contenedores)
- NGINX (Reverse proxy)
- GitHub Actions (CI/CD)
- k6 (Pruebas de carga)

## 5. Desarrollo Basado en SDD

El desarrollo siguió el Software Design Document (SDD) que definió:
- Modelo de dominio con 4 entidades
- 14 endpoints REST documentados
- 4 vistas MVC con roles diferenciados
- Estrategia de caché Cache-Aside con TTL 5 minutos
- Diagramas de secuencia para cada flujo principal

## 6. Pruebas

### 6.1 Pruebas Unitarias (xUnit + Moq)
- Tests de entidades de dominio (Profesional, Calificacion, Rubro)
- Tests de casos de uso (BuscarProfesional con mock de caché)
- Tests de adaptadores (GeneradorEnlaceWhatsApp)

### 6.2 Pruebas de Integración
- Tests end-to-end de la API REST (8 tests)
- Verificación de vistas MVC (9 tests)
- Base de datos en memoria para aislamiento

### 6.3 Pruebas de Carga (k6)
- Prueba de carga: rampa hasta 500 usuarios concurrentes
- Prueba de estrés: todos los endpoints API
- Threshold: p95 < 300ms, tasa error < 5%

## 7. Atributos de Calidad Alcanzados

| Atributo | Meta | Mecanismo | Estado |
|----------|------|-----------|--------|
| Rendimiento | <300ms | Redis caché (TTL 5min) | ✅ |
| Escalabilidad | 500 usuarios | Docker horizontal scaling | ✅ |
| Mantenibilidad | <8h nuevo módulo | Clean Architecture modular | ✅ |
| Disponibilidad | 99.5% | Docker health checks, degradación elegante | ✅ |
| Seguridad | Validación completa | Validación en entidades + DTOs | ✅ |

## 8. Infraestructura

### Docker Compose (5 servicios):
1. `sqlserver`: SQL Server 2022 con health checks
2. `redis`: Redis 7 Alpine con persistencia
3. `web`: Frontend MVC (.NET 8)
4. `api`: REST API (.NET 8) con Swagger
5. `nginx`: Reverse proxy y load balancer

### CI/CD (GitHub Actions):
1. Build y compilación automática
2. Ejecución de pruebas unitarias e integración
3. Build de imágenes Docker
4. Validación de scripts k6
5. Deploy automatizado a producción

### DNS y CDN:
- Dominio configurado con subdominio para API
- Cloudflare como CDN (caché estáticos, SSL, DDoS)
- NGINX con headers de caché para assets

## 9. Repositorio GitHub

**URL**: https://github.com/EVERSONALANYA/TrabajoHuamanga

Contenido:
- Código fuente completo (6 proyectos .NET)
- Docker Compose para despliegue
- Documentación completa en `/docs`
- Scripts de pruebas k6
- Pipeline CI/CD
- README con instrucciones

## 10. Lecciones Aprendidas

1. **Clean Architecture** facilita enormemente las pruebas unitarias gracias a la inversión de dependencias
2. **Redis como caché** reduce drásticamente los tiempos de respuesta con mínima complejidad
3. **WhatsApp wa.me links** son una solución elegante y gratuita para contacto directo
4. **Docker Compose** simplifica el despliegue de múltiples servicios
5. La integración de **IA (Gemini Flash)** es accesible y económica para clasificación simple

## 11. Trabajo Futuro

- Autenticación con Identity Server o JWT
- Panel del profesional para gestionar perfil
- Notificaciones push al profesional
- Geolocalización para búsqueda por cercanía
- App móvil con .NET MAUI
- Métricas de uso con Application Insights

---

**Everson Alanya Tinco**  
Ingeniería de Sistemas — UNSCH  
IS-488 Arquitectura de Software  
Noviembre 2024
