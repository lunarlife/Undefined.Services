using Undefined.Verify;

namespace Undefined.Systents;

public class SObject : IDisposable
{
    private static readonly Type ComponentBaseType = typeof(ComponentBase);

    private readonly List<ComponentBase> _components = [];
    private readonly Dictionary<Type, ComponentBase> _componentsTypes = [];
    
    private readonly object _componentLock = new();

    public IReadOnlyList<ComponentBase> Components
    {
        get
        {
            lock (_componentLock) return _components.AsReadOnly();
        }
    }

    public Space Space { get; }

    internal SObject(Space space)
    {
        Space = space;
        foreach (var type in space.DefaultComponents) AddComponentInternal(type);
    }

    public void Dispose() => Destroy();

    public T AddComponent<T>() where T : ComponentBase, new()
    {
        var type = typeof(T);
        return (T)AddComponentInternal(type);
    }

    private ComponentBase AddComponentInternal(Type type)
    {
        lock (_componentLock)
        {
            Verifying.Argument(!_componentsTypes.ContainsKey(type), $"Object already has component {type.Name}.");
            var component = Space._InternalDeclareComponent(this, type);
            _componentsTypes.Add(type, component);
            _components.Add(component);
            return component;
        }
    }

    public T GetComponent<T>() where T : ComponentBase, new() => (T)GetComponent(typeof(T));

    public bool TryGetComponent<T>(out T? component) where T : ComponentBase, new()
    {
        var hasComponent = TryGetComponent(typeof(T), out var c);
        component = hasComponent ? (T)c! : null;
        return hasComponent;
    }

    public bool TryGetComponent(Type type, out ComponentBase? component) =>
        _componentsTypes.TryGetValue(type, out component);

    public ComponentBase GetComponent(Type type)
    {
        Verifying.Argument(type is { IsAbstract: false, IsClass: true },
            $"Type must be not abstract class [Type {type.Name}].");
        Verifying.Argument(_componentsTypes.TryGetValue(type, out var component),
            $"Object does not have a component {type.Name}.");
        return component!;
    }

    public ComponentBase AddComponent(Type type)
    {
        Verifying.Argument(type is { IsAbstract: false, IsClass: true },
            $"Component must be not abstract class [Type {type.Name}].");
        Verifying.Argument(ComponentBaseType.IsAssignableFrom(type),
            $"Type {type.Name} is not {nameof(ComponentBase)}.");

        return AddComponentInternal(type);
    }

    public void Destroy()
    {
        lock (_componentLock)
            for (var i = 0; i < _components.Count; i++)
            {
                var component = _components[i];
                component.Destroy();
                Space._InternalDestroyComponent(component);
            }

        Space._InternalDestroyObject(this);
    }

    internal void _InternalDestroyComponent(ComponentBase component)
    {
        Verifying.Argument(component.Object == this, "Something was wrong.");
        lock (_componentLock) _components.Remove(component);
    }
}