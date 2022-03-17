using Microsoft.EntityFrameworkCore;

namespace CodeNotion.SharpGenerator.EfCore;

public class ScenarioBuilder
{
    private readonly string _basePath;
    private readonly Func<DbContext> _contextFactory;
    private string[]? Interfaces { get; set; }
    private LanguageVersion Version { get; set; } = LanguageVersion.CSharp10;
    private string[] Usings { get; set; } = Array.Empty<string>();

    public ScenarioBuilder(string basePath, Func<DbContext> contextFactory)
    {
        _basePath = basePath;
        _contextFactory = contextFactory;
    }

    public ScenarioBuilder BuildInterfaces(params string[] interfaces)
    {
        Interfaces = interfaces;
        return this;
    }

    public ScenarioBuilder SetLanguageVersion(LanguageVersion version)
    {
        Version = version;
        return this;
    }

    public ScenarioBuilder Using(params string[] usings)
    {
        Usings = usings;
        return this;
    }

    public async Task Build(string path, string name, Func<DbContext, Task> dataFactory)
    {
        await using var ctx = _contextFactory();
        var fullPath = $"{_basePath}/{path}/{name}.cs";
        if (File.Exists(fullPath))
        {
            return;
        }

        await dataFactory(ctx);

        // user tracked entries in the data factory
        var entries = ctx
            .ChangeTracker
            .Entries()
            .Select(x => x.Entity)
            .ToArray();

        ObjectLoader.Load(entries);

        // Getting all lazy loaded entries
        entries = ctx
            .ChangeTracker
            .Entries()
            .Select(x => x.Entity)
            .ToArray();

        var generatedDataSourceFile = Version switch
        {
            LanguageVersion.CSharp7_3 => new CSharp7_3.DataFeederGenerator(Interfaces).Generate(path.Replace('/', '.'), name, entries, Usings),
            LanguageVersion.CSharp10 => new CSharp10.DataFeederGenerator(Interfaces).Generate(path.Replace('/', '.'), name, entries, Usings),
            _ => throw new NotSupportedException($"Version {Version.ToString()} is not supported")
        };

        var directory = Path.GetDirectoryName(fullPath)!;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await File.WriteAllTextAsync(fullPath, generatedDataSourceFile.ToString());
    }
}

public enum LanguageVersion
{
    // ReSharper disable once InconsistentNaming
    CSharp7_3,
    CSharp10
}