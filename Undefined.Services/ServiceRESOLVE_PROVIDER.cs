using Undefined.Services.Application;

namespace Undefined.Services;

public class ServiceRESOLVE_PROVIDER : IServiceRESOLVE_PROVIDER
{
    private readonly IApplicationProvider _provider;

    public ServiceRESOLVE_PROVIDER(IApplicationProvider provider)
    {
        _provider = provider;
    }

    public T Resolve<T>() where T : IService => (T)Resolve(typeof(T));

    public IService Resolve(Type serviceType) => _provider.GetResolver(serviceType).Resolve();
}