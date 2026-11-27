# 03 — Requisitos funcionales

> **Historia de usuario:** expresa la necesidad desde el punto de vista del usuario.
> **Requisito funcional:** expresa lo que el sistema debe hacer para satisfacer esa necesidad.

## Requisitos funcionales

| ID | Requisito funcional | Módulo |
|---|---|---|
| RF01 | El sistema debe permitir a un profesional registrar su perfil indicando nombre, rubro, zona de atención y datos de contacto. | Profesionales |
| RF02 | El sistema debe permitir a un profesional editar su perfil y darse de baja del directorio. | Profesionales |
| RF03 | El sistema debe permitir buscar profesionales filtrando por rubro, zona/distrito o nombre. | Búsqueda |
| RF04 | El sistema debe mostrar en los resultados el promedio de calificación y el número de reseñas de cada profesional. | Búsqueda |
| RF05 | El sistema debe indicar de forma visible cuando un profesional aún no cuenta con calificaciones. | Búsqueda |
| RF06 | El sistema debe permitir a un cliente calificar a un profesional con una puntuación de 1 a 5 estrellas y un comentario. | Reputación |
| RF07 | El sistema debe calcular automáticamente el promedio de calificación de cada profesional ante cada nueva reseña. | Reputación |
| RF08 | El sistema debe aplicar reglas de validación para reducir calificaciones fraudulentas o repetidas por la misma fuente. | Reputación |
| RF09 | El sistema debe generar un enlace de contacto directo por WhatsApp hacia el número registrado del profesional. | Contacto |
| RF10 | El sistema debe permitir registrar usuarios e iniciar sesión con credenciales. | Usuarios |
| RF11 | El sistema debe asignar permisos según el rol del usuario (cliente, profesional, administrador). | Usuarios |
| RF12 | El sistema debe permitir a un cliente reportar un perfil o una calificación como falsa o inapropiada. | Moderación |
| RF13 | El sistema debe permitir al administrador revisar y dar de baja perfiles o calificaciones reportadas. | Moderación |
| RF14 | El sistema debe sugerir el rubro más adecuado a partir de una descripción libre de la necesidad del cliente, usando un modelo de lenguaje vía OpenRouter. | Sugerencia IA |
| RF15 | El sistema debe tratar la sugerencia de la IA como una recomendación editable, que requiere confirmación del usuario. | Sugerencia IA |
| RF16 | El sistema debe generar un reporte de los rubros más buscados en un periodo determinado. | Informes |
| RF17 | El sistema debe identificar las zonas de Huamanga con pocos profesionales registrados en un rubro determinado. | Informes |

## Relación entre historias de usuario y requisitos funcionales

| Historia de usuario | Requisitos funcionales relacionados |
|---|---|
| HU01 Registrar perfil | RF01 |
| HU02 Buscar profesionales | RF03 |
| HU03 Ver reputación | RF04, RF07 |
| HU04 Calificar profesional | RF06, RF07 |
| HU05 Contactar por WhatsApp | RF09 |
| HU06 Moderar reportes | RF13, RF07 |
| HU07 Registrarse e iniciar sesión | RF10, RF11 |
| HU08 Editar perfil o darse de baja | RF02 |
| HU09 Ver profesionales sin calificaciones | RF05 |
| HU10 Evitar calificaciones fraudulentas | RF08 |
| HU11 Reportar perfil o reseña | RF12 |
| HU12 Recibir sugerencia de rubro | RF14 |
| HU13 Confirmar sugerencia | RF15 |
| HU14 Rubros más buscados | RF16 |
| HU15 Zonas con poca oferta | RF17 |
