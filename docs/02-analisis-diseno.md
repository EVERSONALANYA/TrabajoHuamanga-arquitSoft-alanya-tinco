# 2. Análisis y Diseño del Sistema

## 2.1 Modelo de Dominio

```
┌──────────────┐     ┌──────────────┐
│  Profesional │────▶│    Rubro     │
│──────────────│     │──────────────│
│ Id           │     │ Id           │
│ NombreCompl. │     │ Nombre       │
│ Telefono     │     │ Descripcion  │
│ Descripcion  │     │ Icono        │
│ FotoUrl      │     │ Activo       │
│ Activo       │     └──────────────┘
│ FechaRegistro│
└──────┬───────┘     ┌──────────────┐
       │────────────▶│    Zona      │
       │             │──────────────│
       │             │ Id           │
       ▼             │ Nombre       │
┌──────────────┐     │ Distrito     │
│ Calificacion │     │ Activo       │
│──────────────│     └──────────────┘
│ Id           │
│ ProfesionalId│
│ NombreUsuario│
│ Puntaje (1-5)│
│ Comentario   │
│ Reportada    │
│ MotivoReporte│
│ Visible      │
│ FechaCreacion│
└──────────────┘
```

## 2.2 Diagrama de Casos de Uso

```
                    ┌─────────────────────────────────────┐
                    │        Trabajo Huamanga              │
                    │                                     │
   ┌────────┐       │  ┌──────────────────────┐          │
   │Usuario │──────▶│  │ Buscar Profesionales │          │
   │        │──────▶│  │ Calificar Profesional│          │
   │        │──────▶│  │ Contactar WhatsApp   │          │
   │        │──────▶│  │ Sugerencia IA Rubro  │          │
   │        │──────▶│  │ Reportar Calificación│          │
   └────────┘       │  └──────────────────────┘          │
                    │                                     │
   ┌────────────┐   │  ┌──────────────────────┐          │
   │Profesional │──▶│  │ Registrarse          │          │
   │            │──▶│  │ Gestionar Perfil     │          │
   └────────────┘   │  └──────────────────────┘          │
                    │                                     │
   ┌───────────┐    │  ┌──────────────────────┐          │
   │   Admin   │───▶│  │ Moderar Calificac.   │          │
   │           │───▶│  │ Ver Estadísticas     │          │
   └───────────┘    │  └──────────────────────┘          │
                    └─────────────────────────────────────┘
```

## 2.3 Diagrama de Secuencia — Búsqueda con Caché

```
Usuario          Web Controller      BuscarProfesional     Redis Cache     SQL Server
  │                   │                    │                   │               │
  │ GET /?rubro=X     │                    │                   │               │
  │──────────────────▶│                    │                   │               │
  │                   │ EjecutarAsync(dto) │                   │               │
  │                   │───────────────────▶│                   │               │
  │                   │                    │ ObtenerAsync(key) │               │
  │                   │                    │──────────────────▶│               │
  │                   │                    │   null (miss)     │               │
  │                   │                    │◀──────────────────│               │
  │                   │                    │                   │ BuscarPorRubroYZona()
  │                   │                    │──────────────────────────────────▶│
  │                   │                    │                   │   resultados  │
  │                   │                    │◀──────────────────────────────────│
  │                   │                    │ GuardarAsync(5min)│               │
  │                   │                    │──────────────────▶│               │
  │                   │   List<Dto>        │                   │               │
  │                   │◀───────────────────│                   │               │
  │   HTML Renderizado│                    │                   │               │
  │◀──────────────────│                    │                   │               │
```

## 2.4 Diagrama de Secuencia — Sugerencia IA

```
Usuario         HomeController     SugerirRubro      OpenRouter API     RubroRepository
  │                  │                  │                  │                  │
  │ POST descripcion │                  │                  │                  │
  │─────────────────▶│                  │                  │                  │
  │                  │ EjecutarAsync()  │                  │                  │
  │                  │────────────────▶ │                  │                  │
  │                  │                  │ POST /completions│                  │
  │                  │                  │─────────────────▶│                  │
  │                  │                  │  "Gasfitero"     │                  │
  │                  │                  │◀─────────────────│                  │
  │                  │                  │ ObtenerPorNombre()                  │
  │                  │                  │────────────────────────────────────▶│
  │                  │                  │          Rubro entity               │
  │                  │                  │◀───────────────────────────────────│
  │                  │  SugerenciaDto   │                  │                  │
  │                  │◀────────────────│                  │                  │
  │  JSON response   │                  │                  │                  │
  │◀─────────────────│                  │                  │                  │
```

## 2.5 Diseño de Base de Datos (Modelo Relacional)

```sql
-- Tabla: Rubros
CREATE TABLE Rubros (
    Id          UNIQUEIDENTIFIER PRIMARY KEY,
    Nombre      NVARCHAR(100) NOT NULL UNIQUE,
    Descripcion NVARCHAR(500),
    Icono       NVARCHAR(10),
    Activo      BIT DEFAULT 1
);

-- Tabla: Zonas
CREATE TABLE Zonas (
    Id       UNIQUEIDENTIFIER PRIMARY KEY,
    Nombre   NVARCHAR(100) NOT NULL,
    Distrito NVARCHAR(100),
    Activo   BIT DEFAULT 1
);

-- Tabla: Profesionales
CREATE TABLE Profesionales (
    Id              UNIQUEIDENTIFIER PRIMARY KEY,
    NombreCompleto  NVARCHAR(200) NOT NULL,
    Telefono        NVARCHAR(20) NOT NULL,
    Descripcion     NVARCHAR(1000),
    FotoUrl         NVARCHAR(500),
    RubroId         UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Rubros(Id),
    ZonaId          UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Zonas(Id),
    Activo          BIT DEFAULT 1,
    FechaRegistro   DATETIME2 DEFAULT GETUTCDATE()
);

-- Tabla: Calificaciones
CREATE TABLE Calificaciones (
    Id              UNIQUEIDENTIFIER PRIMARY KEY,
    ProfesionalId   UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Profesionales(Id),
    NombreUsuario   NVARCHAR(100) NOT NULL,
    Puntaje         INT NOT NULL CHECK (Puntaje BETWEEN 1 AND 5),
    Comentario      NVARCHAR(500),
    Reportada       BIT DEFAULT 0,
    MotivoReporte   NVARCHAR(500),
    Visible         BIT DEFAULT 1,
    FechaCreacion   DATETIME2 DEFAULT GETUTCDATE()
);

-- Índices para rendimiento
CREATE INDEX IX_Profesionales_RubroId ON Profesionales(RubroId);
CREATE INDEX IX_Profesionales_ZonaId ON Profesionales(ZonaId);
CREATE INDEX IX_Calificaciones_ProfesionalId ON Calificaciones(ProfesionalId);
```
