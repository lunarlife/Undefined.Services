using Undefined.Services.Declaration;
using Undefined.Services.Scopes;

namespace Undefined.Services;

public interface IApplicationProvider
{
    public bool HasResolverForType(Type serviceType);
    public bool HasResolverForType<TService>() where TService : class, IService;
    public IServiceResolver GetResolver(Type serviceType);
    public IServiceResolver GetResolver<TService>() where TService : class, IService;
    public bool TryGetResolver(Type serviceType, out IServiceResolver? resolver);
    public bool TryGetResolver<TService>(out IServiceResolver? resolver) where TService : class, IService;
    public void AddResolver(IServiceResolver resolver);

    
    public bool HasScope(Type scopeType);
    public bool HasScope<TScope>() where TScope : Scope, new();
    public Scope GetScope<TScope>() where TScope : Scope, new();
    public Scope GetScope(Type scopeType);
    public bool TryGetScope(Type scopeType, out Scope? scope);
    public bool TryGetScope<TScope>(out Scope? scope) where TScope : Scope, new();
    public void DeclareScope<TScope>() where TScope : Scope, new();
}