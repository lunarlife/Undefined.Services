using Undefined.Services.Entities.Components;

namespace Undefined.Services.Entities.Templates.Builder;

public interface IComponentTemplateBuilder<out T> where T : IComponent, new()
{
    public IComponentTemplateBuilder<T> Variable();
    public IComponentTemplateBuilder<T> DontRaiseAddEvent();
    public IComponentTemplateBuilder<T> Preset(Action<T> preset);
    public IEntityTemplateBuilder Build();
}