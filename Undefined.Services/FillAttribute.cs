namespace Undefined.Services;

[AttributeUsage(AttributeTargets.Field)]
public class FillAttribute : Attribute
{
    public FillType Type { get; }

    public FillAttribute(FillType type = FillType.Init)
    {
        Type = type;
    }
}