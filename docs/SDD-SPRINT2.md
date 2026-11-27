# Software Design Document (SDD) - Sprint 2

## Proyecto: Trabajo Huamanga
### Directorio de Profesionales de Servicios en Huamanga, Ayacucho

| Campo | Detalle |
|-------|---------|
| **Curso** | IS-488 Arquitectura de Software |
| **Docente** | Ing. Lizbeth Jaico Quispe |
| **Alumno** | Everson Alanya Tinco |
| **Semestre** | 2026-II |
| **Sprint** | 2 - Implementacion y Validacion de la Arquitectura |

---

## 1. Introduccion

### 1.1 Proposito
Este documento describe el diseno de software del proyecto "Trabajo Huamanga", un directorio digital que permite a los vecinos de Huamanga buscar profesionales de servicios locales (electricistas, gasfiteros, pintores, etc.) y calificarlos mediante un sistema de reputacion.

### 1.2 Alcance
El sistema expone una API REST que permite:
- Registrar y buscar profesionales por rubro y distrito
- Registrar calificaciones (puntaje 1-5 con comentario)
- Consultar la reputacion de un profesional
- Moderar calificaciones (ocultar)

### 1.3 Stack Tecnologico
- **Lenguaje**: C# (.NET 8.0)
- **Patron**: Clean Architecture (4 capas)
- **API**: Minimal API con ASP.NET Core
- **Documentacion**: OpenAPI/Swagger (Swashbuckle 6.6.2)
- **Testing**: xUnit (unitarias), Postman (funcionales), k6 (carga)
- **CI/CD**: GitHub Actions (4 jobs)
- **Deploy**: Railway.app (Docker)
- **Base de datos**: In-Memory (actual) → PostgreSQL (planificado)

---

## 2. Arquitectura del Sistema

### 2.1 Estilo Arquitectonico: Clean Architecture

La aplicacion sigue Clean Architecture con 4 capas concentricas + capa de pruebas:

```
+--------------------------------------------------+
|                   Api (Presentacion)              |
|  Program.cs - Minimal API endpoints              |
+--------------------------------------------------+
         |                        |
+--------------------------------------------------+
|              Aplicacion (Casos de Uso)            |
|  RegistrarCalificacion, Validadores, Comandos     |
+--------------------------------------------------+
         |                        |
+--------------------------------------------------+
|              Dominio (Entidades)                  |
|  Profesional, Calificacion, Interfaces            |
+--------------------------------------------------+
         |                        |
+--------------------------------------------------+
|           Infraestructura (Adaptadores)           |
|  CalificacionRepositoryEnMemoria                  |
+--------------------------------------------------+
         |
+--------------------------------------------------+
|                   Pruebas                         |
|  xUnit, Postman, k6                              |
+--------------------------------------------------+
```

### 2.2 Regla de Dependencia
Las dependencias SOLO apuntan hacia adentro:
- **Api** depende de Aplicacion e Infraestructura
- **Aplicacion** depende SOLO de Dominio
- **Dominio** no depende de nadie (nucleo)
- **Infraestructura** depende de Dominio (implementa interfaces)

---

## 3. Especificaciones Funcionales

### 3.1 Funcionalidad: Busqueda de Profesionales

| Campo | Descripcion |
|-------|-------------|
| **ID** | HU-001 |
| **Nombre** | Buscar profesionales por rubro y/o distrito |
| **Descripcion** | Como vecino, quiero buscar profesionales filtrando por rubro y distrito para encontrar el servicio que necesito |
| **Endpoint** | `GET /api/profesionales/buscar?rubro={rubro}&distrito={distrito}` |
| **Metodo HTTP** | GET |

#### Criterios de Aceptacion:
1. **CA-001**: Dado que existen profesionales registrados, cuando busco por rubro "Electricista", entonces recibo una lista con solo los electricistas (Status 200)
2. **CA-002**: Dado que existen profesionales en distintos distritos, cuando busco por distrito "Ayacucho", entonces recibo solo los profesionales de ese distrito (Status 200)
3. **CA-003**: Dado que busco por rubro y distrito a la vez, cuando ambos filtros aplican, entonces recibo la interseccion de ambos filtros (Status 200)
4. **CA-004**: Dado que busco sin filtros, cuando no paso parametros, entonces recibo todos los profesionales (Status 200)
5. **CA-005**: Dado que busco un rubro inexistente, cuando el filtro no coincide con nadie, entonces recibo un array vacio (Status 200)

