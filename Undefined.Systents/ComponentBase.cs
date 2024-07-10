namespace Undefined.Systents.Components;

public abstract class ComponentBase : IDisposable
{
    public Object Object { get; private set; }
    public Space Space => Object.Space;

    internal void _InternalInit(Object obj)
    {
        Object = obj;
    }

    protected virtual void OnComponentDestroy()
    {
    }

    public void Destroy()
    {
        OnComponentDestroy();
    }

    public void Dispose() => Destroy();
}