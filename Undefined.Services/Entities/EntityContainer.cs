using Undefined.Events;
using Undefined.Services.Entities.Components;
using Undefined.Services.Entities.Templates;
using Undefined.Services.Events;
using Undefined.Services.Events.Components;
using Undefined.Services.Events.Entities;

namespace Undefined.Services.Entities;

public class EntityContainer : IEntityContainer
{
    internal readonly ComponentsInitializer ComponentsInitializer = new();

    private bool _isDisposed;
    private readonly object _objectsLock = new();
    private readonly Dictionary<Type, List<IComponent>> _components = [];
    private readonly List<IEntity> _entities = [];
    private readonly object _componentsLock = new();
    private readonly Event<EntityInstantiateEventArgs> _entityInstantiateEvent = new();

    public EntityContainer(IEntityTemplate template)
    {
        InstantiateTemplate = template;
    }

    public IEntityTemplate InstantiateTemplate { get; }
    public IReadOnlyList<IEntity> Entities
    {
        get
        {
            lock (_entities)
                return _entities.Clone();
        }
    }

    public IEventAccess<EntityInstantiateEventArgs> OnEntityInstantiate => _entityInstantiateEvent.Access;

    ~EntityContainer() => Dispose();

    public IReadOnlyList<IComponent> FindComponents(Type baseType) =>
        FindComponentsInternal<IComponent>(baseType);

    public IReadOnlyList<T> FindComponents<T>() where T : IComponent =>
        FindComponentsInternal<T>(typeof(T));

    private IReadOnlyList<T> FindComponentsInternal<T>(Type type) where T : IComponent
    {
        if (type.IsAbstract || type.IsInterface)
        {
            var subList = new List<List<T>>();
            lock (_componentsLock)
                foreach (var pair in _components)
                {
                    if (!type.IsAssignableFrom(pair.Key))
                        continue;
                    subList.Add(pair.Value.CloneAs<IComponent, T>());
                }

            return new MultiList<T>(subList);
        }

        lock (_componentsLock)
            return _components.TryGetValue(type, out var list) ? list.CloneAs<IComponent, T>() : [];
    }

    public IEntity Instantiate()
    {
        CheckIsNotDisposed();
        var e = InstantiateNewEntity();
        if(e is Entity entity) entity._Initialize(this);
        _entityInstantiateEvent.Raise(new EntityInstantiateEventArgs(e));
        lock (_objectsLock) _entities.Add(e);
        return e;
    }

    protected virtual IEntity InstantiateNewEntity() => new Entity();

    public T Instantiate<T>() where T : Component, new()
    {
        CheckIsNotDisposed();
        var entity = Instantiate();
        return entity.AddComponent<T>();
    }

    private void CheckIsNotDisposed()
    {
        if (_isDisposed) throw new ObjectDisposedException("Space already disposed.");
    }

    internal void _ComponentEvent(ComponentEventArgs args)
    {
        var component = args.Component;
        var componentType = component.GetType();
        switch (args)
        {
            case ComponentAddEventArgs:
                lock (_componentsLock)
                {
                    if (!_components.TryGetValue(componentType, out var list))
                    {
                        list = [];
                        _components.Add(componentType, list);
                    }
                    list.Add(component);
                }
                break;
        }
    }

    public void Dispose()
    {
        CheckIsNotDisposed();
        GC.SuppressFinalize(this);
        _isDisposed = true;
        _updateListener.Detach();
        lock (_servicesLock)
            foreach (var service in _services)
                if (service is IDisposable disposable)
                    disposable.Dispose();
    }
}