#### Tareas Tecnicas:
- [x] Implementar endpoint GET con query parameters opcionales
- [x] Aplicar filtrado case-insensitive con `String.Contains`
- [x] Retornar lista vacia (no error) cuando no hay coincidencias
- [x] Documentar con Swagger

---

### 3.2 Funcionalidad: Registrar Calificacion

| Campo | Descripcion |
|-------|-------------|
| **ID** | HU-002 |
| **Nombre** | Calificar a un profesional |
| **Descripcion** | Como vecino, quiero calificar a un profesional con puntaje del 1 al 5 y un comentario para ayudar a otros vecinos |
| **Endpoint** | `POST /api/calificaciones` |
| **Metodo HTTP** | POST |

#### Request Body:
```json
{
  "vecinoId": "guid",
  "profesionalId": "guid",
  "puntaje": 4,
  "comentario": "Excelente trabajo"
}
```

#### Criterios de Aceptacion:
1. **CA-006**: Dado que el puntaje esta entre 1 y 5, cuando registro una calificacion, entonces se crea exitosamente (Status 201, mensaje "Calificacion registrada")
2. **CA-007**: Dado que el puntaje es menor que 1 o mayor que 5, cuando intento registrar, entonces recibo error de validacion (Status 400, error "El puntaje debe estar entre 1 y 5.")
3. **CA-008**: Dado que el vecino ya califico a ese profesional, cuando intenta calificar de nuevo, entonces recibo error (Status 400, error "El vecino ya califico a este profesional.")
4. **CA-009**: Dado datos validos, cuando registro la calificacion, entonces los validadores se ejecutan en cadena (puntaje primero, luego duplicado)

#### Tareas Tecnicas:
- [x] Crear comando `RegistrarCalificacionComando` con propiedades requeridas
- [x] Implementar caso de uso `RegistrarCalificacion` con inyeccion de validadores
- [x] Implementar `ValidadorPuntaje` (rango 1-5)
- [x] Implementar `ValidadorNoRepetida` (consulta al repositorio)
- [x] Configurar DI con `AddTransient<IValidadorCalificacion>`

---

### 3.3 Funcionalidad: Consultar Reputacion

| Campo | Descripcion |
|-------|-------------|
| **ID** | HU-003 |
| **Nombre** | Ver reputacion de un profesional |
| **Descripcion** | Como vecino, quiero ver el promedio de calificaciones de un profesional para decidir si contratarlo |
| **Endpoint** | `GET /api/reputacion/{profesionalId}` |
| **Metodo HTTP** | GET |

#### Response:
```json
{
  "profesionalId": "guid",
  "promedio": 4.5,
  "total": 10
}
```

#### Criterios de Aceptacion:
1. **CA-010**: Dado que un profesional tiene calificaciones, cuando consulto su reputacion, entonces recibo el promedio y total de calificaciones no ocultas (Status 200)
2. **CA-011**: Dado que un profesional no tiene calificaciones, cuando consulto su reputacion, entonces recibo promedio 0 y total 0 (Status 200)
3. **CA-012**: Dado que algunas calificaciones estan ocultas, cuando consulto reputacion, entonces solo cuentan las no ocultas

#### Tareas Tecnicas:
- [x] Implementar `CalcularReputacion` como metodo estatico en entidad `Profesional`
- [x] Filtrar calificaciones ocultas en el calculo
- [x] Retornar `profesionalId`, `promedio` y `total`

---

### 3.4 Funcionalidad: Moderar Calificacion

| Campo | Descripcion |
|-------|-------------|
| **ID** | HU-004 |
| **Nombre** | Ocultar calificacion inapropiada |
| **Descripcion** | Como administrador, quiero poder ocultar calificaciones inapropiadas para mantener la calidad del directorio |
| **Endpoint** | `PATCH /api/calificaciones/{id}/ocultar` |
| **Metodo HTTP** | PATCH |

#### Criterios de Aceptacion:
1. **CA-013**: Dado un ID de calificacion valido, cuando ejecuto ocultar, entonces recibo confirmacion (Status 200, mensaje "Calificacion ocultada")
2. **CA-014**: Dado que una calificacion fue ocultada, cuando consulto reputacion, entonces esa calificacion no cuenta en el promedio

