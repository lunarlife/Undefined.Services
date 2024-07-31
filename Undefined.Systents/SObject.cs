using Undefined.Verify;

namespace Undefined.Systents;

public class SObject : IDisposable
{
    private static readonly Type ComponentBaseType = typeof(ComponentBase);
    private readonly object _addComponentLock = new();
    private readonly List<ComponentBase> _components = [];

    public IReadOnlyList<ComponentBase> Components
    {
        get
        {
            lock (_addComponentLock) return _components.AsReadOnly();
        }
    }

    public Space Space { get; }

    internal SObject(Space space)
    {
        Space = space;
        foreach (var type in space.DefaultComponents) AddComponentInternal(type);
    }

    public void Dispose() => Destroy();

    public T AddComponent<T>() where T : ComponentBase
    {
        var type = typeof(T);
        return (T)AddComponentInternal(type);
    }

    private ComponentBase AddComponentInternal(Type type)
    {
        lock (_addComponentLock)
        {
            var component = Space._InternalDeclareComponent(this, type);
            _components.Add(component);
            return component;
        }
    }

    public ComponentBase AddComponent(Type type)
    {
        Verifying.Argument(ComponentBaseType.IsAssignableFrom(type),
            $"Type {type.Name} is not {nameof(ComponentBase)}.");
        return AddComponentInternal(type);
    }

    public void Destroy()
    {
        lock (_addComponentLock)
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
        lock (_addComponentLock) _components.Remove(component);
    }
}