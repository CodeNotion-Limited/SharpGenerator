using System.Collections;
using System.Text;

namespace CodeNotion.SharpGenerator.CSharp10;

internal class CollectionGenerator
{
    private readonly InstanceGenerator _instanceGenerator;

    internal CollectionGenerator(InstanceGenerator instanceGenerator)
    {
        _instanceGenerator = instanceGenerator;
    }

    internal StringBuilder Generate(StringBuilder sb, IEnumerable instances, string propertyName, string typeName)
    {
        sb.AppendLine($@"
    public {typeName}[] {propertyName} = 
    {{");
        foreach (var instance in instances)
        {
            sb.Append($"\t\tnew() ");
            _instanceGenerator.Generate(sb, instance);
            sb.AppendLine($",");
        }

        sb.Append("\t};");

        return sb;
    }
}