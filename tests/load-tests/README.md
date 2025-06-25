# Testes de Performance com K6

Este diretório contém testes de estresse usando K6 para comparar a performance entre os endpoints standard e optimized da API.

## Pré-requisitos

1. **Instalar K6**: 
   ```bash
   # Windows (via Chocolatey)
   choco install k6
   
   # Windows (via Scoop)
   scoop install k6
   
   # Ou baixar diretamente do site: https://k6.io/docs/get-started/installation/
   ```

2. **API em execução**: 
   ```bash
   # Na pasta src/AsyncTest.Api
   dotnet run
   ```
   A API deve estar rodando em `http://localhost:5272`

## Estrutura dos Testes

### 1. `standard-endpoint-test.js`
Teste focado exclusivamente no endpoint `/products/standard`
- **Objetivo**: Medir performance do endpoint padrão
- **Perfil de carga**: Ramp up gradual até 100 usuários concorrentes
- **Duração**: ~6.5 minutos
- **Saída**: `standard-endpoint-results.html` e `standard-endpoint-results.json`

### 2. `optimized-endpoint-test.js`
Teste focado exclusivamente no endpoint `/products/optimized`
- **Objetivo**: Medir performance do endpoint otimizado
- **Perfil de carga**: Idêntico ao teste standard para comparação justa
- **Duração**: ~6.5 minutos
- **Saída**: `optimized-endpoint-results.html` e `optimized-endpoint-results.json`

### 3. `comparison-test.js`
Teste comparativo que alterna entre ambos os endpoints
- **Objetivo**: Comparação direta lado a lado
- **Perfil de carga**: Distribuição 50/50 entre os endpoints
- **Duração**: ~6.5 minutos
- **Saída**: `comparison-results.html` com análise comparativa detalhada

## Como Executar os Testes

### Teste Individual - Standard Endpoint
```bash
k6 run standard-endpoint-test.js
```

### Teste Individual - Optimized Endpoint
```bash
k6 run optimized-endpoint-test.js
```

### Teste Comparativo
```bash
k6 run comparison-test.js
```

### Executar Todos os Testes Sequencialmente
```bash
# Windows PowerShell
k6 run standard-endpoint-test.js; k6 run optimized-endpoint-test.js; k6 run comparison-test.js

# Windows CMD
k6 run standard-endpoint-test.js && k6 run optimized-endpoint-test.js && k6 run comparison-test.js
```

## Configuração do Teste

### Perfil de Carga
- **Ramp up**: 30s → 10 usuários → 1m → 50 usuários → 2m → 100 usuários
- **Sustentação**: 2 minutos com 100 usuários concorrentes
- **Ramp down**: 1m → 50 usuários → 30s → 0 usuários

### Thresholds (Critérios de Sucesso)
- **Response Time**: 95% das requisições < 500ms
- **Error Rate**: Taxa de erro < 1%

### Métricas Coletadas
- **Request Rate**: Requisições por segundo
- **Response Times**: Média, 95º percentil, máximo
- **Error Rate**: Porcentagem de requisições falhadas
- **Success Rate**: Porcentagem de requisições bem-sucedidas

## Interpretação dos Resultados

### Arquivos HTML
Os relatórios HTML fornecem visualização clara dos resultados:
- **Métricas principais**: Taxa de requisições, tempos de resposta, taxa de erro
- **Indicadores visuais**: Verde para sucesso, vermelho para falhas
- **Comparação**: O teste comparativo mostra diferenças percentuais

### Métricas JSON
Os arquivos JSON contêm dados brutos para análise programática ou integração com outras ferramentas.

## Análise Esperada

Com base no código analisado, a diferença principal entre os endpoints é:

**Standard Endpoint**: Usa `async/await` em toda a cadeia de chamadas
**Optimized Endpoint**: Retorna `Task` diretamente sem `async/await` desnecessário

A otimização pode resultar em:
- Menor overhead de alocação de state machines
- Redução na pressão do garbage collector
- Melhor throughput em cenários de alta concorrência

## Troubleshooting

### API não responde
```bash
# Verificar se a API está rodando
curl http://localhost:5272/products/standard
```

### K6 não encontrado
Verifique se o K6 está instalado e no PATH do sistema.

### Resultados inconsistentes
- Execute os testes múltiplas vezes
- Verifique se não há outros processos consumindo recursos
- Considere fazer warm-up da aplicação antes dos testes
