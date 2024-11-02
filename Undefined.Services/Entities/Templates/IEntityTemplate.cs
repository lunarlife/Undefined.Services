namespace Undefined.Services.Entities.Templates;

public interface IEntityTemplate
{
    public IReadOnlyList<IComponentTemplate> Components { get; }
}