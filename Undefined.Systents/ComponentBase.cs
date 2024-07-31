namespace Undefined.Systents;

public abstract class ComponentBase : IDisposable
{
    public SObject Object { get; private set; }
    public Space Space => Object.Space;

    public void Dispose() => Destroy();

    private void Init(SObject obj)
    {
        Object = obj;
    }

    protected virtual void OnComponentDestroy()
    {
    }

    public void Destroy()
    {
        OnComponentDestroy();
        Object._InternalDestroyComponent(this);
    }
}