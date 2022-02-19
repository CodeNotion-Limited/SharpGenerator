using System;
using System.Text;

namespace Vit.Backoffice.Tests.SourceGeneration
{
    public class DbContextSeederGenerator
    {
        public StringBuilder Generate(StringBuilder sb, string propertyName, Type type)
        {
            sb.Append($"\t\t\tdataContext.Set<{type.Name}>().AddRange({propertyName});");
            return sb;
        }
    }
}