import http from 'k6/http';
import { check, sleep, group } from 'k6';
import { Rate, Trend } from 'k6/metrics';

// ============================================================
// Trabajo Huamanga - Pruebas de Carga con k6
// Proyecto: Directorio de profesionales de servicios en Huamanga
// Curso: IS-488 Arquitectura de Software - UNSCH
// Alumno: Everson Alanya Tinco
// ============================================================

const BASE_URL = __ENV.BASE_URL || 'https://trabajohuamanga-arquitsoft-alanya-tinco-production.up.railway.app';

// Metricas personalizadas
const errorRate = new Rate('errors');
const healthCheckDuration = new Trend('health_check_duration');
const busquedaDuration = new Trend('busqueda_duration');
const calificacionDuration = new Trend('calificacion_duration');

// Escenarios de carga progresiva hasta 2,000 usuarios virtuales concurrentes
export const options = {
  stages: [
    // Fase 1: Calentamiento (warm-up)
    { duration: '30s', target: 50 },      // 0 → 50 usuarios en 30s

    // Fase 2: Carga baja
    { duration: '1m', target: 200 },       // 50 → 200 usuarios en 1m

    // Fase 3: Carga media
    { duration: '1m', target: 500 },       // 200 → 500 usuarios en 1m

    // Fase 4: Carga alta
    { duration: '1m', target: 1000 },      // 500 → 1,000 usuarios en 1m

    // Fase 5: Carga maxima (pico)
    { duration: '1m', target: 2000 },      // 1,000 → 2,000 usuarios en 1m

    // Fase 6: Sostenimiento del pico
    { duration: '1m', target: 2000 },      // Mantener 2,000 usuarios por 1m

    // Fase 7: Descarga gradual (cool-down)
    { duration: '30s', target: 500 },      // 2,000 → 500 usuarios en 30s
    { duration: '30s', target: 0 },        // 500 → 0 usuarios en 30s
  ],

  thresholds: {
    // Umbrales de rendimiento esperados
    http_req_duration: ['p(95)<3000'],  // 95% de requests < 3s
    http_req_failed: ['rate<0.15'],     // Menos de 15% de fallos
    errors: ['rate<0.15'],              // Tasa de errores < 15%
  },
};

// Datos de prueba
const RUBROS = ['Electricista', 'Gasfitero', 'Pintor', 'Carpintero', 'Tutor de Matematicas'];
const DISTRITOS = ['Ayacucho', 'San Juan Bautista', 'Carmen Alto', 'Jesus Nazareno', 'Andres Avelino Caceres'];

function randomItem(arr) {
  return arr[Math.floor(Math.random() * arr.length)];
}

