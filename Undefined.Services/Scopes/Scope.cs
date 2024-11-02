using Undefined.Services.Application;
using Undefined.Verifying;

namespace Undefined.Services.Scopes;


public abstract class Scope
{
    private readonly object _servicesLock = new();

    public Scope()
    {
    }

    public T Resolve<T>() where T : IService => (T)ResolveInternal(typeof(T));

    public IService Resolve(Type serviceType)
    {
        Verify.Argument(typeof(IService).IsAssignableFrom(serviceType));
        return ResolveInternal(serviceType);
    }

    private IService ResolveInternal(Type serviceType)
    {
        lock (_servicesLock)
        {
            return ResolveService(serviceType);
        }
    }


    internal void ReleaseService(IService service)
    {
    }

    protected abstract IService ResolveService(Type serviceType);
}