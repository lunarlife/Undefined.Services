using Undefined.Events;
using Undefined.Services.Entities.Components;
using Undefined.Services.Events.Components;
using Undefined.Services.Events.Entities;

namespace Undefined.Services.Entities;

public interface IEntity
{
    public IEntityContainer Container { get; }
    public List<IComponent> Components { get; }
    public IEventAccess<EntityRemoveEventArgs> OnRemove { get; }
    public IEventAccess<ComponentAddEventArgs> OnComponentAdd { get; }

    public T AddComponent<T>() where T : IComponent, new();
    public IComponent AddComponent(Type type);

    public T GetComponent<T>() where T : IComponent;
    public IComponent GetComponent(Type type);
    public bool TryGetComponent<T>(out T? component) where T : IComponent;
    public bool TryGetComponent(Type type, out IComponent? component);

}