# C4 - Nivel 1: Diagrama de contexto del sistema - Trabajo Huamanga

Muestra el sistema en su entorno: quiénes lo usan y con qué sistemas externos se relaciona.

```mermaid
flowchart TD
    Vecino["Vecino<br/>Persona<br/>Busca y califica profesionales"]
    Prof["Profesional<br/>Persona<br/>Registra su perfil y servicios"]
    Admin["Administrador<br/>Persona<br/>Modera perfiles y calificaciones"]

    Sistema["Trabajo Huamanga<br/>Sistema<br/>Directorio de servicios con reputacion entre vecinos"]

    WA["WhatsApp<br/>Sistema externo<br/>Contacto directo con el profesional"]
    OR["OpenRouter<br/>Sistema externo<br/>IA que sugiere el rubro"]

    Vecino -->|"Busca, contacta y califica"| Sistema
    Prof -->|"Publica su perfil"| Sistema
    Admin -->|"Modera"| Sistema
    Sistema -->|"Genera enlace wa.me"| WA
    Sistema -->|"Consulta el rubro sugerido - API REST"| OR
```

| Elemento | Descripción |
| --- | --- |
| Vecino | Persona de Huamanga que busca un servicio, contacta al profesional y deja una calificación. |
| Profesional | Persona que ofrece servicios (electricista, gasfitero, tutor, etc.) y mantiene su perfil. |
| Administrador | Revisa reportes y oculta calificaciones o perfiles indebidos. |
| Trabajo Huamanga | Sistema que se construye. |
| WhatsApp | Sistema externo usado para el contacto. |
| OpenRouter | Sistema externo de IA usado para sugerir el rubro. |
