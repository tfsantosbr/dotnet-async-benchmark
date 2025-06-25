#!/bin/bash

echo "🚀 AsyncTest Performance Benchmarks"
echo "===================================="
echo ""

# Verificar se estamos na pasta correta
if [ ! -f "dotnet-async-benchmark.sln" ]; then
    echo "❌ Execute este script na pasta raiz do projeto (onde está o arquivo .sln)"
    exit 1
fi

# Build do projeto
echo "🔨 Compilando projeto..."
dotnet build src/AsyncTest.Benchmarks/AsyncTest.Benchmarks.csproj -c Release
if [ $? -ne 0 ]; then
    echo "❌ Erro na compilação do projeto"
    exit 1
fi

echo "✅ Projeto compilado com sucesso"
echo ""

# Menu de opções
echo "Escolha qual benchmark executar:"
echo "1. Benchmark de Métodos Async (performance geral)"
echo "2. Benchmark de Garbage Collection (pressão GC)"
echo "3. Benchmark de Memória Detalhado (alocações)"
echo "4. Benchmark de State Machine (overhead)"
echo "5. Executar TODOS os benchmarks"
echo ""
read -p "Digite sua escolha (1-5): " choice

case $choice in
    1)
        echo "🏃 Executando AsyncMethodsBenchmark..."
        dotnet run --project src/AsyncTest.Benchmarks/AsyncTest.Benchmarks.csproj -c Release -- async
        ;;
    2)
        echo "🗑️ Executando GarbageCollectionBenchmark..."
        dotnet run --project src/AsyncTest.Benchmarks/AsyncTest.Benchmarks.csproj -c Release -- gc
        ;;
    3)
        echo "💾 Executando DetailedMemoryBenchmark..."
        dotnet run --project src/AsyncTest.Benchmarks/AsyncTest.Benchmarks.csproj -c Release -- memory
        ;;
    4)
        echo "⚙️ Executando StateMachineBenchmark..."
        dotnet run --project src/AsyncTest.Benchmarks/AsyncTest.Benchmarks.csproj -c Release -- statemachine
        ;;
    5)
        echo "🔄 Executando TODOS os benchmarks..."
        dotnet run --project src/AsyncTest.Benchmarks/AsyncTest.Benchmarks.csproj -c Release -- all
        ;;
    *)
        echo "❌ Escolha inválida. Executando benchmark básico..."
        dotnet run --project src/AsyncTest.Benchmarks/AsyncTest.Benchmarks.csproj -c Release -- async
        ;;
esac

echo ""
echo "✅ Benchmark(s) concluído(s)!"
echo "📊 Verifique os resultados em: src/AsyncTest.Benchmarks/BenchmarkDotNet.Artifacts"
echo ""
echo "📋 Arquivos gerados:"
echo "   • results/ - Resultados em HTML, Markdown e JSON"
echo "   • logs/ - Logs detalhados da execução"
echo ""
echo "🔍 Abra os arquivos HTML para visualização detalhada dos resultados."
