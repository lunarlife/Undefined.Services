using Undefined.Services.Entities.Components;

namespace Undefined.Services.Entities.Templates;

public delegate void ComponentPreset(IComponent component);

public interface IComponentTemplate
{
    public Type Type { get; }
    public bool ShouldAddEventBeRaised { get; }
    public bool IsImmutable { get; }
    public ComponentPreset? Preset { get; }
}