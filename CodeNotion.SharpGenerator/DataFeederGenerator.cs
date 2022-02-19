using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vit.Backoffice.Tests.SourceGeneration
{
    public class DataFeederGenerator
    {
        private readonly string[] _interfaces;

        public DataFeederGenerator(string[]? interfaces)
        {
            _interfaces = interfaces ?? Array.Empty<string>();
        }

        public StringBuilder Generate(string classNamespace, string className, IEnumerable<object> instances)
        {
            var instanceGenerator = new InstanceGenerator();
            var collectionGenerator = new CollectionGenerator(instanceGenerator);
            var dbContextSeederGenerator = new DbContextSeederGenerator();

            var typeGroups = instances
                .GroupBy(x =>
                {
                    var type = x.GetType();
                    return type.GetInterface("Castle.DynamicProxy.IProxyTargetAccessor") != null ? type.BaseType : type;
                })
                .ToArray();

            var typeNamespaces = typeGroups.Select(x => x.Key.Namespace).Distinct().ToArray();

            var sb = new StringBuilder(@"// ReSharper disable RedundantUsingDirective
// ReSharper disable ArrangeTrailingCommaInSinglelineLists
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

##NAMESPACES##

namespace ##NAMESPACE##
{
    public class ##NAME## ##INTERFACES##
    {");

            foreach (var typeGroup in typeGroups)
            {
                collectionGenerator.Generate(sb, typeGroup, typeGroup.Key.Name, typeGroup.Key.Name);
                sb.Append("\n");
            }

            sb.AppendLine($@"
        public void Seed(DbContext dataContext)
        {{");
            foreach (var typeGroup in typeGroups)
            {
                dbContextSeederGenerator.Generate(sb, typeGroup.Key.Name, typeGroup.Key);
                sb.Append("\n");
            }

            sb.Append(@"
        }
    }
}");

            var namespacesString = string.Join("\n", typeNamespaces.Concat(instanceGenerator.Namespaces).Distinct().Select(x => $"using {x};"));
            sb.Replace("##NAMESPACES##", namespacesString);
            sb.Replace("##NAMESPACE##", classNamespace);
            sb.Replace("##NAME##", className);
            sb.Replace("##INTERFACES##", _interfaces.Length > 0 ? $": {string.Join(", ", _interfaces)}" : string.Empty);

            return sb;
        }
    }
}