#!/bin/bash

echo "🚀 Executando Testes de Performance K6 - AsyncTest API"
echo "=================================================="

# Verificar se K6 está instalado
if ! command -v k6 &> /dev/null; then
    echo "❌ K6 não está instalado. Por favor, instale o K6 primeiro."
    echo "Instruções: https://k6.io/docs/get-started/installation/"
    exit 1
fi

# Verificar se a API está rodando
echo "🔍 Verificando se a API está em execução..."
if curl -s "http://localhost:5272/products/standard" > /dev/null; then
    echo "✅ API está rodando em http://localhost:5272"
else
    echo "❌ API não está respondendo em http://localhost:5272"
    echo "Por favor, execute 'dotnet run' na pasta src/AsyncTest.Api"
    exit 1
fi

echo ""
echo "📊 Iniciando testes de performance..."
echo ""

# Executar teste do endpoint standard
echo "1️⃣ Testando endpoint Standard (/products/standard)..."
k6 run standard-endpoint-test.js
echo "✅ Teste Standard concluído"
echo ""

# Executar teste do endpoint optimized  
echo "2️⃣ Testando endpoint Optimized (/products/optimized)..."
k6 run optimized-endpoint-test.js
echo "✅ Teste Optimized concluído"
echo ""

# Executar teste comparativo
echo "3️⃣ Executando teste comparativo..."
k6 run comparison-test.js
echo "✅ Teste Comparativo concluído"
echo ""

echo "🎉 Todos os testes foram executados com sucesso!"
echo ""
echo "📋 Relatórios gerados:"
echo "   • standard-endpoint-results.html"
echo "   • optimized-endpoint-results.html" 
echo "   • comparison-results.html"
echo ""
echo "🔍 Abra os arquivos HTML no navegador para visualizar os resultados detalhados."
