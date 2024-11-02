using Undefined.Services.Application.Services.Scopes;

namespace Undefined.Services.Application.Services;

public class CoreScope : IServiceScope, IServiceProvider
{
    private readonly ServiceProvider _rootProvider;
    private readonly bool _isRootScope;
    
    public IServiceProvider ServiceProvider { get; }

    public CoreScope(ServiceProvider rootProvider, bool isRootScope)
    {
        _rootProvider = rootProvider;
        _isRootScope = isRootScope;
    }
    public void Dispose()
    {
        
    }

    public IServiceScope CreateScope() => _rootProvider.CreateScope();

    public T GetService<T>() where T : IService => throw new NotImplementedException();

    public IService GetService(Type serviceType) => throw new NotImplementedException();
}