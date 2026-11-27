# 05 — Restricciones

Condiciones, reglas o limitaciones que deben respetarse durante el desarrollo. Pueden ser tecnológicas, organizacionales, legales o del proyecto.

## Restricciones tecnológicas

| ID | Restricción | Tipo | Descripción |
|---|---|---|---|
| RC01 | Aplicación web | Tecnológica | El sistema debe desarrollarse íntegramente como una aplicación web accesible desde cualquier navegador. |
| RC02 | Control de versiones | Proyecto | El código fuente debe gestionarse con Git y mantenerse en un repositorio en GitHub. |
| RC03 | API REST | Tecnológica | La comunicación entre el frontend y el backend debe realizarse mediante una API REST. |
| RC04 | Contacto por WhatsApp | Tecnológica | El contacto entre cliente y profesional se deriva a WhatsApp mediante un enlace directo; no se desarrolla mensajería interna. |
| RC05 | IA mediante OpenRouter | Tecnológica | La sugerencia de rubro debe usar un modelo de lenguaje accedido mediante OpenRouter, sin alojar el modelo en la infraestructura propia. |
| RC06 | Caché con Redis | Tecnológica | Las búsquedas más frecuentes por rubro y zona deben almacenarse en una capa de caché Redis. |
| RC07 | Contenedores Docker | Tecnológica | El sistema debe desplegarse mediante contenedores Docker, reproduciendo el mismo entorno en desarrollo y producción. |
| RC08 | Base de datos | Tecnológica | Se emplea SQL Server como base de datos relacional, con índices por rubro y zona. |

## Restricciones adicionales identificadas

| ID | Restricción | Tipo | Descripción |
|---|---|---|---|
| RC09 | Independencia del proveedor | Organizacional | El sistema no debe depender de un proveedor de infraestructura específico (nube o servidor propio). |
| RC10 | Sin pagos en la plataforma | Organizacional | El sistema no gestiona pagos ni negociación del servicio; estos quedan a cargo de las partes. |
| RC11 | Sin verificación de antecedentes | Organizacional | No se verifican antecedentes legales ni certificaciones oficiales; la confianza se basa en la reputación entre clientes. |
| RC12 | Protección de datos personales | Legal | El tratamiento de datos de clientes y profesionales debe cumplir la Ley N.° 29733, Ley de Protección de Datos Personales del Perú. |
| RC13 | Desarrollo guiado por pruebas | Proyecto | El desarrollo debe seguir TDD, con pruebas unitarias, de integración y de carga. |
| RC14 | Documentación con modelo C4 | Proyecto | La arquitectura se documenta con el modelo C4, en Markdown y con diagramas en Mermaid. |
| RC15 | Tiempo académico | Proyecto | El proyecto debe desarrollarse dentro del semestre 2026-II; la aplicación móvil nativa queda para una fase 2. |
