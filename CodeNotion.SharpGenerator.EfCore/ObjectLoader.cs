using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CodeNotion.SharpGenerator.EfCore
{
    public class ObjectLoader
    {
        public readonly HashSet<object> VisitedObject = new();

        public void Load(IEnumerable instances)
        {
            foreach (var instance in instances)
            {
                Load(instance);
            }
        }

        public void Load(object instance)
        {
            var instanceType = instance.GetType();
            var properties = instanceType.GetProperties();
            foreach (var property in properties)
            {
                if ((typeof(IEnumerable).IsAssignableFrom(property.PropertyType)))
                {
                    continue;
                }

                var value = property.GetValue(instance);
                if (value is null)
                {
                    continue;
                }

                if (VisitedObject.Any(x => x == value))
                {
                    continue;
                }

                if (property.PropertyType.IsClass)
                {
                    Load(value);
                }
            }
        }
    }
}