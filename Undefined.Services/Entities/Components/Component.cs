using Undefined.Events;
using Undefined.Services.Events.Components;

namespace Undefined.Services.Entities.Components;

public abstract class Component : IComponent
{
    internal const string INITIALIZE_METHOD_NAME = nameof(_Initialize);
    private readonly Event<ComponentRemoveEventArgs> _onRemove = new();
    
    public IEntity Entity { get; private set; }

    public IEventAccess<ComponentRemoveEventArgs> OnRemove => _onRemove.Access;


    private void _Initialize(Entity entity)
    {
        Entity = entity;
    }

    public void Remove() => Dispose();

    private void RemoveIntl() => _onRemove.Raise(new ComponentRemoveEventArgs(this));


    public void Dispose()
    {
        GC.SuppressFinalize(this);
        RemoveIntl();
    }
    ~Component() => Dispose();
}