using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Vit.Backoffice.Tests.SourceGeneration.Extensions
{
    internal static class TypeExtensions
    {
        internal static bool IsSimple(this Type type) =>
            type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || type.IsEnum || type.IsValueType;
    }
}