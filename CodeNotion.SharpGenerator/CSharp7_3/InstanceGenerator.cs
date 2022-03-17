using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeNotion.SharpGenerator.Extensions;

namespace CodeNotion.SharpGenerator.CSharp7_3;

internal class InstanceGenerator
{
    internal readonly HashSet<string> Namespaces = new();

    internal StringBuilder Generate(StringBuilder sb, object instance)
    {
        sb.Append("{ ");
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
            short => value.ToString()!,
            double => $"{value.ToString().Replace(',', '.')}d"!,
            float => $"{value.ToString().Replace(',', '.')}f"!,
            decimal => $"{value.ToString().Replace(',', '.')}m"!,
            DateTime time => $"DateTime.Parse(\"{time:yyyy-MM-dd HH:mm:ss.fff}\")",
            Guid => $"new Guid(\"{value}\")",
            Enum => $"{type.Name}.{value}",
            _ => throw new Exception($"Unsupported type {value.GetType()}")
        };
    }
}