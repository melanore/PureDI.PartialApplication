namespace App;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

[MemoryDiagnoser]
public class Benchmarks
{
    private static readonly CompositionRoot CompositionRoot = new ();
    
    [Benchmark]
    public AuthorWithBooks PureDi() =>
        CompositionRoot.getAuthorWithBooksById.Invoke(0);

    [Benchmark(Baseline = true)]
    public AuthorWithBooks Functions() => 
        Domain.Functions.AuthorApi.getAuthorWithBooksById(0);
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
    }
}