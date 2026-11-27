# Trazabilidad: driver, escenario, decisión y componente - Trabajo Huamanga

Esta tabla responde la pregunta de la exposición: de qué requisito o driver sale cada componente del diagrama.

| Driver | Escenario de calidad | Decisión (ADR) | Componente o elemento que lo implementa | Capa |
| --- | --- | --- | --- | --- |
| DA01 - Escalabilidad | EQ-02 | ADR-001 Monolito modular, ADR-007 Docker | API sin estado en réplicas detrás de NGINX | Despliegue |
| DA02 - Rendimiento | EQ-01 | ADR-003 Caché con Redis | Módulo Búsqueda, caché Redis, índices por rubro y zona | Aplicación e Infraestructura |
| DA03 - Seguridad e integridad | EQ-03 | ADR-006 Validación y moderación | Módulo Reputación (validadores en cadena), Módulo Moderación y reportes | Dominio y Aplicación |
| DA04 - Integración externa | EQ-04 | ADR-004 Interfaces y adaptadores | Módulo Contacto (enlace wa.me), Módulo Sugerencia de rubro (adaptador de OpenRouter) | Aplicación e Infraestructura |
| DA05 - API REST | (todos) | ADR-005 API REST | Controladores API REST | Presentación |
| DA06 - Mantenibilidad | EQ-05 | ADR-001 Monolito modular, ADR-002 Clean Architecture | Separación en capas y módulos; boilerplate en `tecnologia/` | Todas |

## Preguntas típicas de la exposición

| Pregunta | Respuesta corta |
| --- | --- |
| ¿Por qué un monolito y no microservicios? | El volumen no justifica la complejidad operativa. El monolito modular permite separar módulos más adelante (ADR-001). |
| ¿Por qué existe Redis en el diagrama? | Las búsquedas por rubro y zona son las más frecuentes; la caché reduce consultas repetidas (DA02, EQ-01). |
| ¿Por qué WhatsApp y OpenRouter están fuera del monolito? | Son sistemas externos; se usan mediante adaptadores para poder cambiarlos (DA04, ADR-004). |
| ¿Qué pasa si falla OpenRouter? | Se usa la selección manual de rubro y la búsqueda sigue funcionando (EQ-04, Circuit Breaker). |
| ¿Cómo se evitan las calificaciones falsas? | Validadores en cadena y moderación del administrador (DA03, EQ-03). |
| ¿Cómo se cambia el proveedor de IA sin romper nada? | Se escribe otro adaptador que implemente `ISugeridorRubro`; el caso de uso no cambia (DA06, EQ-05). |
