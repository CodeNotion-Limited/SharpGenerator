using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vit.Backoffice.Tests.SourceGeneration.Extensions;

namespace Vit.Backoffice.Tests.SourceGeneration
{
    public class InstanceGenerator
    {
        public readonly HashSet<string> Namespaces = new();

        public StringBuilder Generate(StringBuilder sb, object instance)
        {
            sb.Append("{");
            foreach (var propertyInfo in instance.GetType().GetProperties().Where(x => x.PropertyType.IsSimple()))
            {
                var value = propertyInfo.GetValue(instance);
                sb.Append($"{propertyInfo.Name} = {SerializeValue(value, propertyInfo.PropertyType)}, ");

                if (!Namespaces.Contains(propertyInfo.PropertyType.Namespace!))
                {
                    Namespaces.Add(propertyInfo.PropertyType.Namespace!);
                }
            }

            sb.Append("}");

            return sb;
        }

        private static string SerializeValue(object? value, Type type)
        {
            if (Nullable.GetUnderlyingType(type) != null)
            {
                type = Nullable.GetUnderlyingType(type)!;
            }

            return value switch
            {
                null => "null",
                string => $"@\"{value.ToString()!.Replace("\"", "\"\"")}\"",
                char => $"'{value}'",
                bool => value.ToString()!.ToLower(),
                int => value.ToString()!,
                double => $"{value}d"!,
                float => $"{value}f"!,
                decimal => $"{value}m"!,
                DateTime time => $"DateTime.Parse(\"{time:yyyy-MM-dd HH:mm:ss.fff}\")",
                Guid => $"new Guid(\"{value}\")",
                Enum => $"{type.Name}.{value}",
                _ => throw new Exception($"Unsupported type {value.GetType()}")
            };
        }
    }
}