using Undefined.Services.Entities.Components;
using Undefined.Verifying;

namespace Undefined.Services.Entities.Templates.Builder;

public class ComponentTemplateBuilder<T> : IComponentTemplateBuilder<T> where T : IComponent, new()
{
    private readonly IEntityTemplateBuilder _entityBuilder;
    private bool _isImmutable = true;
    private Action<T> _preset;
    private bool _raiseEvent = true;

    public ComponentTemplateBuilder(IEntityTemplateBuilder entityBuilder)
    {
        _entityBuilder = entityBuilder;
    }

    public IComponentTemplateBuilder<T> Variable()
    {
        _isImmutable = false;
        return this;
    }

    public IComponentTemplateBuilder<T> DontRaiseAddEvent()
    {
        _raiseEvent = false;
        return this;
    }

    public IComponentTemplateBuilder<T> Preset(Action<T> preset)
    {
        _preset = preset;
        return this;
    }

    public IEntityTemplateBuilder Build() =>
        _entityBuilder.Component<T>(new ComponentTemplate(typeof(T), _isImmutable, _raiseEvent,
            c => { _preset(c.Is<T>()); }));
}