import http from 'k6/http';
import { check, sleep } from 'k6';

// Configuração do teste de estresse
export const options = {
  stages: [
    { duration: '30s', target: 10 },   // Ramp up para 10 usuários em 30s
    { duration: '1m', target: 50 },    // Ramp up para 50 usuários em 1m
    { duration: '2m', target: 100 },   // Ramp up para 100 usuários em 2m
    { duration: '2m', target: 100 },   // Mantém 100 usuários por 2m
    { duration: '1m', target: 50 },    // Ramp down para 50 usuários em 1m
    { duration: '30s', target: 0 },    // Ramp down para 0 usuários em 30s
  ],
  thresholds: {
    http_req_duration: ['p(95)<500'], // 95% das requisições devem ser < 500ms
    http_req_failed: ['rate<0.01'],   // Taxa de erro < 1%
  },
};

export default function () {
  const response = http.get('http://localhost:5272/products/optimized', {
    headers: {
      'Accept': 'application/json',
    },
  });

  check(response, {
    'status is 200': (r) => r.status === 200,
    'response time < 500ms': (r) => r.timings.duration < 500,
    'response body is not empty': (r) => r.body.length > 0,
  });

  sleep(1);
}

export function handleSummary(data) {
  return {
    'optimized-endpoint-results.html': htmlReport(data),
    'optimized-endpoint-results.json': JSON.stringify(data),
  };
}

function htmlReport(data) {
  return `
<!DOCTYPE html>
<html>
<head>
    <title>Optimized Endpoint - Load Test Results</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        .metric { margin: 10px 0; padding: 10px; background: #f5f5f5; border-radius: 5px; }
        .pass { background: #d4edda; }
        .fail { background: #f8d7da; }
    </style>
</head>
<body>
    <h1>Optimized Endpoint Load Test Results</h1>
    <div class="metric">
        <h3>Request Rate</h3>
        <p>Total Requests: ${data.metrics.http_reqs.values.count}</p>
        <p>Requests/sec: ${data.metrics.http_reqs.values.rate.toFixed(2)}</p>
    </div>
    <div class="metric">
        <h3>Response Times</h3>
        <p>Average: ${data.metrics.http_req_duration.values.avg.toFixed(2)}ms</p>
        <p>95th percentile: ${data.metrics.http_req_duration.values['p(95)'].toFixed(2)}ms</p>
        <p>Max: ${data.metrics.http_req_duration.values.max.toFixed(2)}ms</p>
    </div>
    <div class="metric ${data.metrics.http_req_failed.values.rate < 0.01 ? 'pass' : 'fail'}">
        <h3>Error Rate</h3>
        <p>Failed Requests: ${(data.metrics.http_req_failed.values.rate * 100).toFixed(2)}%</p>
    </div>
</body>
</html>`;
}