#### Tareas Tecnicas:
- [x] Implementar endpoint PATCH
- [x] Marcar propiedad `Oculta = true` en la calificacion
- [x] Verificar que el calculo de reputacion excluye ocultas

---

### 3.5 Funcionalidad: CRUD de Profesionales

| Campo | Descripcion |
|-------|-------------|
| **ID** | HU-005 |
| **Nombre** | Gestionar profesionales (crear, listar, actualizar, ver detalle) |
| **Endpoints** | `GET /api/profesionales`, `GET /api/profesionales/{id}`, `POST /api/profesionales`, `PUT /api/profesionales/{id}` |

#### Criterios de Aceptacion:
1. **CA-015**: Listar todos los profesionales retorna array con objetos que tienen id, nombre, rubro, distrito (Status 200)
2. **CA-016**: Obtener por ID existente retorna el profesional (Status 200)
3. **CA-017**: Obtener por ID inexistente retorna error (Status 404, error "Profesional no encontrado")
4. **CA-018**: Crear profesional genera un nuevo ID y retorna el objeto (Status 201)
5. **CA-019**: Actualizar profesional modifica nombre, rubro y distrito (Status 200)

---

## 4. Principios SOLID Aplicados

### 4.1 S - Single Responsibility Principle (SRP)
**Principio**: Cada clase debe tener una unica razon para cambiar.

**Aplicacion en el proyecto**:

| Clase | Responsabilidad Unica |
|-------|----------------------|
| `Profesional` | Representar la entidad de dominio y calcular reputacion |
| `Calificacion` | Representar la entidad de calificacion |
| `ValidadorPuntaje` | Validar que el puntaje este entre 1 y 5 |
| `ValidadorNoRepetida` | Validar que no exista calificacion duplicada |
| `RegistrarCalificacion` | Orquestar el caso de uso de registrar calificacion |
| `CalificacionRepositoryEnMemoria` | Persistir calificaciones en memoria |

**Ejemplo concreto** - `ValidadorPuntaje.cs`:
```csharp
public class ValidadorPuntaje : IValidadorCalificacion
{
    // SOLO valida el rango del puntaje, nada mas
    public Task<Resultado> ValidarAsync(RegistrarCalificacionComando comando)
    {
        var resultado = comando.Puntaje is >= 1 and <= 5
            ? Resultado.Ok()
            : Resultado.Falla("El puntaje debe estar entre 1 y 5.");
        return Task.FromResult(resultado);
    }
}
```

---

### 4.2 O - Open/Closed Principle (OCP)
**Principio**: Las clases deben estar abiertas para extension, cerradas para modificacion.

**Aplicacion en el proyecto**:
- La interfaz `IValidadorCalificacion` permite agregar nuevos validadores SIN modificar el caso de uso `RegistrarCalificacion`
- Para agregar una nueva regla de validacion (ej: "maximo 3 calificaciones por dia"), solo se crea una nueva clase que implemente `IValidadorCalificacion` y se registra en el DI

**Ejemplo** - Agregar un nuevo validador sin tocar codigo existente:
```csharp
// Nuevo validador - NO modifica clases existentes
public class ValidadorLimiteDiario : IValidadorCalificacion
{
    public async Task<Resultado> ValidarAsync(RegistrarCalificacionComando cmd)
    {
        // nueva logica aqui
    }
}

// Solo registrar en DI (Program.cs):
builder.Services.AddTransient<IValidadorCalificacion, ValidadorLimiteDiario>();
```

---

### 4.3 L - Liskov Substitution Principle (LSP)
**Principio**: Los subtipos deben poder sustituir a sus tipos base sin alterar el comportamiento.

**Aplicacion en el proyecto**:
- `ValidadorPuntaje` y `ValidadorNoRepetida` ambos implementan `IValidadorCalificacion`
- El caso de uso `RegistrarCalificacion` recibe `IEnumerable<IValidadorCalificacion>` y ejecuta cada validador de forma indistinta
- Se puede sustituir cualquier validador por otro sin romper la cadena de validacion

