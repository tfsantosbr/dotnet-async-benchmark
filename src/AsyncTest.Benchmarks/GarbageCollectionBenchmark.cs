using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using AsyncTest.Api.Application;

namespace AsyncTest.Benchmarks;

[MemoryDiagnoser]
[ThreadingDiagnoser]
[GcServer(true)]
[SimpleJob(RuntimeMoniker.Net90)]
[HideColumns("Error", "StdDev", "Median", "RatioSD")]
[MarkdownExporter]
[HtmlExporter]
[JsonExporter]
public class GarbageCollectionBenchmark
{
    private GetProductStandardAsync.DatabaseContext _standardDbContext = null!;
    private GetProductStandardAsync.Repository _standardRepository = null!;
    private GetProductStandardAsync.Handler _standardHandler = null!;

    private GetProductOptimizedAsync.DatabaseContext _optimizedDbContext = null!;
    private GetProductOptimizedAsync.Repository _optimizedRepository = null!;
    private GetProductOptimizedAsync.Handler _optimizedHandler = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Setup Standard dependencies
        _standardDbContext = new GetProductStandardAsync.DatabaseContext();
        _standardRepository = new GetProductStandardAsync.Repository(_standardDbContext);
        _standardHandler = new GetProductStandardAsync.Handler(_standardRepository);

        // Setup Optimized dependencies
        _optimizedDbContext = new GetProductOptimizedAsync.DatabaseContext();
        _optimizedRepository = new GetProductOptimizedAsync.Repository(_optimizedDbContext);
        _optimizedHandler = new GetProductOptimizedAsync.Handler(_optimizedRepository);

        // Force GC before each benchmark
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    [Benchmark(Baseline = true)]
    [IterationCount(100)]
    public async Task StandardAsync_GCPressure()
    {
        // Execute multiple calls to create GC pressure
        var tasks = new List<Task<string>>();
        for (int i = 0; i < 50; i++)
        {
            tasks.Add(_standardHandler.GetProductAsync());
        }
        await Task.WhenAll(tasks);
    }

    [Benchmark]
    [IterationCount(100)]
    public async Task OptimizedAsync_GCPressure()
    {
        // Execute multiple calls to create GC pressure
        var tasks = new List<Task<string>>();
        for (int i = 0; i < 50; i++)
        {
            tasks.Add(_optimizedHandler.GetProductAsync());
        }
        await Task.WhenAll(tasks);
    }

    [Benchmark]
    [IterationCount(50)]
    public async Task StandardAsync_ExtremeConcurrency()
    {
        // Test with extreme concurrency to see GC impact
        var tasks = new List<Task<string>>();
        for (int i = 0; i < 1000; i++)
        {
            tasks.Add(_standardHandler.GetProductAsync());
        }
        await Task.WhenAll(tasks);
    }

    [Benchmark]
    [IterationCount(50)]
    public async Task OptimizedAsync_ExtremeConcurrency()
    {
        // Test with extreme concurrency to see GC impact
        var tasks = new List<Task<string>>();
        for (int i = 0; i < 1000; i++)
        {
            tasks.Add(_optimizedHandler.GetProductAsync());
        }
        await Task.WhenAll(tasks);
    }
}
