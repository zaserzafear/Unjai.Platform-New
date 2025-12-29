using System.Collections.ObjectModel;

namespace Unjai.Platform.Extensions.ObjectMapper;

public class TypeMappingCollection : KeyedCollection<Type, (Type Source, Type Destination)>
{
    protected override Type GetKeyForItem((Type Source, Type Destination) item)
        => item.Source;

    public void Add<TSource, TDestination>()
        => Add((typeof(TSource), typeof(TDestination)));
}
