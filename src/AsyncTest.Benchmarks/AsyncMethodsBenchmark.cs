using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using AsyncTest.Api.Application;

namespace AsyncTest.Benchmarks;

[MemoryDiagnoser]
[GcServer(true)]
[SimpleJob(RuntimeMoniker.Net90, baseline: true)]
[SimpleJob(RuntimeMoniker.Net80)]
[HideColumns("Error", "StdDev", "Median", "RatioSD")]
[MarkdownExporter]
[HtmlExporter]
[JsonExporter]
public class AsyncMethodsBenchmark
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
    [BenchmarkCategory("SingleCall")]
    public async Task<string> StandardAsync_SingleCall()
    {
        return await _standardHandler.GetProductAsync();
    }

    [Benchmark]
    [BenchmarkCategory("SingleCall")]
    public async Task<string> OptimizedAsync_SingleCall()
    {
        return await _optimizedHandler.GetProductAsync();
    }

    [Benchmark]
    [BenchmarkCategory("MultipleCalls")]
    public async Task<string[]> StandardAsync_MultipleCalls()
    {
        var tasks = new Task<string>[10];
        for (int i = 0; i < 10; i++)
        {
            tasks[i] = _standardHandler.GetProductAsync();
        }
        return await Task.WhenAll(tasks);
    }

    [Benchmark]
    [BenchmarkCategory("MultipleCalls")]
    public async Task<string[]> OptimizedAsync_MultipleCalls()
    {
        var tasks = new Task<string>[10];
        for (int i = 0; i < 10; i++)
        {
            tasks[i] = _optimizedHandler.GetProductAsync();
        }
        return await Task.WhenAll(tasks);
    }

    [Benchmark]
    [BenchmarkCategory("HighConcurrency")]
    public async Task<string[]> StandardAsync_HighConcurrency()
    {
        var tasks = new Task<string>[100];
        for (int i = 0; i < 100; i++)
        {
            tasks[i] = _standardHandler.GetProductAsync();
        }
        return await Task.WhenAll(tasks);
    }

    [Benchmark]
    [BenchmarkCategory("HighConcurrency")]
    public async Task<string[]> OptimizedAsync_HighConcurrency()
    {
        var tasks = new Task<string>[100];
        for (int i = 0; i < 100; i++)
        {
            tasks[i] = _optimizedHandler.GetProductAsync();
        }
        return await Task.WhenAll(tasks);
    }
}
