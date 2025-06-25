@echo off
setlocal enabledelayedexpansion

echo 🚀 AsyncTest Performance Benchmarks
echo ====================================
echo.

REM Verificar se estamos na pasta correta
if not exist "dotnet-async-benchmark.sln" (
    echo ❌ Execute este script na pasta raiz do projeto (onde está o arquivo .sln)
    pause
    exit /b 1
)

REM Build do projeto
echo 🔨 Compilando projeto...
dotnet build src\AsyncTest.Benchmarks\AsyncTest.Benchmarks.csproj -c Release
if %errorlevel% neq 0 (
    echo ❌ Erro na compilação do projeto
    pause
    exit /b 1
)

echo ✅ Projeto compilado com sucesso
echo.

REM Menu de opções
echo Escolha qual benchmark executar:
echo 1. Benchmark de Métodos Async (performance geral)
echo 2. Benchmark de Garbage Collection (pressão GC)
echo 3. Benchmark de Memória Detalhado (alocações)
echo 4. Benchmark de State Machine (overhead)
echo 5. Executar TODOS os benchmarks
echo.
set /p choice="Digite sua escolha (1-5): "

if "%choice%"=="1" (
    echo 🏃 Executando AsyncMethodsBenchmark...
    dotnet run --project src\AsyncTest.Benchmarks\AsyncTest.Benchmarks.csproj -c Release -- async
) else if "%choice%"=="2" (
    echo 🗑️ Executando GarbageCollectionBenchmark...
    dotnet run --project src\AsyncTest.Benchmarks\AsyncTest.Benchmarks.csproj -c Release -- gc
) else if "%choice%"=="3" (
    echo 💾 Executando DetailedMemoryBenchmark...
    dotnet run --project src\AsyncTest.Benchmarks\AsyncTest.Benchmarks.csproj -c Release -- memory
) else if "%choice%"=="4" (
    echo ⚙️ Executando StateMachineBenchmark...
    dotnet run --project src\AsyncTest.Benchmarks\AsyncTest.Benchmarks.csproj -c Release -- statemachine
) else if "%choice%"=="5" (
    echo 🔄 Executando TODOS os benchmarks...
    dotnet run --project src\AsyncTest.Benchmarks\AsyncTest.Benchmarks.csproj -c Release -- all
) else (
    echo ❌ Escolha inválida. Executando benchmark básico...
    dotnet run --project src\AsyncTest.Benchmarks\AsyncTest.Benchmarks.csproj -c Release -- async
)

echo.
echo ✅ Benchmark(s) concluído(s)!
echo 📊 Verifique os resultados em: src\AsyncTest.Benchmarks\BenchmarkDotNet.Artifacts
echo.
echo 📋 Arquivos gerados:
echo    • results\ - Resultados em HTML, Markdown e JSON
echo    • logs\ - Logs detalhados da execução
echo.
echo 🔍 Abra os arquivos HTML para visualização detalhada dos resultados.
echo.
pause
