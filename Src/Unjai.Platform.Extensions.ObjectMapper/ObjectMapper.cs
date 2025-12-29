using System.Reflection;

namespace Unjai.Platform.Extensions.ObjectMapper;

public interface IObjectMapper
{
    TDestination Map<TDestination>(object source);
}

public class ObjectMapper : IObjectMapper
{
    private readonly Dictionary<(Type, Type), Func<object, object>> _mapFuncs;

    public ObjectMapper(IEnumerable<AutoMapperProfile> profiles)
    {
        _mapFuncs = new();

        foreach (var profile in profiles)
        {
            foreach (var (source, dest) in profile.Mappings)
            {
                // simple bind by property name
                _mapFuncs[(source, dest)] = CreateMapFunction(source, dest);
            }
        }
    }

    private static Func<object, object> CreateMapFunction(Type source, Type dest)
    {
        var ctor = dest.GetConstructors().FirstOrDefault();

        if (ctor != null && ctor.GetParameters().Length > 0)
        {
            var ctorParams = ctor.GetParameters();

            return (srcObj) =>
            {
                var args = new object?[ctorParams.Length];

                foreach (var (param, index) in ctorParams.Select((p, i) => (p, i)))
                {
                    var srcProp = source.GetProperty(param.Name!, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    args[index] = srcProp?.GetValue(srcObj);
                }

                return ctor.Invoke(args);
            };
        }

        return (srcObj) =>
        {
            var destObj = Activator.CreateInstance(dest)!;

            foreach (var sProp in source.GetProperties())
            {
                var dProp = dest.GetProperty(sProp.Name);
                if (dProp == null || !dProp.CanWrite)
                    continue;

                dProp.SetValue(destObj, sProp.GetValue(srcObj));
            }

            return destObj;
        };
    }

    public TDestination Map<TDestination>(object source)
    {
        var key = (source.GetType(), typeof(TDestination));
        if (_mapFuncs.TryGetValue(key, out var func))
        {
            return (TDestination)func(source);
        }

        throw new InvalidOperationException($"Mapping not found: {key}");
    }
}
