# 04 — Atributos de calidad

Los atributos de calidad indican **cómo** debe funcionar el sistema, más allá de **qué** hace.

**Escenario de análisis:** a medida que la plataforma se difunde en Huamanga, crece el número de profesionales registrados y de clientes que buscan servicios al mismo tiempo, sobre todo en horarios de mayor demanda (fines de semana, emergencias domésticas). Las búsquedas por rubro y zona son la operación más frecuente.

| ID | Atributo | Escenario de calidad | Medida orientativa |
|---|---|---|---|
| AC01 | **Rendimiento** | Las búsquedas por rubro y zona deben responder rápidamente aunque crezca el número de profesionales y de clientes concurrentes. | Búsquedas en menos de 2 s en el 95 % de las solicitudes; las búsquedas frecuentes se sirven desde caché (Redis). |
| AC02 | **Escalabilidad** | El sistema debe soportar un incremento de usuarios agregando instancias de la API, sin cambios en el código. | Soportar al menos 5 veces la carga normal agregando réplicas de la API, verificado con pruebas de carga. |
| AC03 | **Disponibilidad** | La búsqueda y el contacto deben seguir funcionando aunque falle un servicio externo. | Si OpenRouter no responde en 5 s, el cliente elige el rubro manualmente y la búsqueda continúa. |
| AC04 | **Seguridad** | Los datos personales y la reputación de los profesionales deben estar protegidos frente a accesos no autorizados y manipulación. | HTTPS, contraseñas con hash, autorización por rol y una sola calificación por cliente y profesional. |
| AC05 | **Mantenibilidad** | El sistema debe permitir cambios y correcciones sin afectar innecesariamente otras funcionalidades. | Módulos y capas separados; cambiar el proveedor de IA no requiere modificar el módulo de Reputación. |
| AC06 | **Testabilidad** | Las reglas de negocio deben poder verificarse de forma automática y aislada. | Desarrollo con TDD; pruebas unitarias del dominio, de integración de la API y de carga de la búsqueda. |
| AC07 | **Portabilidad** | El sistema debe poder desplegarse en cualquier proveedor de infraestructura. | El sistema completo se levanta con Docker Compose en un servidor propio o en la nube. |
| AC08 | **Usabilidad** | Un cliente debe poder encontrar y contactar a un profesional sin ayuda, desde computadora o celular. | Búsqueda y contacto en 3 pasos o menos; interfaz responsive. |
