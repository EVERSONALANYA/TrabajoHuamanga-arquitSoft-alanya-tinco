# 1. Identificación del Problema y Requerimientos

## 1.1 Problemática

En la ciudad de Huamanga (Ayacucho), los habitantes enfrentan dificultades significativas para encontrar profesionales de servicios confiables (electricistas, gasfiteros, albañiles, pintores, etc.). Actualmente:

- **No existe un directorio centralizado** de profesionales de servicios locales
- Los habitantes dependen del **boca a boca**, que es limitado y poco confiable
- No hay un **sistema de reputación transparente** que permita evaluar la calidad del servicio
- Los profesionales independientes **no tienen visibilidad digital** para ofrecer sus servicios
- El contacto es difícil: números perdidos, intermediarios, información desactualizada

### Datos del contexto
- Huamanga tiene aproximadamente 200,000 habitantes distribuidos en distritos como Andrés Avelino Cáceres, Carmen Alto, San Juan Bautista, Jesús Nazareno y el Centro Histórico
- La mayoría de profesionales trabajan de forma independiente sin presencia digital
- WhatsApp es la herramienta de comunicación más usada en la zona

## 1.2 Objetivo General

Desarrollar una plataforma web que funcione como directorio de profesionales de servicios en Huamanga, permitiendo a los usuarios buscar, evaluar y contactar profesionales de manera rápida y confiable.

## 1.3 Objetivos Específicos

1. Implementar un sistema de búsqueda por rubro y zona con caché Redis (<300ms)
2. Desarrollar un sistema de calificaciones con moderación administrativa
3. Integrar contacto directo vía WhatsApp (wa.me links)
4. Implementar sugerencia de rubro mediante IA (OpenRouter → Gemini Flash)
5. Garantizar escalabilidad para 500 usuarios concurrentes

## 1.4 Requerimientos Funcionales

| ID | Requerimiento | Prioridad | Actor |
|----|--------------|-----------|-------|
| RF-01 | Buscar profesionales por rubro, zona y término | Alta | Usuario |
| RF-02 | Ver perfil detallado de un profesional | Alta | Usuario |
| RF-03 | Calificar a un profesional (1-5 estrellas + comentario) | Alta | Usuario |
| RF-04 | Contactar profesional vía WhatsApp | Alta | Usuario |
| RF-05 | Sugerencia de rubro por IA | Media | Usuario |
| RF-06 | Registrarse como profesional | Alta | Profesional |
| RF-07 | Gestionar perfil propio | Media | Profesional |
| RF-08 | Reportar calificaciones inapropiadas | Media | Usuario |
| RF-09 | Moderar calificaciones reportadas | Alta | Administrador |
| RF-10 | Ver estadísticas del sistema | Baja | Administrador |

## 1.5 Requerimientos No Funcionales

| ID | Requerimiento | Métrica |
|----|--------------|---------|
| RNF-01 | Rendimiento | Tiempo de respuesta < 300ms (con caché < 5ms) |
| RNF-02 | Escalabilidad | Soportar 500 usuarios concurrentes |
| RNF-03 | Disponibilidad | 99.5% uptime |
| RNF-04 | Mantenibilidad | Agregar nuevo módulo en < 8 horas |
| RNF-05 | Seguridad | Validación de datos en todas las entradas |
| RNF-06 | Usabilidad | Interfaz responsive, compatible con móviles |

## 1.6 Actores del Sistema

```
┌──────────┐     ┌──────────────┐     ┌───────────────┐
│ Usuario  │     │ Profesional  │     │ Administrador │
│          │     │              │     │               │
│ • Buscar │     │ • Registrar  │     │ • Moderar     │
│ • Califi │     │ • Perfil     │     │ • Gestionar   │
│ • Contac │     │ • Ver calif. │     │ • Estadístic. │
└──────────┘     └──────────────┘     └───────────────┘
```

## 1.7 Casos de Uso Principales

1. **CU-01**: Usuario busca profesional por rubro y zona
2. **CU-02**: Usuario califica a un profesional
3. **CU-03**: Usuario contacta profesional por WhatsApp
4. **CU-04**: Usuario recibe sugerencia de rubro por IA
5. **CU-05**: Profesional se registra en la plataforma
6. **CU-06**: Administrador modera calificaciones reportadas
