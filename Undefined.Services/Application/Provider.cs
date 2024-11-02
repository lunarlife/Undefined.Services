using Undefined.Services.Declaration;
using Undefined.Services.Scopes;
using Undefined.Verifying;

namespace Undefined.Services.Application;

internal class Provider : IApplicationProvider
{
    private readonly Dictionary<Type, IServiceResolver> _resolversTypes = [];
    private readonly List<IServiceResolver> _resolvers = [];

    private readonly Dictionary<Type, Scope> _scopesTypes = [];
    private readonly List<Scope> _scopes = [];

    private readonly object _resolversLock = new();
    private readonly object _scopesLock = new();

    public bool HasResolverForType(Type serviceType)
    {
        lock (_resolversLock)
            return _resolversTypes.ContainsKey(serviceType);
    }


    public bool HasResolverForType<TService>() where TService : class, IService => HasResolverForType(typeof(TService));

    public IServiceBuilderOld<TService> DeclareService<TService>()
        where TService : class, IService
    {
        var type = typeof(TService);
        lock (_resolversLock)
        {
            Verify.Argument(_resolversTypes.ContainsKey(type),
                $"{nameof(ApplicationSpace)} has already declared service {type.Name}.");
            return new DefaultServiceBuilder<TService>(this);
        }
    }

    public IServiceResolver GetResolver(Type serviceType)
    {
        Verify.Argument(typeof(IService).IsAssignableFrom(serviceType),
            $"Type {serviceType.Name} must implement {nameof(IService)}.");
        lock (_resolversLock)
        {
            Verify.Argument(_resolversTypes.TryGetValue(serviceType, out var resolver),
                $"Service type {serviceType.Name} is not declared.");
            return resolver!;
        }
    }

    public IServiceResolver GetResolver<TService>() where TService : class, IService => GetResolver(typeof(TService));

    public bool TryGetResolver(Type serviceType, out IServiceResolver? resolver)
    {
        lock (_resolversLock)
            return _resolversTypes.TryGetValue(serviceType, out resolver);
    }

    public bool TryGetResolver<TService>(out IServiceResolver? resolver) where TService : class, IService =>
        TryGetResolver(typeof(TService), out resolver);

    public void AddResolver(IServiceResolver resolver)
    {
        resolver.Provider.VerifyEquals(this,
            $"{nameof(resolver)} has wrong {nameof(resolver.Provider)}.");
        if (ApplicationSpace.StaticSpace.Provider != this && resolver.IsStatic)
        {
            ApplicationSpace.StaticSpace.Provider.AddResolver(resolver);
            return;
        }

        lock (_resolversLock)
        {
            Verify.Argument(_resolversTypes.TryAdd(resolver.ServiceType, resolver),
                $"Service {resolver.ServiceType.Name} is already declared.");
            _resolvers.Add(resolver);
        }
    }

    public bool HasScope(Type scopeType)
    {
        lock (_scopesLock)
            return _scopesTypes.ContainsKey(scopeType);
    }

    public bool HasScope<TScope>() where TScope : Scope, new() => HasScope(typeof(TScope));

    public Scope GetScope<TScope>() where TScope : Scope, new() => GetScope(typeof(TScope));

    public Scope GetScope(Type scopeType)
    {
        Verify.Argument(typeof(Scope).IsAssignableFrom(scopeType),
            $"Type {scopeType.Name} must implement {nameof(IService)}.");
        lock (_scopesLock)
        {
            Verify.Argument(_scopesTypes.TryGetValue(scopeType, out var scope),
                $"Scope {scopeType.Name} is not declared.");
            return scope!;
        }
    }

    public bool TryGetScope(Type scopeType, out Scope? scope)
    {
        lock (_scopesLock)
            return _scopesTypes.TryGetValue(scopeType, out scope);
    }

    public bool TryGetScope<TScope>(out Scope? scope) where TScope : Scope, new() =>
        TryGetScope(typeof(TScope), out scope);

    public void DeclareScope<TScope>() where TScope : Scope, new()
    {
        var type = typeof(TScope);
        lock (_scopesLock)
        {
            Verify.Argument(HasScope<TScope>(), $"Scope {type.Name} is already declared.");
            var scope = new TScope();
            _scopesTypes.TryAdd(type, scope);
            _scopes.Add(scope);
        }
    }
}