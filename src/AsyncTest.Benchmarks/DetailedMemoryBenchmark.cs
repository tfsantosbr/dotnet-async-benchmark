using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using AsyncTest.Api.Application;
using System.Diagnostics;

namespace AsyncTest.Benchmarks;

[MemoryDiagnoser]
[ThreadingDiagnoser]
[SimpleJob(RuntimeMoniker.Net90)]
[HideColumns("Error", "StdDev")]
[MarkdownExporter]
[HtmlExporter]
public class DetailedMemoryBenchmark
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
    }

    [Benchmark(Baseline = true)]
    public async Task<MemoryMetrics> StandardAsync_MemoryAnalysis()
    {
        var beforeMemory = GC.GetTotalMemory(false);
        var beforeGen0 = GC.CollectionCount(0);
        var beforeGen1 = GC.CollectionCount(1);
        var beforeGen2 = GC.CollectionCount(2);

        // Execute operations
        var tasks = new Task<string>[20];
        for (int i = 0; i < 20; i++)
        {
            tasks[i] = _standardHandler.GetProductAsync();
        }
        await Task.WhenAll(tasks);

        var afterMemory = GC.GetTotalMemory(false);
        var afterGen0 = GC.CollectionCount(0);
        var afterGen1 = GC.CollectionCount(1);
        var afterGen2 = GC.CollectionCount(2);

        return new MemoryMetrics
        {
            MemoryAllocated = afterMemory - beforeMemory,
            Gen0Collections = afterGen0 - beforeGen0,
            Gen1Collections = afterGen1 - beforeGen1,
            Gen2Collections = afterGen2 - beforeGen2
        };
    }

    [Benchmark]
    public async Task<MemoryMetrics> OptimizedAsync_MemoryAnalysis()
    {
        var beforeMemory = GC.GetTotalMemory(false);
        var beforeGen0 = GC.CollectionCount(0);
        var beforeGen1 = GC.CollectionCount(1);
        var beforeGen2 = GC.CollectionCount(2);

        // Execute operations
        var tasks = new Task<string>[20];
        for (int i = 0; i < 20; i++)
        {
            tasks[i] = _optimizedHandler.GetProductAsync();
        }
        await Task.WhenAll(tasks);

        var afterMemory = GC.GetTotalMemory(false);
        var afterGen0 = GC.CollectionCount(0);
        var afterGen1 = GC.CollectionCount(1);
        var afterGen2 = GC.CollectionCount(2);

        return new MemoryMetrics
        {
            MemoryAllocated = afterMemory - beforeMemory,
            Gen0Collections = afterGen0 - beforeGen0,
            Gen1Collections = afterGen1 - beforeGen1,
            Gen2Collections = afterGen2 - beforeGen2
        };
    }
}

public class MemoryMetrics
{
    public long MemoryAllocated { get; set; }
    public int Gen0Collections { get; set; }
    public int Gen1Collections { get; set; }
    public int Gen2Collections { get; set; }

    public override string ToString()
    {
        return $"Memory: {MemoryAllocated:N0} bytes, GC: Gen0={Gen0Collections}, Gen1={Gen1Collections}, Gen2={Gen2Collections}";
    }
}
