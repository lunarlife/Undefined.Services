using Undefined.Events;
using Undefined.Services.Entities.Components;
using Undefined.Services.Entities.Templates;
using Undefined.Services.Events;
using Undefined.Services.Events.Entities;

namespace Undefined.Services.Entities;

public interface IEntityContainer : IService, IDisposable
{
    public IReadOnlyList<IEntity> Entities { get; }
    public IEventAccess<EntityInstantiateEventArgs> OnEntityInstantiate { get; }

    public IEntityTemplate InstantiateTemplate { get; }
    public IReadOnlyList<IComponent> FindComponents(Type baseType);

    public IReadOnlyList<T> FindComponents<T>() where T : IComponent;
    public IEntity Instantiate();
    public T Instantiate<T>() where T : Component, new();
}