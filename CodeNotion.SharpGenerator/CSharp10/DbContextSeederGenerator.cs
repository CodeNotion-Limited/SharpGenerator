using System;
using System.Text;

namespace CodeNotion.SharpGenerator.CSharp10;

internal class DbContextSeederGenerator
{
    internal static StringBuilder Generate(StringBuilder sb, string propertyName, Type type)
    {
        sb.Append($"\t\tdataContext.Set<{type.Name}>().AddRange({propertyName});");
        return sb;
    }
}