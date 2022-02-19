using System.Collections;
using System.Text;

namespace Vit.Backoffice.Tests.SourceGeneration
{
    public class CollectionGenerator
    {
        private readonly InstanceGenerator _instanceGenerator;

        public CollectionGenerator(InstanceGenerator instanceGenerator)
        {
            _instanceGenerator = instanceGenerator;
        }

        public StringBuilder Generate(StringBuilder sb, IEnumerable instances, string propertyName, string typeName)
        {
            sb.AppendLine($@"
        public {typeName}[] {propertyName} = 
        {{");
            foreach (var instance in instances)
            {
                sb.Append($"            new() ");
                _instanceGenerator.Generate(sb, instance);
                sb.AppendLine($",");
            }

            sb.Append(@"        };");

            return sb;
        }
    }
}