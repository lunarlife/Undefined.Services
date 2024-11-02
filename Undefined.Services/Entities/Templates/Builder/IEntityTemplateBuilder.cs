using Undefined.Services.Entities.Components;

namespace Undefined.Services.Entities.Templates.Builder;

public interface IEntityTemplateBuilder
{
    public IReadOnlyList<IComponentTemplate> Components { get; }
    public IComponentTemplateBuilder<T> Component<T>() where T : IComponent, new();
    public IEntityTemplateBuilder Component<T>(IComponentTemplate template);
    public IEntityTemplate Build();
}