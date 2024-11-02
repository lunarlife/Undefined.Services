namespace Undefined.Services.Entities.Templates;

public class ComponentTemplate : IComponentTemplate
{
    public Type Type { get; }
    public bool ShouldAddEventBeRaised { get; }
    public bool IsImmutable { get; }
    public ComponentPreset? Preset { get; }

    public ComponentTemplate(Type type, bool isRemovable, bool mustRaiseAddEvent, ComponentPreset? preset)
    {
        Type = type;
        IsImmutable = isRemovable;
        Preset = preset;
        ShouldAddEventBeRaised = mustRaiseAddEvent;
    }
}