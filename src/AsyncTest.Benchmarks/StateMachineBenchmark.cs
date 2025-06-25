using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using AsyncTest.Api.Application;

namespace AsyncTest.Benchmarks;

[MemoryDiagnoser]
[ThreadingDiagnoser]
[SimpleJob(RuntimeMoniker.Net90)]
[HideColumns("Error", "StdDev")]
[MarkdownExporter]
[HtmlExporter]
[JsonExporter]
public class StateMachineBenchmark
{
    private GetProductStandardAsync.DatabaseContext _standardDbContext = null!;
    private GetProductStandardAsync.Repository _standardRepository = null!;
    private GetProductStandardAsync.Handler _standardHandler = null!;

    private GetProductOptimizedAsync.DatabaseContext _optimizedDbContext = null!;
    private GetProductOptimizedAsync.Repository _optimizedRepository = null!;
    private GetProductOptimizedAsync.Handler _optimizedHandler = null!;

    [Params(1, 10, 100, 1000)]
    public int ConcurrentTasks { get; set; }

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
    [BenchmarkCategory("StateMachine")]
    public async Task StandardAsync_StateMachineOverhead()
    {
        var tasks = new Task<string>[ConcurrentTasks];
        for (int i = 0; i < ConcurrentTasks; i++)
        {
            tasks[i] = _standardHandler.GetProductAsync();
        }
        await Task.WhenAll(tasks);
    }

    [Benchmark]
    [BenchmarkCategory("StateMachine")]
    public async Task OptimizedAsync_StateMachineOverhead()
    {
        var tasks = new Task<string>[ConcurrentTasks];
        for (int i = 0; i < ConcurrentTasks; i++)
        {
            tasks[i] = _optimizedHandler.GetProductAsync();
        }
        await Task.WhenAll(tasks);
    }

    [Benchmark]
    [BenchmarkCategory("ThroughputTest")]
    public async Task StandardAsync_ThroughputTest()
    {
        // Test sustained throughput
        var totalTasks = 500;
        var batchSize = 50;

        for (int batch = 0; batch < totalTasks / batchSize; batch++)
        {
            var tasks = new Task<string>[batchSize];
            for (int i = 0; i < batchSize; i++)
            {
                tasks[i] = _standardHandler.GetProductAsync();
            }
            await Task.WhenAll(tasks);
        }
    }

    [Benchmark]
    [BenchmarkCategory("ThroughputTest")]
    public async Task OptimizedAsync_ThroughputTest()
    {
        // Test sustained throughput
        var totalTasks = 500;
        var batchSize = 50;

        for (int batch = 0; batch < totalTasks / batchSize; batch++)
        {
            var tasks = new Task<string>[batchSize];
            for (int i = 0; i < batchSize; i++)
            {
                tasks[i] = _optimizedHandler.GetProductAsync();
            }
            await Task.WhenAll(tasks);
        }
    }
}
