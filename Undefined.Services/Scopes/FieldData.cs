using System.Reflection;

namespace Undefined.Services.Scopes;

public struct FieldData
{
    public FieldInfo Field { get; }
    public FilterUpdateType FilterUpdateType { get; }
    public FieldType Type { get; }

    public FieldData(FieldInfo field, FilterUpdateType filterUpdateType, FieldType type)
    {
        Field = field;
        FilterUpdateType = filterUpdateType;
        Type = type;
    }
}