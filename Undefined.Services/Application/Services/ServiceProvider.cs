using Undefined.Services.Application.Services.Scopes;

namespace Undefined.Services.Application.Services;

public class ServiceProvider : IServiceProvider
{
    private CoreScope _rootScope;
    

    public ServiceProvider(IServiceCollection services)
    {
        _rootScope = new CoreScope(this, true);
    }
    
    public IServiceScope CreateScope() => new CoreScope(this, false);

    public IService GetService(Type serviceType) => throw new NotImplementedException();

    public T GetService<T>() where T : IService => throw new NotImplementedException();

}