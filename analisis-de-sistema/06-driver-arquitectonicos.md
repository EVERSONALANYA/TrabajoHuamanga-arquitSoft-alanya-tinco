# 06 — Drivers arquitectónicos

Los drivers son los requisitos, atributos de calidad y restricciones que **influyen de manera importante** en cómo se diseña la arquitectura.

> Pregunta guía: *¿Qué requisito o condición puede cambiar la forma en que diseñamos la arquitectura?*

## Evaluación de candidatos

| Fuente | Elemento | ¿Puede ser driver? | Justificación |
|---|---|---|---|
| Requisito funcional | RF07 Calcular promedio de calificación | **Sí** | Requiere un componente de reputación con reglas propias, independiente de la búsqueda. |
| Requisito funcional | RF08 Validar calificaciones fraudulentas | **Sí** | La confianza es el valor central del sistema; condiciona el diseño del dominio y la seguridad. |
| Requisito funcional | RF14 Sugerir rubro con IA | **Sí** | Introduce una dependencia externa que puede fallar y debe aislarse. |
| Requisito funcional | RF02 Editar perfil | No | Es una funcionalidad CRUD que no cambia la estructura general. |
| Requisito funcional | RF16 Rubros más buscados | No | Es una consulta de informe que no cambia la estructura general. |
| Atributo de calidad | AC01 Rendimiento | **Sí** | Requiere caché e índices para las búsquedas. |
| Atributo de calidad | AC02 Escalabilidad | **Sí** | Requiere un backend sin estado que pueda replicarse. |
| Atributo de calidad | AC03 Disponibilidad | **Sí** | Condiciona el manejo de fallos del servicio de IA. |
| Atributo de calidad | AC04 Seguridad | **Sí** | Condiciona autenticación, autorización por rol y protección de datos. |
| Atributo de calidad | AC05 Mantenibilidad | **Sí** | Justifica la separación en capas y módulos. |
| Atributo de calidad | AC08 Usabilidad | No | Afecta el diseño de la interfaz, no la estructura de la arquitectura. |
| Restricción | RC03 API REST | **Sí** | Define el estilo de comunicación frontend–backend. |
| Restricción | RC04 Contacto por WhatsApp | **Sí** | Simplifica la arquitectura: no hay mensajería ni pagos internos. |
| Restricción | RC07 Contenedores Docker | **Sí** | Define la estrategia de despliegue. |
| Restricción | RC15 Tiempo académico | No | Limita el alcance, pero no cambia la estructura del sistema. |

## Drivers arquitectónicos seleccionados

| ID | Driver arquitectónico | Origen | ¿Por qué influye en la arquitectura? |
|---|---|---|---|
| DA01 | El sistema debe responder rápidamente a las búsquedas por rubro y zona. | AC01 – Rendimiento / RC06, RC08 | Influye en el almacenamiento: exige una capa de caché (Redis) y un modelo de datos indexado por rubro y zona. |
| DA02 | El sistema debe soportar el crecimiento de usuarios escalando horizontalmente. | AC02 – Escalabilidad / RC07 | Influye en el despliegue: la API debe ser sin estado para replicarse detrás de un proxy reverso. |
| DA03 | La reputación de los profesionales debe ser confiable y calcularse automáticamente. | RF07, RF08 / AC04 – Seguridad | Justifica un módulo de Reputación con sus propias reglas de validación, probadas con TDD. |
| DA04 | El sistema debe proteger los datos de clientes y profesionales. | AC04 – Seguridad / RC12 | Influye en autenticación, autorización por rol y protección de datos personales. |
| DA05 | El sistema debe integrarse con un modelo de lenguaje externo mediante OpenRouter. | RF14 / RC05, AC03 | Requiere aislar la integración de IA para que un fallo o un cambio de proveedor no afecte la búsqueda. |
| DA06 | El sistema debe utilizar una API REST para la comunicación entre frontend y backend. | RC03 – API REST | Limita las alternativas de comunicación entre las partes del sistema. |
| DA07 | El sistema debe desplegarse en contenedores, sin depender de un proveedor. | RC07, RC09 | Define la arquitectura de despliegue con Docker y la portabilidad del sistema. |
| DA08 | El sistema debe organizarse en módulos independientes. | AC05 – Mantenibilidad / RC13 | Justifica la arquitectura en capas y la separación por módulos de negocio, facilitando las pruebas. |
