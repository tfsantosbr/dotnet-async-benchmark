@echo off
setlocal enabledelayedexpansion

echo 🚀 Executando Testes de Performance K6 - AsyncTest API
echo ==================================================

REM Verificar se K6 está instalado
k6 version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ K6 não está instalado. Por favor, instale o K6 primeiro.
    echo Instruções: https://k6.io/docs/get-started/installation/
    pause
    exit /b 1
)

REM Verificar se a API está rodando
echo 🔍 Verificando se a API está em execução...
curl -s "http://localhost:5272/products/standard" >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ API não está respondendo em http://localhost:5272
    echo Por favor, execute 'dotnet run' na pasta src/AsyncTest.Api
    pause
    exit /b 1
)

echo ✅ API está rodando em http://localhost:5272
echo.
echo 📊 Iniciando testes de performance...
echo.

REM Executar teste do endpoint standard
echo 1️⃣ Testando endpoint Standard (/products/standard)...
k6 run standard-endpoint-test.js
if %errorlevel% neq 0 (
    echo ❌ Erro no teste Standard
    pause
    exit /b 1
)
echo ✅ Teste Standard concluído
echo.

REM Executar teste do endpoint optimized
echo 2️⃣ Testando endpoint Optimized (/products/optimized)...
k6 run optimized-endpoint-test.js
if %errorlevel% neq 0 (
    echo ❌ Erro no teste Optimized
    pause
    exit /b 1
)
echo ✅ Teste Optimized concluído
echo.

REM Executar teste comparativo
echo 3️⃣ Executando teste comparativo...
k6 run comparison-test.js
if %errorlevel% neq 0 (
    echo ❌ Erro no teste Comparativo
    pause
    exit /b 1
)
echo ✅ Teste Comparativo concluído
echo.

echo 🎉 Todos os testes foram executados com sucesso!
echo.
echo 📋 Relatórios gerados:
echo    • standard-endpoint-results.html
echo    • optimized-endpoint-results.html
echo    • comparison-results.html
echo.
echo 🔍 Abra os arquivos HTML no navegador para visualizar os resultados detalhados.
echo.
pause