// ============================================================
// ESCENARIO PRINCIPAL
// ============================================================
export default function () {

  // --- 1. Health Check ---
  group('Health Check', () => {
    const res = http.get(`${BASE_URL}/health`);
    healthCheckDuration.add(res.timings.duration);
    const ok = check(res, {
      'health: status 200': (r) => r.status === 200,
      'health: returns healthy': (r) => {
        try {
          return JSON.parse(r.body).status === 'healthy';
        } catch (e) { return false; }
      },
    });
    errorRate.add(!ok);
  });

  sleep(0.5);

  // --- 2. Listar Rubros ---
  group('Listar Rubros', () => {
    const res = http.get(`${BASE_URL}/api/rubros`);
    const ok = check(res, {
      'rubros: status 200': (r) => r.status === 200,
      'rubros: returns array': (r) => {
        try {
          return Array.isArray(JSON.parse(r.body));
        } catch (e) { return false; }
      },
    });
    errorRate.add(!ok);
  });

  // --- 3. Listar Zonas ---
  group('Listar Zonas', () => {
    const res = http.get(`${BASE_URL}/api/zonas`);
    const ok = check(res, {
      'zonas: status 200': (r) => r.status === 200,
      'zonas: returns array': (r) => {
        try {
          return Array.isArray(JSON.parse(r.body));
        } catch (e) { return false; }
      },
    });
    errorRate.add(!ok);
  });

  sleep(0.5);

  // --- 4. Listar Todos los Profesionales ---
  group('Listar Profesionales', () => {
    const res = http.get(`${BASE_URL}/api/profesionales`);
    const ok = check(res, {
      'profesionales: status 200': (r) => r.status === 200,
      'profesionales: returns array': (r) => {
        try {
          return Array.isArray(JSON.parse(r.body));
        } catch (e) { return false; }
      },
    });
    errorRate.add(!ok);
  });

  sleep(0.3);

  // --- 5. Busqueda por Rubro ---
  group('Buscar por Rubro', () => {
    const rubro = randomItem(RUBROS);
    const res = http.get(`${BASE_URL}/api/profesionales/buscar?rubro=${encodeURIComponent(rubro)}`);
    busquedaDuration.add(res.timings.duration);
    const ok = check(res, {
      'busqueda rubro: status 200': (r) => r.status === 200,
      'busqueda rubro: returns array': (r) => {
        try {
          return Array.isArray(JSON.parse(r.body));
        } catch (e) { return false; }
      },
    });
    errorRate.add(!ok);
  });

  // --- 6. Busqueda por Distrito ---
  group('Buscar por Distrito', () => {
    const distrito = randomItem(DISTRITOS);
    const res = http.get(`${BASE_URL}/api/profesionales/buscar?distrito=${encodeURIComponent(distrito)}`);
    busquedaDuration.add(res.timings.duration);
    const ok = check(res, {
      'busqueda distrito: status 200': (r) => r.status === 200,
      'busqueda distrito: returns array': (r) => {
        try {
          return Array.isArray(JSON.parse(r.body));
        } catch (e) { return false; }
      },
    });
    errorRate.add(!ok);
  });

  sleep(0.3);

  // --- 7. Busqueda combinada (Rubro + Distrito) ---
  group('Buscar Rubro + Distrito', () => {
    const rubro = randomItem(RUBROS);
    const distrito = randomItem(DISTRITOS);
    const res = http.get(`${BASE_URL}/api/profesionales/buscar?rubro=${encodeURIComponent(rubro)}&distrito=${encodeURIComponent(distrito)}`);
    busquedaDuration.add(res.timings.duration);
    const ok = check(res, {
      'busqueda combinada: status 200': (r) => r.status === 200,
    });
    errorRate.add(!ok);
  });

  // --- 8. Registrar Profesional (escritura) ---
  group('Registrar Profesional', () => {
    const payload = JSON.stringify({
      nombre: `Test User ${Date.now()}`,
      rubro: randomItem(RUBROS),
      distrito: randomItem(DISTRITOS),
    });
    const params = { headers: { 'Content-Type': 'application/json' } };
    const res = http.post(`${BASE_URL}/api/profesionales`, payload, params);
    const ok = check(res, {
      'registrar: status 201': (r) => r.status === 201,
      'registrar: returns id': (r) => {
        try {
          return JSON.parse(r.body).id !== undefined;
        } catch (e) { return false; }
      },
    });
    errorRate.add(!ok);
  });

  sleep(0.3);

  // --- 9. Registrar Calificacion ---
  group('Registrar Calificacion', () => {
    const payload = JSON.stringify({
      vecinoId: crypto.randomUUID ? crypto.randomUUID() : '00000000-0000-0000-0000-' + Date.now().toString().slice(-12),
      profesionalId: crypto.randomUUID ? crypto.randomUUID() : '11111111-1111-1111-1111-' + Date.now().toString().slice(-12),
      puntaje: Math.floor(Math.random() * 5) + 1,
      comentario: 'Buen servicio - prueba de carga k6',
    });
    const params = { headers: { 'Content-Type': 'application/json' } };
    const res = http.post(`${BASE_URL}/api/calificaciones`, payload, params);
    calificacionDuration.add(res.timings.duration);
    const ok = check(res, {
      'calificacion: status 201': (r) => r.status === 201,
    });
    errorRate.add(!ok);
  });

  sleep(0.3);

  // --- 10. Validacion de Puntaje Invalido ---
  group('Validar Puntaje Invalido', () => {
    const payload = JSON.stringify({
      vecinoId: '00000000-0000-0000-0000-000000000001',
      profesionalId: '11111111-1111-1111-1111-111111111111',
      puntaje: 10, // fuera de rango 1-5
      comentario: 'Test puntaje invalido',
    });
    const params = { headers: { 'Content-Type': 'application/json' } };
    const res = http.post(`${BASE_URL}/api/calificaciones`, payload, params);
    const ok = check(res, {
      'puntaje invalido: status 400': (r) => r.status === 400,
      'puntaje invalido: error message': (r) => {
        try {
          return JSON.parse(r.body).error.includes('puntaje');
        } catch (e) { return false; }
      },
    });
    errorRate.add(!ok);
  });

  sleep(0.5);
}

// ============================================================
// Resumen personalizado al final de la prueba
// ============================================================
export function handleSummary(data) {
  const summary = {
    'Resultado de Pruebas de Carga - Trabajo Huamanga': {
      'URL Base': BASE_URL,
      'VUs Maximo': 2000,
      'Duracion Total': '7 minutos',
      'Total Requests': data.metrics.http_reqs ? data.metrics.http_reqs.values.count : 'N/A',
      'Requests/s Promedio': data.metrics.http_reqs ? data.metrics.http_reqs.values.rate.toFixed(2) : 'N/A',
      'Tiempo Respuesta P95 (ms)': data.metrics.http_req_duration ? data.metrics.http_req_duration.values['p(95)'].toFixed(2) : 'N/A',
      'Tiempo Respuesta P99 (ms)': data.metrics.http_req_duration ? data.metrics.http_req_duration.values['p(99)'].toFixed(2) : 'N/A',
      'Tasa de Error (%)': data.metrics.http_req_failed ? (data.metrics.http_req_failed.values.rate * 100).toFixed(2) : 'N/A',
    },
  };

  console.log('\n' + '='.repeat(60));
  console.log('  RESUMEN - PRUEBA DE CARGA TRABAJO HUAMANGA');
  console.log('='.repeat(60));
  for (const [key, val] of Object.entries(summary['Resultado de Pruebas de Carga - Trabajo Huamanga'])) {
    console.log(`  ${key}: ${val}`);
  }
  console.log('='.repeat(60) + '\n');

  return {
    'stdout': textSummary(data, { indent: '  ', enableColors: true }),
    'results/load-test-summary.json': JSON.stringify(data, null, 2),
  };
}

// k6 built-in textSummary
import { textSummary } from 'https://jslib.k6.io/k6-summary/0.0.2/index.js';
