using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Loggers;
using AsyncTest.Benchmarks;

namespace AsyncTest.Benchmarks;

class Program
{
    static void Main(string[] args)
    {
        var config = ManualConfig.Create(DefaultConfig.Instance)
            .WithOptions(ConfigOptions.DisableOptimizationsValidator)
            .AddExporter(MarkdownExporter.GitHub)
            .AddExporter(HtmlExporter.Default)
            .AddExporter(JsonExporter.FullCompressed)
            .AddLogger(ConsoleLogger.Default);

        Console.WriteLine("🚀 AsyncTest Performance Benchmarks");
        Console.WriteLine("=====================================");
        Console.WriteLine();

        if (args.Length == 0)
        {
            Console.WriteLine("Escolha qual benchmark executar:");
            Console.WriteLine("1. Benchmark de Métodos Async (AsyncMethodsBenchmark)");
            Console.WriteLine("2. Benchmark de Garbage Collection (GarbageCollectionBenchmark)");
            Console.WriteLine("3. Benchmark de Memória Detalhado (DetailedMemoryBenchmark)");
            Console.WriteLine("4. Benchmark de State Machine (StateMachineBenchmark)");
            Console.WriteLine("5. Executar TODOS os benchmarks");
            Console.WriteLine();
            Console.Write("Digite sua escolha (1-5): ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    BenchmarkRunner.Run<AsyncMethodsBenchmark>(config);
                    break;
                case "2":
                    BenchmarkRunner.Run<GarbageCollectionBenchmark>(config);
                    break;
                case "3":
                    BenchmarkRunner.Run<DetailedMemoryBenchmark>(config);
                    break;
                case "4":
                    BenchmarkRunner.Run<StateMachineBenchmark>(config);
                    break;
                case "5":
                    RunAllBenchmarks(config);
                    break;
                default:
                    Console.WriteLine("Escolha inválida. Executando benchmark básico...");
                    BenchmarkRunner.Run<AsyncMethodsBenchmark>(config);
                    break;
            }
        }
        else
        {
            // Support command line arguments
            switch (args[0].ToLower())
            {
                case "async":
                    BenchmarkRunner.Run<AsyncMethodsBenchmark>(config);
                    break;
                case "gc":
                    BenchmarkRunner.Run<GarbageCollectionBenchmark>(config);
                    break;
                case "memory":
                    BenchmarkRunner.Run<DetailedMemoryBenchmark>(config);
                    break;
                case "statemachine":
                    BenchmarkRunner.Run<StateMachineBenchmark>(config);
                    break;
                case "all":
                    RunAllBenchmarks(config);
                    break;
                default:
                    Console.WriteLine($"Argumento desconhecido: {args[0]}");
                    Console.WriteLine("Argumentos válidos: async, gc, memory, statemachine, all");
                    break;
            }
        }

        Console.WriteLine("\n✅ Benchmarks concluídos!");
        Console.WriteLine("📊 Verifique os arquivos de resultado gerados na pasta BenchmarkDotNet.Artifacts");
    }

    private static void RunAllBenchmarks(IConfig config)
    {
        Console.WriteLine("🔄 Executando todos os benchmarks...");

        Console.WriteLine("\n1️⃣ Executando AsyncMethodsBenchmark...");
        BenchmarkRunner.Run<AsyncMethodsBenchmark>(config);

        Console.WriteLine("\n2️⃣ Executando GarbageCollectionBenchmark...");
        BenchmarkRunner.Run<GarbageCollectionBenchmark>(config);

        Console.WriteLine("\n3️⃣ Executando DetailedMemoryBenchmark...");
        BenchmarkRunner.Run<DetailedMemoryBenchmark>(config);

        Console.WriteLine("\n4️⃣ Executando StateMachineBenchmark...");
        BenchmarkRunner.Run<StateMachineBenchmark>(config);
    }
}
