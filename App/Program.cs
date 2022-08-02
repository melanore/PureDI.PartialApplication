namespace App;

using Microsoft.FSharp.Core;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using static Domain.Models;
using static Domain.Functions;

[MemoryDiagnoser]
public class Benchmarks
{
    [Benchmark]
    public AuthorWithBooks PureDi() => 
        CompositionRoot.Resolve<FSharpFunc<int, AuthorWithBooks>>("Domain.AuthorApi.getAuthorWithBooksById")
            .Invoke(0);

    [Benchmark(Baseline = true)]
    public AuthorWithBooks Functions() => 
        AuthorApi.getAuthorWithBooksById(0);
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
    }
}