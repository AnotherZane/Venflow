<p align="center">
 <img width="100px" src="images/venflow.png" align="center" alt="GitHub Readme Stats" />
 <h1 align="center">Venflow</h1>
 <p align="center">A brand new, fast and lightweight ORM. | <a href="twentyfourminutes.github.io/venflow/">Documentation</a></p>
</p>
<p align="center">
<a href="https://www.nuget.org/packages/Venflow"><img alt="Nuget" src="https://img.shields.io/nuget/v/Venflow"></a> <a href="https://www.nuget.org/packages/Venflow"><img alt="Nuget" src="https://img.shields.io/nuget/dt/Venflow"></a> <a href="https://github.com/TwentyFourMinutes/Venflow/issues"><img alt="GitHub issues" src="https://img.shields.io/github/issues-raw/TwentyFourMinutes/Venflow"></a> <a href="https://github.com/TwentyFourMinutes/Venflow/blob/master/LICENSE"><img alt="GitHub" src="https://img.shields.io/github/license/TwentyFourMinutes/DulcisX"></a> <a href="https://discordapp.com/invite/EYKxkce"><img alt="Discord" src="https://discordapp.com/api/guilds/275377268728135680/widget.png"></a>
</p>

## About

Venflow is a brand new and from the ground up written ORM which tries to provide an alternative to EF-Core and many other ORMs. It allows you to define Models and their relations with each other. Additionally it maps all queries on its own while still maintaining great performance, with options for custom SQL.

Lets face it, EF-Core is awesome, but it can be slow, really slow. However this library tries to overcome that by providing similar features while maintaining great performance. Venflow comes with a very similar UX to Dapper and EF-Core, in order to keep the learning curve as low as possible. 

#### Features

- Simple change-tracking for update operations
- Autogenerated Inserts
- Autogenerated Deletes
- Autogenerated Query Materializer, Join Generator
- SQL Injection safe string Interpolated SQL


### Collaboration

If you want to collaborate on this project more, than creating issues and PR's, feel free to contact me on any of the mentioned contacts at the bottom of the file.

How you can help other than that? This can be done in numerous ways, over on the issue section, such as:

- Creating feature requests
- Creating pull requests
- Reporting bugs

## Installation