**Ejemplo** - `RegistrarCalificacion.cs`:
```csharp
public class RegistrarCalificacion
{
    private readonly IEnumerable<IValidadorCalificacion> _validadores;

    // Cualquier implementacion de IValidadorCalificacion funciona aqui
    public async Task<Resultado> EjecutarAsync(RegistrarCalificacionComando cmd)
    {
        foreach (var validador in _validadores) // LSP en accion
        {
            var resultado = await validador.ValidarAsync(cmd);
            if (!resultado.Exito) return resultado;
        }
        // ...
    }
}
```

---

### 4.4 I - Interface Segregation Principle (ISP)
**Principio**: Los clientes no deben depender de interfaces que no usan.

**Aplicacion en el proyecto**:
- `IValidadorCalificacion` tiene un solo metodo: `ValidarAsync()` - interfaz minima y enfocada
- `ICalificacionRepository` define solo los metodos necesarios para el repositorio de calificaciones, separado de la gestion de profesionales
- No se usa una interfaz monolitica `IRepository<T>` con operaciones CRUD genericas que no todos necesitan

**Interfaces del proyecto**:
```
IValidadorCalificacion     → ValidarAsync(comando)
ICalificacionRepository    → RegistrarAsync(), ObtenerPorProfesionalAsync(), ExisteAsync()
```

---

### 4.5 D - Dependency Inversion Principle (DIP)
**Principio**: Los modulos de alto nivel no deben depender de modulos de bajo nivel. Ambos deben depender de abstracciones.

**Aplicacion en el proyecto**:
- **Dominio** define las interfaces (`ICalificacionRepository`, `IValidadorCalificacion`)
- **Infraestructura** implementa las interfaces (`CalificacionRepositoryEnMemoria`)
- **Aplicacion** depende de las abstracciones, no de las implementaciones concretas
- **Api** (Program.cs) configura la inyeccion de dependencias

**Configuracion DI en Program.cs**:
```csharp
// Alto nivel (Aplicacion) NO conoce la implementacion
// Solo depende de la abstraccion ICalificacionRepository
builder.Services.AddSingleton<ICalificacionRepository, CalificacionRepositoryEnMemoria>();
builder.Services.AddTransient<IValidadorCalificacion, ValidadorPuntaje>();
builder.Services.AddTransient<IValidadorCalificacion, ValidadorNoRepetida>();
builder.Services.AddTransient<RegistrarCalificacion>();

// Cambiar a PostgreSQL = cambiar UNA linea:
// builder.Services.AddScoped<ICalificacionRepository, CalificacionRepositoryPostgres>();
```

---

## 5. Patrones de Diseno Aplicados

### 5.1 Repository Pattern
- **Donde**: `ICalificacionRepository` (Dominio) → `CalificacionRepositoryEnMemoria` (Infraestructura)
- **Por que**: Abstrae el acceso a datos, permitiendo cambiar de In-Memory a PostgreSQL sin modificar la logica de negocio

### 5.2 Command Pattern
- **Donde**: `RegistrarCalificacionComando` como DTO de entrada
- **Por que**: Encapsula la intencion del usuario en un objeto inmutable que viaja desde la API hasta el caso de uso

### 5.3 Chain of Responsibility
- **Donde**: Cadena de `IValidadorCalificacion` ejecutados secuencialmente en `RegistrarCalificacion`
- **Por que**: Permite agregar o remover validaciones sin modificar el flujo principal

### 5.4 Result Pattern
- **Donde**: Clase `Resultado` con `Exito`, `Error`, metodos `Ok()` y `Falla()`
- **Por que**: Evita excepciones para control de flujo, retorna errores de forma explicita

---

## 6. Endpoints REST Documentados

| # | Metodo | Ruta | Descripcion | Status Codes |
|---|--------|------|-------------|-------------|
| 1 | GET | `/health` | Health check | 200 |
| 2 | GET | `/api/rubros` | Listar rubros disponibles | 200 |
| 3 | GET | `/api/zonas` | Listar distritos/zonas | 200 |
| 4 | GET | `/api/profesionales` | Listar todos los profesionales | 200 |
| 5 | GET | `/api/profesionales/{id}` | Obtener profesional por ID | 200, 404 |
| 6 | GET | `/api/profesionales/buscar` | Buscar por rubro y/o distrito | 200 |
| 7 | POST | `/api/profesionales` | Registrar nuevo profesional | 201 |
| 8 | PUT | `/api/profesionales/{id}` | Actualizar profesional | 200, 404 |
| 9 | POST | `/api/calificaciones` | Registrar calificacion | 201, 400 |
| 10 | GET | `/api/calificaciones/profesional/{pid}` | Calificaciones de un profesional | 200 |
| 11 | GET | `/api/reputacion/{pid}` | Reputacion de un profesional | 200 |
| 12 | PATCH | `/api/calificaciones/{id}/ocultar` | Ocultar calificacion | 200 |

