namespace Undefined.Services.Application.Services.Lookup.Runtime.Factory;

internal interface IServiceRuntimeResolverBuilder
{
    
    public Func<CoreScope, IService> GetCachedOrCreateFactoryFunction(ServiceCallSite callSite,
        Func<CoreScope, IService> factory, Func<Type, Action<CoreScope, IService>>? dependenciesResolverFunc);

    public Func<CoreScope, IService> GetCachedOrCreateFactoryFunction(Type serviceType, Type implementationType,
        Action<CoreScope, IService>? injectFunc);

    public Func<CoreScope, IService> CreateFactoryFunctionNoCache(Type serviceType, Type implementationType,
        Action<CoreScope, IService>? injectFunc);
}