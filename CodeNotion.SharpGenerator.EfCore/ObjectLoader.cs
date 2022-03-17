using System.Collections;

namespace CodeNotion.SharpGenerator.EfCore;

internal class ObjectLoader
{
    internal static void Load(IEnumerable instances)
    {
        foreach (var instance in instances)
        {
            Load(instance);
        }
    }

    private static void Load(object instance)
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

            if (property.PropertyType.IsClass)
            {
                Load(value);
            }
        }
    }
}