using System;

namespace CodeNotion.SharpGenerator.Extensions;

internal static class TypeExtensions
{
    internal static bool IsSimple(this Type type) =>
        type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || type.IsEnum || type.IsValueType;
}