// ════════════════════════════════════════════════════════
// k6 — Prueba de estrés: API REST endpoints
// Ejecutar: k6 run k6/api-stress-test.js
// ════════════════════════════════════════════════════════

import http from 'k6/http';
import { check, sleep, group } from 'k6';
import { Rate } from 'k6/metrics';

const errorRate = new Rate('errors');

export const options = {
    scenarios: {
        // Estrés progresivo hasta encontrar el punto de quiebre
        stress: {
            executor: 'ramping-vus',
            startVUs: 0,
            stages: [
                { duration: '1m', target: 100 },    // Normal
                { duration: '1m', target: 300 },    // Carga alta
                { duration: '1m', target: 500 },    // Estrés
                { duration: '30s', target: 0 },     // Recuperación
            ],
        },
    },
    thresholds: {
        http_req_duration: ['p(95)<500'],       // Más permisivo en estrés
        http_req_failed: ['rate<0.10'],         // Hasta 10% errores aceptable
        errors: ['rate<0.10'],
    },
};

const API_URL = __ENV.API_URL || 'http://localhost:5001';

export default function () {
    group('Catálogos', () => {
        // GET rubros
        let res = http.get(`${API_URL}/api/catalogos/rubros`);
        check(res, { 'rubros OK': (r) => r.status === 200 });
        errorRate.add(res.status !== 200);

        // GET zonas
        res = http.get(`${API_URL}/api/catalogos/zonas`);
        check(res, { 'zonas OK': (r) => r.status === 200 });
        errorRate.add(res.status !== 200);
    });

    group('Búsqueda', () => {
        const res = http.get(`${API_URL}/api/busqueda?termino=electricista`);
        check(res, {
            'búsqueda API OK': (r) => r.status === 200,
            'respuesta es JSON': (r) => r.headers['Content-Type']?.includes('json'),
        });
        errorRate.add(res.status !== 200);
    });

    group('Profesionales', () => {
        const res = http.get(`${API_URL}/api/profesionales`);
        check(res, { 'profesionales OK': (r) => r.status === 200 });
        errorRate.add(res.status !== 200);
    });

    group('Moderación', () => {
        const res = http.get(`${API_URL}/api/moderacion/reportadas`);
        check(res, { 'moderación OK': (r) => r.status === 200 });
        errorRate.add(res.status !== 200);
    });

    sleep(1);
}