**Documentacion interactiva**: Swagger UI disponible en `/swagger`

---

## 7. Estructura del Proyecto

```
TrabajoHuamanga/
├── src/
│   ├── TrabajoHuamanga.Dominio/           # Capa de Dominio (nucleo)
│   │   ├── Profesional.cs                 # Entidad principal
│   │   ├── Calificacion.cs                # Entidad de calificacion
│   │   ├── ICalificacionRepository.cs     # Interfaz del repositorio
│   │   └── IValidadorCalificacion.cs      # Interfaz de validacion
│   │
│   ├── TrabajoHuamanga.Aplicacion/        # Capa de Aplicacion
│   │   ├── RegistrarCalificacion.cs       # Caso de uso
│   │   ├── RegistrarCalificacionComando.cs # Comando (DTO)
│   │   ├── Resultado.cs                   # Patron Result
│   │   └── Validadores/
│   │       ├── ValidadorPuntaje.cs        # SRP: solo valida rango
│   │       └── ValidadorNoRepetida.cs     # SRP: solo valida duplicados
│   │
│   ├── TrabajoHuamanga.Infraestructura/   # Capa de Infraestructura
│   │   └── CalificacionRepositoryEnMemoria.cs # Implementacion concreta
│   │
│   └── TrabajoHuamanga.Api/               # Capa de Presentacion
│       ├── Program.cs                     # Minimal API + DI
│       └── Dockerfile                     # Multi-stage build
│
├── tests/
│   ├── TrabajoHuamanga.Pruebas/           # Tests unitarios (xUnit)
│   └── k6/
│       └── load-test.js                   # Pruebas de carga (hasta 2,000 VUs)
│
├── docs/
│   ├── SDD-SPRINT2.md                     # Este documento
│   ├── ANALISIS-BD-CACHE.md               # Analisis de BD y cache
│   └── DOCUMENTACION-TECNICA-SPRINT3.md   # Documentacion Sprint 3
│
└── .github/workflows/
    └── ci-cd.yml                          # Pipeline CI/CD (4 jobs)
```

---

## 8. Pruebas

### 8.1 Pruebas Unitarias (xUnit)
- 4 tests en `TrabajoHuamanga.Pruebas`
- Validacion del puntaje (rango 1-5)
- Validacion de calificacion no repetida
- Integrados en CI/CD (GitHub Actions)

### 8.2 Pruebas Funcionales (Postman)
- 23 requests organizados en 7 carpetas
- 52 assertions — **52/52 pasando**
- Cubren todos los 12 endpoints

### 8.3 Pruebas de Carga (k6)
- Simulacion progresiva: 50 → 200 → 500 → 1,000 → 2,000 VUs
- 10 escenarios de prueba por iteracion
- Metricas: tiempo de respuesta P95, tasa de error, requests/segundo
- Umbrales: P95 < 3s, error rate < 15%

---

## 9. Pipeline CI/CD

```
Push/PR a main
      │
      ▼
┌─────────────────┐
│ build-and-test  │  Restaurar → Compilar → Ejecutar xUnit
└────────┬────────┘
         │ (si pasa)
         ▼
┌─────────────────┐
│ code-analysis   │  Analisis de codigo con dotnet format
└────────┬────────┘
         │ (si pasa)
         ▼
┌─────────────────┐
│ docker-build    │  Build multi-stage de la imagen Docker
└────────┬────────┘
         │ (si pasa)
         ▼
┌─────────────────┐
│    deploy       │  Deploy automatico a Railway.app
└─────────────────┘
```

---

## 10. Despliegue

- **Plataforma**: Railway.app (PaaS)
- **URL**: `https://trabajohuamanga-arquitsoft-alanya-tinco-production.up.railway.app`
- **Contenedor**: Docker multi-stage (sdk:8.0 → aspnet:8.0)
- **Puerto**: Variable de entorno `PORT` (asignado por Railway)
- **Swagger**: `/swagger` (accesible en produccion)
