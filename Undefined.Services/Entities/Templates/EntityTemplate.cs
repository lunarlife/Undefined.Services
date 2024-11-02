namespace Undefined.Services.Entities.Templates;

public class EntityTemplate : IEntityTemplate
{
    public IReadOnlyList<IComponentTemplate> Components { get; }

    public EntityTemplate(IReadOnlyList<IComponentTemplate> components)
    {
        Components = components;
    }
}