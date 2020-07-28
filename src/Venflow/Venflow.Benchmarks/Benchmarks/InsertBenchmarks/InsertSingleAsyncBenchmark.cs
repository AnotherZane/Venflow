﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using RepoDb;
using System.Threading.Tasks;
using Venflow.Benchmarks.Models;

namespace Venflow.Benchmarks.Benchmarks.InsertBenchmarks
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net48)]
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
    [RPlotExporter]
    public class InsertSingleAsyncBenchmark : BenchmarkBase
    {
        [GlobalSetup]
        public override async Task Setup()
        {
            await base.Setup();

            await EFCoreInsertSingleAsync();
            await VenflowInsertSingleAsync();
            await VenflowInsertSingleWithPKReturnAsync();
            await RepoDbInsertSingleAsync();
        }

        private Person GetDummyPerson()
        {
            return new Person { Name = "Insert" };
        }

        [Benchmark(Baseline = true)]
        public Task EFCoreInsertSingleAsync()
        {
            PersonDbContext.People.Add(GetDummyPerson());

            return PersonDbContext.SaveChangesAsync();
        }

        [Benchmark]
        public Task VenflowInsertSingleAsync()
        {
            return Database.People.Insert().Build().InsertAsync(GetDummyPerson());
        }

        [Benchmark]
        public Task VenflowInsertSingleWithPKReturnAsync()
        {
            return Database.People.Insert().SetIdentityColumns().Build().InsertAsync(GetDummyPerson());
        }

        [Benchmark]
        public Task RepoDbInsertSingleAsync()
        {
            return DbConnectionExtension.InsertAsync(Database.GetConnection(), GetDummyPerson());
        }

        [GlobalCleanup]
        public override Task Cleanup()
        {
            return base.Cleanup();
        }
    }
}