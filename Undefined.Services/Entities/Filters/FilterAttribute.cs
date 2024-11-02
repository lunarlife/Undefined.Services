namespace Undefined.Services.Entities.Filters;

[AttributeUsage(AttributeTargets.Field)]
public class FilterAttribute : Attribute
{
    public FilterAutoUpdate AutoUpdate { get; }

    public FilterAttribute(FilterAutoUpdate autoUpdate = FilterAutoUpdate.Init)
    {
        AutoUpdate = autoUpdate;
    }
}