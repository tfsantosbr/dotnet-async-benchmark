import http from 'k6/http';
import { check, sleep } from 'k6';
import { Counter, Rate, Trend } from 'k6/metrics';

// Métricas customizadas para comparação
const standardRequests = new Counter('standard_requests');
const optimizedRequests = new Counter('optimized_requests');
const standardDuration = new Trend('standard_duration');
const optimizedDuration = new Trend('optimized_duration');
const standardErrorRate = new Rate('standard_errors');
const optimizedErrorRate = new Rate('optimized_errors');

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
    'standard_duration': ['p(95)<500'],
    'optimized_duration': ['p(95)<500'],
    'standard_errors': ['rate<0.01'],
    'optimized_errors': ['rate<0.01'],
  },
};

export default function () {
  // Alterna entre os dois endpoints para comparação direta
  const useStandard = Math.random() < 0.5;
  
  if (useStandard) {
    testStandardEndpoint();
  } else {
    testOptimizedEndpoint();
  }

  sleep(1);
}

function testStandardEndpoint() {
  const response = http.get('http://localhost:5272/products/standard', {
    headers: {
      'Accept': 'application/json',
    },
  });

  standardRequests.add(1);
  standardDuration.add(response.timings.duration);
  standardErrorRate.add(response.status !== 200);

  check(response, {
    'standard - status is 200': (r) => r.status === 200,
    'standard - response time < 500ms': (r) => r.timings.duration < 500,
    'standard - response body is not empty': (r) => r.body.length > 0,
  }, { endpoint: 'standard' });
}

function testOptimizedEndpoint() {
  const response = http.get('http://localhost:5272/products/optimized', {
    headers: {
      'Accept': 'application/json',
    },
  });

  optimizedRequests.add(1);
  optimizedDuration.add(response.timings.duration);
  optimizedErrorRate.add(response.status !== 200);

  check(response, {
    'optimized - status is 200': (r) => r.status === 200,
    'optimized - response time < 500ms': (r) => r.timings.duration < 500,
    'optimized - response body is not empty': (r) => r.body.length > 0,
  }, { endpoint: 'optimized' });
}

export function handleSummary(data) {
  return {
    'comparison-results.html': htmlComparisonReport(data),
    'comparison-results.json': JSON.stringify(data),
  };
}

