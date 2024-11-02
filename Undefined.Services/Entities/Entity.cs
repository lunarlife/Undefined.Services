using Undefined.Events;
using Undefined.Services.Entities.Components;
using Undefined.Services.Events.Components;
using Undefined.Services.Events.Entities;
using Undefined.Verifying;

namespace Undefined.Services.Entities;

public class Entity : IEntity, IDisposable
{
    private static readonly Type ComponentBaseType = typeof(IComponent);

    private readonly List<IComponent> _components = [];
    private readonly Dictionary<Type, IComponent> _componentsTypes = [];

    private readonly object _componentsLock = new();

    private readonly Event<EntityRemoveEventArgs> _onRemove = new();
    private readonly Event<ComponentAddEventArgs> _onComponentAdd = new();
    private EntityContainer _container;

    public IEntityContainer Container => _container;

    public List<IComponent> Components
    {
        get
        {
            lock (_componentsLock) return _components.Clone();
        }
    }

    public IEventAccess<EntityRemoveEventArgs> OnRemove => _onRemove.Access;
    public IEventAccess<ComponentAddEventArgs> OnComponentAdd => _onComponentAdd.Access;


    internal void _Initialize(EntityContainer container)
    {
        _container = container;
        var template = container.InstantiateTemplate;
        var templateComponents = template.Components;
        lock (_componentsLock)
        {
            foreach (var tc in templateComponents)
            {
                var component = CreateComponent(tc.Type, true, tc.IsImmutable);
                ApplyComponent(component, tc.ShouldAddEventBeRaised);
            }
        }
    }


    public T AddComponent<T>() where T : IComponent, new()
    {
        var type = typeof(T);
        return (T)AddComponentIntl(type);
    }

    private IComponent AddComponentIntl(Type type)
    {
        lock (_componentsLock)
        {
            Verify.Argument(!_componentsTypes.ContainsKey(type), $"Object already has component {type.Name}.");
            var component = CreateComponent(type, false, false);
            ApplyComponent(component, true);
            return component;
        }
    }

    private void ApplyComponent(Component component, bool raiseEvent)
    {
        lock (_componentsLock)
        {
            var args = new ComponentAddEventArgs(component);
            if (raiseEvent)
                _onComponentAdd.Raise(args);
            _container._ComponentEvent(args);
            _componentsTypes.Add(component.GetType(), component);
            _components.Add(component);
        }
    }

    private Component CreateComponent(Type type, bool isDefault, bool isImmutable) =>
        _container.ComponentsInitializer.CreateComponent(type,
            new ComponentInitializeData(this, isDefault, isImmutable));

    public T GetComponent<T>() where T : IComponent => (T)GetComponent(typeof(T));

    public bool TryGetComponent<T>(out T? component) where T : IComponent
    {
        var hasComponent = TryGetComponent(typeof(T), out var c);
        component = hasComponent ? (T)c! : default;
        return hasComponent;
    }

    public bool TryGetComponent(Type type, out IComponent? component) =>
        _componentsTypes.TryGetValue(type, out component);

    public IComponent GetComponent(Type type)
    {
        Verify.Argument(!type.IsValueType,
            $"Structs are not allowed. [Type {type.Name}].");
        IComponent? component;
        if (type.IsAbstract || type.IsInterface)
        {
            lock (_componentsLock)
                component = Verify.NotNull(_components.FirstOrDefault(type.IsInstanceOfType));
        }
        else
        {
            lock (_componentsLock)
                Verify.Argument(_componentsTypes.TryGetValue(type, out component),
                    $"Object does not have a component {type.Name}.");
        }

        return component!;
    }

    public IComponent AddComponent(Type type)
    {
        Verify.Argument(type is { IsAbstract: false, IsClass: true },
            $"Component must be not abstract class [Type {type.Name}].");
        Verify.Argument(ComponentBaseType.IsAssignableFrom(type),
            $"Type {type.Name} is not {nameof(IComponent)}.");

        return AddComponentIntl(type);
    }

    internal void _InternalDestroyComponent(IComponent component)
    {
        Verify.Argument(component.Object == this, "Something was wrong.");
        lock (_componentsLock) _components.Remove(component);
    }


    public void Remove()
    {
        lock (_componentsLock)
            for (var i = 0; i < _components.Count; i++)
            {
                var component = _components[i];
                component.Remove();
            }

        Space._InternalDestroyObject(this);
    }

    public void Dispose() => Remove();
}