using Undefined.Services.Entities.Components;

namespace Undefined.Services.Entities.Templates.Builder;

public class EntityTemplateBuilder : IEntityTemplateBuilder
{
    private readonly List<IComponentTemplate> _components = [];
    
    public IReadOnlyList<IComponentTemplate> Components => _components.AsReadOnly();

    public IComponentTemplateBuilder<T> Component<T>() where T : IComponent, new() =>
        new ComponentTemplateBuilder<T>(this);

    public IEntityTemplateBuilder Component<T>(IComponentTemplate template)
    {
        _components.Add(template);
        return this;
    }

    public IEntityTemplate Build() => new EntityTemplate(_components);
}