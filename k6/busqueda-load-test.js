// ════════════════════════════════════════════════════════
// k6 — Prueba de carga: Búsqueda de profesionales
// Ejecutar: k6 run k6/busqueda-load-test.js
// ════════════════════════════════════════════════════════

import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate, Trend } from 'k6/metrics';

// Métricas personalizadas
const errorRate = new Rate('errors');
const busquedaDuration = new Trend('busqueda_duration');

// Configuración de escenarios
export const options = {
    scenarios: {
        // Escenario 1: Carga gradual (ramp-up)
        carga_gradual: {
            executor: 'ramping-vus',
            startVUs: 0,
            stages: [
                { duration: '30s', target: 50 },   // Subir a 50 usuarios en 30s
                { duration: '1m', target: 100 },    // Subir a 100 usuarios en 1 min
                { duration: '2m', target: 200 },    // Subir a 200 usuarios en 2 min
                { duration: '1m', target: 500 },    // Pico: 500 usuarios concurrentes
                { duration: '30s', target: 0 },     // Bajar a 0
            ],
        },
    },
    thresholds: {
        // Criterios de éxito
        http_req_duration: ['p(95)<300'],   // 95% de requests < 300ms
        http_req_failed: ['rate<0.05'],     // Menos del 5% de errores
        errors: ['rate<0.05'],
        busqueda_duration: ['p(95)<300'],
    },
};

const BASE_URL = __ENV.BASE_URL || 'http://localhost:5000';

// Datos de prueba
const rubros = ['electricista', 'gasfitero', 'albañil', 'pintor', 'carpintero'];
const zonas = ['centro', 'carmen alto', 'san juan'];

export default function () {
    // ─── Escenario: Usuario busca profesionales ───

    // 1. Cargar página principal
    let res = http.get(`${BASE_URL}/`);
    check(res, {
        'página principal carga': (r) => r.status === 200,
        'contiene buscador': (r) => r.body.includes('Buscar'),
    });
    errorRate.add(res.status !== 200);

    sleep(1); // Simular lectura del usuario

    // 2. Buscar por término aleatorio
    const termino = rubros[Math.floor(Math.random() * rubros.length)];
    const startBusqueda = Date.now();

    res = http.get(`${BASE_URL}/?termino=${termino}`);
    busquedaDuration.add(Date.now() - startBusqueda);

    check(res, {
        'búsqueda OK': (r) => r.status === 200,
        'resultados presentes': (r) => r.body.includes('profesional(es) encontrado(s)'),
        'tiempo < 300ms': (r) => r.timings.duration < 300,
    });
    errorRate.add(res.status !== 200);

    sleep(2); // Simular lectura de resultados

    // 3. Buscar vía API REST
    res = http.get(`${BASE_URL.replace(':5000', ':5001')}/api/busqueda?termino=${termino}`);
    check(res, {
        'API búsqueda OK': (r) => r.status === 200,
    });

    sleep(1);
}

export function handleSummary(data) {
    return {
        'k6/resultados/busqueda-summary.json': JSON.stringify(data, null, 2),
        stdout: textSummary(data, { indent: ' ', enableColors: true }),
    };
}

function textSummary(data, opts) {
    const metrics = data.metrics;
    return `
╔══════════════════════════════════════════════════════╗
║       RESULTADOS - Prueba de Carga Búsqueda         ║
╠══════════════════════════════════════════════════════╣
║ Duración total:     ${data.state?.testRunDurationMs || 'N/A'}ms
║ VUs máximos:        ${metrics.vus?.values?.max || 'N/A'}
║ Requests totales:   ${metrics.http_reqs?.values?.count || 'N/A'}
║
║ Tiempo respuesta:
║   p50: ${metrics.http_req_duration?.values?.med?.toFixed(2) || 'N/A'}ms
║   p95: ${metrics.http_req_duration?.values?.['p(95)']?.toFixed(2) || 'N/A'}ms
║   p99: ${metrics.http_req_duration?.values?.['p(99)']?.toFixed(2) || 'N/A'}ms
║
║ Tasa de error:      ${((metrics.http_req_failed?.values?.rate || 0) * 100).toFixed(2)}%
║
║ Búsqueda p95:       ${metrics.busqueda_duration?.values?.['p(95)']?.toFixed(2) || 'N/A'}ms
╚══════════════════════════════════════════════════════╝
`;
}
