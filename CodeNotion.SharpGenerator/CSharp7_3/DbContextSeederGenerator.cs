using System;
using System.Text;

namespace CodeNotion.SharpGenerator.CSharp7_3;

internal class DbContextSeederGenerator
{
    internal static StringBuilder Generate(StringBuilder sb, string propertyName, Type type)
    {
        sb.Append($"\t\t\tdataContext.Set<{type.Name}>().AddRange({propertyName});");
        return sb;
    }
}