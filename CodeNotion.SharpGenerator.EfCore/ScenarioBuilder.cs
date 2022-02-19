using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vit.Backoffice.Tests.SourceGeneration;

namespace CodeNotion.SharpGenerator.EfCore
{
    public class ScenarioBuilder
    {
        private readonly string _basePath;
        private readonly Func<DbContext> _contextFactory;
        private string[]? Interfaces { get; set; }

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

            new ObjectLoader().Load(entries);

            // Getting all lazy loaded entries
            entries = ctx
                .ChangeTracker
                .Entries()
                .Select(x => x.Entity)
                .ToArray();

            var generatedDataSourceFile = new DataFeederGenerator(Interfaces).Generate(path.Replace('/', '.'), name, entries);

            var directory = Path.GetDirectoryName(fullPath)!;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(fullPath, generatedDataSourceFile.ToString());
        }
    }
}