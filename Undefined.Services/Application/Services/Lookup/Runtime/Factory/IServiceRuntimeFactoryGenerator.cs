namespace Undefined.Services.Application.Services.Lookup.Runtime.Factory;

internal interface IServiceRuntimeFactoryGenerator
{
    public Func<IServiceProvider, IService> GetCachedOrCreateFactoryFunction(Type serviceType, Type implementationType, Action<CoreScope, IService>? injectFunc);
    public Func<IServiceProvider, IService> CreateFactoryFunctionNoCache(Type serviceType, Type implementationType, Action<CoreScope, IService>? injectFunc);
    public Func<IServiceProvider, IService> CreateFactoryFunctionCached(Type serviceType, Type implementationType, Action<CoreScope, IService>? injectFunc);
}