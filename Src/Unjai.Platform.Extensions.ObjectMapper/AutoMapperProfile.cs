namespace Unjai.Platform.Extensions.ObjectMapper;

public abstract class AutoMapperProfile
{
    public TypeMappingCollection Mappings { get; } = new();

    protected void CreateMap<TSource, TDestination>()
        => Mappings.Add<TSource, TDestination>();
}
