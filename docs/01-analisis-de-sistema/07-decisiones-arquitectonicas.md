# Decisiones arquitectónicas (ADR) - Trabajo Huamanga

Un ADR (Architecture Decision Record) documenta una decisión importante de diseño junto con su justificación.

| ID | Decisión arquitectónica | Driver relacionado | Justificación | Resultado |
| --- | --- | --- | --- | --- |
| ADR-001 | Monolito modular | DA01 - Escalabilidad; DA06 - Mantenibilidad | Organizar las funcionalidades en módulos independientes dentro de una misma aplicación desplegable, más simple de operar que microservicios para un proyecto de este tamaño. | Módulos de Profesionales, Búsqueda, Reputación, Contacto, Sugerencia de rubro y Moderación/Reportes. |
| ADR-002 | Clean Architecture | DA06 - Mantenibilidad | Separar las reglas del negocio de los detalles tecnológicos. | Capas de Dominio, Aplicación, Infraestructura y Presentación. |
| ADR-003 | Estrategia de caché con Redis | DA02 - Rendimiento | Reducir consultas repetitivas a la base de datos en las búsquedas más frecuentes por rubro y zona. | Caché de resultados de búsqueda frecuentes. |
| ADR-004 | Integración externa mediante interfaces y adaptadores | DA04 - Integración externa | Desacoplar los casos de uso de WhatsApp y de OpenRouter, para poder cambiar de proveedor sin tocar el dominio. | Contratos (interfaces) y adaptadores para el enlace de WhatsApp y el cliente de IA. |
| ADR-005 | API REST entre frontend y backend | DA05 - API REST | Separar la interfaz del backend con un contrato claro. | API REST consumida por la aplicación web y el panel de administración. |
| ADR-006 | Validación y moderación de calificaciones | DA03 - Seguridad e integridad | Reducir calificaciones fraudulentas o repetidas por la misma fuente. | Validadores, motor de reputación y panel de moderación. |
| ADR-007 | Despliegue en contenedores Docker | DA01 - Escalabilidad | Reproducir el mismo entorno en desarrollo y producción, sin depender de un proveedor de infraestructura. | Réplicas de la API detrás de un proxy reverso. |

## Alternativas descartadas

- **Microservicios (ADR-001):** se descartan por la complejidad operativa que añaden sin que el volumen de usuarios actual lo justifique. El monolito modular deja abierta la posibilidad de separar módulos en el futuro.
- **Arquitectura en capas simple sin Clean Architecture (ADR-002):** se descarta porque acopla las reglas de negocio al framework y a la base de datos, y dificulta las pruebas unitarias exigidas por TDD.
- **Mensajería interna propia (ADR-004):** queda fuera de alcance; se deriva el contacto a WhatsApp, canal que los usuarios ya utilizan.

## Consecuencias

- Un solo despliegue y una sola base de datos simplifican la operación, pero exigen disciplina para que los módulos no accedan a las tablas de otros.
- Redis agrega un componente más que operar, a cambio de menor carga sobre SQL Server.
- La IA es solo una sugerencia editable por el vecino, por lo que una falla del proveedor externo no bloquea la búsqueda.