The alpha versions of Venflow can currently be downloaded on [nuget.org](https://www.nuget.org/packages/Venflow). However please do note that since this package is still in beta, it may still contain bugs and other issues.

Also you can install it via the **P**ackage **M**anager **C**onsole:

```
Install-Package Venflow
```

## Comparison

Benchmarking ORM's isn't quite an easy task, since there are a bunch of different factors which can alter the result in one way or another. I do not show any beautiful graphs here for the simple reason, that showing them would be pretty impractical, since there would be just too many. That is also the reason why I tried to come up with a composite number based on the benchmark results. If you still want check all the individual benchmarks, which you defiantly should, the source code can be found [here](./src/Venflow/Venflow.Benchmarks) and the results as `.csv` and `.md` are over [here](./benchmarks).

Lets just directly hop into the composite numbers of each tested ORM.

| ORM Name                                                   | Composite Score* | Mean Score* | Allocation Score* |
| :--------------------------------------------------------- | :--------------: | :---------: | :---------------: |
| #1 [Venflow](https://github.com/TwentyFourMinutes/Venflow) |      13.025      |   11.626    |       1.399       |
| #2 [Dapper](https://github.com/StackExchange/Dapper)**     |      16.389      |   12.656    |       3.733       |
| #3 [RepoDb](https://github.com/mikependon/RepoDb)**        |      44.797      |   38.626    |       6.170       |
| #4 [EFCore](https://github.com/dotnet/efcore)              |     249.145      |   192.909   |      56.236       |

\* Lower is considered to be better </br>
\*\* Do have missing benchmark entries for specific benchmark groups and therefor either might have better/worse scores.

Now how do I calculate this _magic number_? The formula is as following: 
```
compositeScore = Σ((meanTime / lowestMeanTimeOfGroup - 1) + (allocation / lowestAllocationOfGroup - 1) / 10)
```
A group is considered as a list of benchmark entries which are inside the same file and have the same \*count and target framework. Now as some ORM's don't have any benchmarks entries for specific benchmark groups it will take instead take the _lowest_ mean and the _lowest_  allocation from this group. The source code of the calculation can be found [here](./src/Venflow/Venflow.Score).

#### Disclaimer

The benchmarks themselves or even the calculation of the composite numbers may not be right and contain bugs. Therefor consider these results with a grain of salt. If you find any bugs inside the calculations or in the benchmarks please create an issue and I'll try to fix it ASAP.

## Is this package for you?

This package is more a competitor to Dapper than EF-Core since it supports Linq2Sql, Database first and migrations, which both Dapper and Venflow don't support out of the box. On the other hand you need to consider the Database you will end up using, if you aren't using PostgreSQL you will have to use a different ORM, at least at this state of the project. 

#### But why should I use Venflow over Dapper anyway?

Venflow supports a lot more things out of the box, such as automatic generated Delete/Insert statements, as well as simple change tracking to easily update specific entities. Another big factor, which probably is one of the biggest differences to Dapper, are the automatically generated materializers for queries. A lot of the times a materializer generated by Venflow will _always_ be faster, especially for bigger tables, than a hand written Dapper one. This is due to the nature of how Dapper and Venflow handle the parsing of SQL results.

## Basic usage

As already mentioned, Venflow tries to keep the learning curve from other ORM's as low as possible, therefor a lot of patterns will seem familiar from either EFCore or Dapper.

### Basic configuration

_The official documentation and guides can be found [here](https://twentyfourminutes.github.io/Venflow/)_

In Venflow you are reflecting your PostgreSQL database with the `Database` class, which will host all of your tables. This class represents a connection to your database and therefor doesn't support multi threaded use. In the following example we will configure a database containing two tables, `Blogs` and `Posts`. One Blog contains many posts and a post contains a single Blog.

```cs
public class BlogDatabase : Database
{
    public Table<Blog> Blogs { get; set; }
    public Table<Post> Posts { get; set; }

    public BlogDatabase() : base("Your connection string.")
    {
    }
}
```

Now lets configure the actual relation between Blogs and Posts through the `EntityConfiguration<T>` class. In the `Configure` , method you can configure several things such as the name of the table this entity should map to and much more. These configuration classes do automatically get discovered, if they are in the same assembly as the `Database` class. If they are not in the same assembly, you can override the `Configure` method in the `Database` class which passes in a `DatabaseOptionsBuilder`, which will allow you to specify assemblies which should also be searched for entity configurations.

```cs
public class BlogConfiguration : EntityConfiguration<Blog>
{
    protected override void Configure(IEntityBuilder<Blog> entityBuilder)
    {
        entityBuilder.HasMany(b => b.Posts)
                     .WithOne(p => p.Blog)
                     .UsingForeignKey(p => p.PostId);
    }
}
```

A instance of your `Database` class exposes the underlying connection and the actual CRUD builders. In the example below you can see how you would query a set of Blogs with their posts.

```cs
await using var database = new BlogDatabase(); // You should register a Transient/Scoped your DI Container.

const string sql = @"SELECT * FROM ""Blogs"" JOIN ""Posts"" ON ""Posts"".""BlogId"" = ""Blogs"".""Id""";

// You can re-use this in different BlogDatabase instances through the database.Blogs.QueryAsync() method
// If you intend to reuse the query below you need to pass the QueryBatch method false for the disposeCommand,
// otherwise the underyling command will be disposed after the first use.
var blogs = database.Blogs.QueryBatch(sql).JoinWith(x => x.Posts).QueryAsync();
```

Subsequent joins can be configured with the `ThenWith` method. Do note, that one handy feature of Venflow is string interpolated SQL. This means that most of the methods which accept SQL also have a sibling named `*Interpolated*` which will automatically extract the used variables and use a parameterized query instead.

## Road map

- Composed PK support
- Direct support for many to many relations
- Support for materialized Views
- Bulk operation support from [`PostgreSQL.Bulk`](https://github.com/TwentyFourMinutes/PostgreSQL.Bulk)

### Acknowledgements

I also want to mention all the other great packages out there, build by awesome people, which helped with building Venflow in one way or another such as being open-source.

- [Npgsql](https://github.com/npgsql/npgsql) by [the Npgsql core contributers](https://github.com/npgsql/) for providing an awesome and fast PostgreSQL data provider.
- [Sharplab](https://github.com/ashmind/SharpLab) by [ashmind](https://github.com/ashmind) and the [sharplab.io](https://sharplab.io) website for immensely simplify the generation for IL.
- [EF-Core](https://github.com/dotnet/efcore) by [Microsoft and the .Net team](https://github.com/dotnet) for providing the inspiration for such an awesome surface API.
- [RepoDb](https://github.com/mikependon/RepoDb) by [mikependon](https://github.com/mikependon) for providing the idea of generating runtime IL/Expressions to boost performance.
- [Fody](https://github.com/Fody/Fody) by [the Fody core contributers](https://github.com/Fody) for providing an easy way to IL weave Venflow.
- [GitHub](https://github.com/) for hosting the documentation with [GitHub Pages](https://pages.github.com/) and the repository itself.
- [DocFX](https://github.com/dotnet/docfx) by [Microsoft and the .Net team](https://github.com/dotnet) for providing a great any easy static markdown/documentation html generator.
- [Typora](https://typora.io/) for being a fully extensive and great markdown editor.
- [GitKraken](https://www.gitkraken.com/) for providing a full fledged git UI with a bunch of great features.
- [Shields](https://shields.io/) for providing awesome badges for the README.

#### Awesome people which helped in the development

- [LunarLite](https://github.com/LunarLite) for helping me with highly complex logically issues.
- [AnotherZane](https://github.com/AnotherZane) for being one of the early alpha testers. 
- [Jas](https://github.com/jas777) and [Fatal](https://github.com/fatalcenturion) for providing general surface API ideas.

## Notes

### Contact information

If you feel like something is not working as intended or you are experiencing issues, feel free to create an issue. Also for feature requests just create an issue. For further information feel free to send me a [mail](mailto:office@twenty-four.dev) to `office@twenty-four.dev` or message me on Discord `24_minutes#7496`.

## Sponsors

I wanna thank [JetBrains](https://www.jetbrains.com/?from=DulcisX) for providing me and the project with a free Open Source license for their whole JetBrains suite. Their Tools greatly improve the development speed of this Project. If you want to get a free Open Source license for your own project and their collaborators, visit their [Open Source page](https://www.jetbrains.com/opensource/).

<a href="https://www.jetbrains.com/?from=DulcisX"><img width="350px" src="images/jetbrains_logo.png"></a>