function htmlComparisonReport(data) {
  const standardData = {
    requests: data.metrics.standard_requests?.values?.count || 0,
    avgDuration: data.metrics.standard_duration?.values?.avg || 0,
    p95Duration: data.metrics.standard_duration?.values?.['p(95)'] || 0,
    maxDuration: data.metrics.standard_duration?.values?.max || 0,
    errorRate: data.metrics.standard_errors?.values?.rate || 0,
  };

  const optimizedData = {
    requests: data.metrics.optimized_requests?.values?.count || 0,
    avgDuration: data.metrics.optimized_duration?.values?.avg || 0,
    p95Duration: data.metrics.optimized_duration?.values?.['p(95)'] || 0,
    maxDuration: data.metrics.optimized_duration?.values?.max || 0,
    errorRate: data.metrics.optimized_errors?.values?.rate || 0,
  };

  const improvement = {
    avgDuration: ((standardData.avgDuration - optimizedData.avgDuration) / standardData.avgDuration * 100).toFixed(2),
    p95Duration: ((standardData.p95Duration - optimizedData.p95Duration) / standardData.p95Duration * 100).toFixed(2),
  };

  return `
<!DOCTYPE html>
<html>
<head>
    <title>Endpoint Performance Comparison</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        .comparison-table { width: 100%; border-collapse: collapse; margin: 20px 0; }
        .comparison-table th, .comparison-table td { 
            border: 1px solid #ddd; 
            padding: 12px; 
            text-align: left; 
        }
        .comparison-table th { background-color: #f2f2f2; }
        .better { background-color: #d4edda; color: #155724; font-weight: bold; }
        .worse { background-color: #f8d7da; color: #721c24; }
        .metric { margin: 10px 0; padding: 10px; background: #f5f5f5; border-radius: 5px; }
        .summary { background: #e7f3ff; border-left: 4px solid #2196F3; padding: 15px; margin: 20px 0; }
    </style>
</head>
<body>
    <h1>Endpoint Performance Comparison Results</h1>
    
    <div class="summary">
        <h3>📊 Performance Summary</h3>
        <p><strong>Average Response Time Improvement:</strong> ${improvement.avgDuration}%</p>
        <p><strong>95th Percentile Response Time Improvement:</strong> ${improvement.p95Duration}%</p>
        <p><strong>Winner:</strong> ${optimizedData.avgDuration < standardData.avgDuration ? 'Optimized Endpoint' : 'Standard Endpoint'}</p>
    </div>

    <table class="comparison-table">
        <thead>
            <tr>
                <th>Metric</th>
                <th>Standard Endpoint</th>
                <th>Optimized Endpoint</th>
                <th>Difference</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td><strong>Total Requests</strong></td>
                <td>${standardData.requests}</td>
                <td>${optimizedData.requests}</td>
                <td>-</td>
            </tr>
            <tr>
                <td><strong>Average Response Time</strong></td>
                <td ${optimizedData.avgDuration < standardData.avgDuration ? 'class="worse"' : 'class="better"'}>
                    ${standardData.avgDuration.toFixed(2)}ms
                </td>
                <td ${optimizedData.avgDuration < standardData.avgDuration ? 'class="better"' : 'class="worse"'}>
                    ${optimizedData.avgDuration.toFixed(2)}ms
                </td>
                <td>${improvement.avgDuration}%</td>
            </tr>
            <tr>
                <td><strong>95th Percentile</strong></td>
                <td ${optimizedData.p95Duration < standardData.p95Duration ? 'class="worse"' : 'class="better"'}>
                    ${standardData.p95Duration.toFixed(2)}ms
                </td>
                <td ${optimizedData.p95Duration < standardData.p95Duration ? 'class="better"' : 'class="worse"'}>
                    ${optimizedData.p95Duration.toFixed(2)}ms
                </td>
                <td>${improvement.p95Duration}%</td>
            </tr>
            <tr>
                <td><strong>Max Response Time</strong></td>
                <td ${optimizedData.maxDuration < standardData.maxDuration ? 'class="worse"' : 'class="better"'}>
                    ${standardData.maxDuration.toFixed(2)}ms
                </td>
                <td ${optimizedData.maxDuration < standardData.maxDuration ? 'class="better"' : 'class="worse"'}>
                    ${optimizedData.maxDuration.toFixed(2)}ms
                </td>
                <td>${((standardData.maxDuration - optimizedData.maxDuration) / standardData.maxDuration * 100).toFixed(2)}%</td>
            </tr>
            <tr>
                <td><strong>Error Rate</strong></td>
                <td ${optimizedData.errorRate < standardData.errorRate ? 'class="worse"' : 'class="better"'}>
                    ${(standardData.errorRate * 100).toFixed(2)}%
                </td>
                <td ${optimizedData.errorRate < standardData.errorRate ? 'class="better"' : 'class="worse"'}>
                    ${(optimizedData.errorRate * 100).toFixed(2)}%
                </td>
                <td>${((standardData.errorRate - optimizedData.errorRate) * 100).toFixed(2)}%</td>
            </tr>
        </tbody>
    </table>

    <div class="metric">
        <h3>🔍 Analysis</h3>
        <p><strong>Performance Difference:</strong></p>
        <ul>
            <li>The optimized endpoint shows ${improvement.avgDuration > 0 ? 'better' : 'worse'} average performance</li>
            <li>95th percentile response time is ${improvement.p95Duration > 0 ? 'improved' : 'degraded'} by ${Math.abs(improvement.p95Duration)}%</li>
            <li>Overall recommendation: Use the <strong>${optimizedData.avgDuration < standardData.avgDuration ? 'Optimized' : 'Standard'}</strong> endpoint for better performance</li>
        </ul>
    </div>
</body>
</html>`;
}
