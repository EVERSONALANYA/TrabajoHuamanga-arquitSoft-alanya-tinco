# Escenarios de atributos de calidad de Trabajo Huamanga

Cada escenario describe qué ocurre, cómo debe responder el sistema y con qué valor se comprueba. Las medidas son propuestas por validar con el equipo y con pruebas.

## EQ-01. Rendimiento: búsqueda de profesionales por rubro y zona

| Elemento | Descripción |
|---|---|
| **Atributo de calidad** | Rendimiento (DA02) |
| **Fuente** | Vecino que busca un profesional |
| **Estímulo** | Busca un rubro (por ejemplo, gasfitero) filtrado por distrito. |
| **Artefacto** | Módulo de Búsqueda, caché Redis y consulta a SQL Server |
| **Entorno** | Funcionamiento normal con muchas búsquedas simultáneas |
| **Respuesta esperada** | El sistema devuelve la lista de profesionales; las búsquedas repetidas se atienden desde la caché. |
| **Medida** | **Propuesta por validar:** el 95 % de las búsquedas responde en 2 segundos o menos, con menos del 1 % de errores técnicos, con 200 usuarios simultáneos. |
| **Verificación** | Prueba de carga que registre tiempos de respuesta, errores y tasa de aciertos de la caché. |
| **Estado** | Pendiente de aprobar las medidas y definir el entorno de prueba. |

## EQ-02. Escalabilidad: crecimiento de profesionales y vecinos

| Elemento | Descripción |
|---|---|
| **Atributo de calidad** | Escalabilidad (DA01) |
| **Fuente** | Aumento de profesionales registrados y de vecinos que usan la plataforma |
| **Estímulo** | La cantidad de usuarios simultáneos crece progresivamente. |
| **Artefacto** | API Web (ASP.NET Core 8) en contenedores Docker, detrás de NGINX |
| **Entorno** | Prueba con datos y configuración documentados |
| **Respuesta esperada** | Se agregan réplicas de la API sin cambiar el código y se mantienen los tiempos de respuesta acordados. |
| **Medida** | **Propuesta por validar:** soportar hasta 500 usuarios simultáneos con el 95 % de respuestas por debajo de 3 segundos, agregando solo réplicas. |
| **Verificación** | Aumentar la carga por etapas y comparar tiempos, errores y uso de recursos. |
| **Estado** | Pendiente de confirmar la cantidad esperada de usuarios y la infraestructura de prueba. |

## EQ-03. Seguridad e integridad: calificaciones falsas o repetidas

| Elemento | Descripción |
|---|---|
| **Atributo de calidad** | Seguridad e integridad de datos (DA03) |
| **Fuente** | Usuario sin sesión, con credenciales inválidas o que intenta calificar de forma indebida |
| **Estímulo** | Intenta calificar dos veces al mismo profesional, calificar sin sesión o calificarse a sí mismo. |
| **Artefacto** | Módulo de Calificaciones y motor de reputación |
| **Entorno** | Funcionamiento normal con pruebas sobre las funciones protegidas |
| **Respuesta esperada** | El sistema rechaza la operación, no modifica la reputación y registra el intento para moderación del administrador. |
| **Medida** | **Propuesta por validar:** rechazar el 100 % de los intentos no autorizados o repetidos incluidos en las pruebas, sin exponer datos sensibles. |
| **Verificación** | Pruebas automatizadas (TDD) de acceso autorizado y no autorizado, y revisión de las respuestas de la API. |
| **Estado** | Pendiente de definir las reglas de calificación válida (una por vecino y servicio). |

## EQ-04. Integración externa: falla de un servicio externo

| Elemento | Descripción |
|---|---|
| **Atributo de calidad** | Integración externa y disponibilidad (DA04) |
| **Fuente** | Servicio externo (OpenRouter o WhatsApp) |
| **Estímulo** | El servicio de IA no responde o responde con error al sugerir el rubro. |
| **Artefacto** | Adaptador de IA y módulo de Búsqueda |
| **Entorno** | Funcionamiento normal con el servicio externo caído |
| **Respuesta esperada** | El sistema avisa con un mensaje claro y deja al usuario elegir el rubro manualmente; la búsqueda y el contacto por WhatsApp siguen funcionando. |
| **Medida** | **Propuesta por validar:** el 100 % de las búsquedas manuales sigue operativo y el aviso aparece en 5 segundos o menos. |
| **Verificación** | Simular la caída del servicio con un doble de prueba y comprobar el comportamiento. |
| **Estado** | Pendiente de definir el tiempo máximo de espera del servicio externo. |

## EQ-05. Mantenibilidad: cambiar el proveedor de IA o una regla de reputación

| Elemento | Descripción |
|---|---|
| **Atributo de calidad** | Mantenibilidad (DA06) |
| **Fuente** | Equipo de desarrollo |
| **Estímulo** | Se necesita cambiar el proveedor de IA o modificar la regla de cálculo de reputación. |
| **Artefacto** | Capas Dominio y Aplicación, e interfaces de la capa de Infraestructura |
| **Entorno** | El sistema está organizado en módulos y cuenta con pruebas |
| **Respuesta esperada** | El cambio se concentra en un adaptador o en una regla del dominio, sin modificar casos de uso ni otros módulos. |
| **Medida** | **Propuesta por validar:** el cambio afecta un solo módulo y las pruebas de regresión acordadas se aprueban. |
| **Verificación** | Revisar los archivos modificados, ejecutar las pruebas e inspeccionar las dependencias entre capas. |
| **Estado** | Pendiente de definir las reglas de dependencia y las pruebas obligatorias. |

## Resumen de escenarios

| Código | Atributo de calidad | Driver | Qué se busca comprobar |
|---|---|---|---|
| EQ-01 | Rendimiento | DA02 | Que la búsqueda por rubro y zona responda dentro del tiempo establecido. |
| EQ-02 | Escalabilidad | DA01 | Que el sistema soporte más usuarios agregando réplicas. |
| EQ-03 | Seguridad e integridad | DA03 | Que no se acepten calificaciones falsas o repetidas. |
| EQ-04 | Integración externa | DA04 | Que la falla de un servicio externo no detenga el sistema. |
| EQ-05 | Mantenibilidad | DA06 | Que los cambios no afecten innecesariamente otros módulos. |
