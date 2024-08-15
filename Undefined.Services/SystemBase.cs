namespace Undefined.Services;

public interface IService
{
    public ServicesSpace Space { get; }
    public bool IsDestroyed { get; }
    public void Destroy();
}

public abstract class ServiceBase : IService
{
    public ServicesSpace Space { get; }
    public bool IsDestroyed { get; private set; }

    public void Destroy()
    {
        IsDestroyed = true;
        ServicesInitializer.DisposeService(this);
    }
}