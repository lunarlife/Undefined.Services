using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace Undefined.Services.Application.Services.Lookup.Runtime.Factory;

internal class ServiceRuntimeResolverBuilder : IServiceRuntimeResolverBuilder
{
    private static readonly MethodInfo _depencenciesResolverInvokeMethodInfo =
        typeof(Action<CoreScope, IService>).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public)!;

    public static IServiceRuntimeResolverBuilder Instance { get; } = new ServiceRuntimeResolverBuilder();

    private readonly ConcurrentDictionary<Type, RuntimeServiceFactory> _cachedFactories = [];
    private readonly object _lock = new();

    private ServiceRuntimeResolverBuilder()
    {
    }

    public Func<CoreScope, IService> GetCachedOrCreateFactoryFunction(ServiceCallSite callSite,
        Func<CoreScope, IService> factory, Func<Type, Action<CoreScope, IService>>? dependenciesResolverFunc)
    {
        /*
        return scope =>
        {
            var service = factory(scope);
            if (callSite.IsDisposable) scope.AddDisposable(service);
            InjectService(callSite.Cache.ServiceId, service.GetType())(scope, service);
            return service;
        };*/

        var cache = callSite.Cache;
        var method = new DynamicMethod($"service_factory_{callSite.ServiceType.Name}", typeof(IService),
            [typeof(CoreScope)]);
        var generator = method.GetILGenerator();

        generator.Emit(OpCodes.Ldarg_0);
        generator.Emit(OpCodes.Callvirt, factory.Method);

        if (dependenciesResolverFunc != null) EmitDependenciesResolver(generator, callSite, dependenciesResolverFunc);


        generator.Emit(OpCodes.Ret);
        return (Func<CoreScope, IService>)method.CreateDelegate(typeof(Func<CoreScope, IService>));
    }

    private static void EmitDependenciesResolver(ILGenerator generator, ServiceCallSite callSite,
        Func<Type, Action<CoreScope, IService>> dependenciesResolverFunc)
    {
        var varService = generator.DeclareLocal(typeof(IService));
        generator.Emit(OpCodes.Stloc, varService);
        if (callSite.HasImplementationType)
        {
            var resolver = dependenciesResolverFunc(callSite.ImplementationType!);

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldloc, varService);
            generator.Emit(OpCodes.Call, resolver.Method);
        }
        else
        {
            generator.Emit(OpCodes.Ldloc, varService);
            generator.Emit(OpCodes.Refanytype);
            generator.Emit(OpCodes.Call, dependenciesResolverFunc.Method); //   Get dependencies resolver


            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldloc, varService);
            generator.Emit(OpCodes.Call, _depencenciesResolverInvokeMethodInfo); //    Call dependencies resolver
        }

        generator.Emit(OpCodes.Ldloc, varService);
    }

    public Func<CoreScope, IService> GetCachedOrCreateFactoryFunction(Type serviceType, Type implementationType,
        Action<CoreScope, IService>? injectFunc)
    {
        var factory =
            _cachedFactories.GetOrAdd(serviceType, type => CreateFactory(type, implementationType, injectFunc));
        return factory.Resolver;
    }

    public Func<CoreScope, IService> CreateFactoryFunctionNoCache(Type serviceType, Type implementationType,
        Action<CoreScope, IService>? injectFunc) => throw new NotImplementedException();

    public Func<CoreScope, IService> CreateFactoryFunctionCached(Type serviceType, Type implementationType,
        Action<CoreScope, IService>? injectFunc) => throw new NotImplementedException();

    private RuntimeServiceFactory CreateFactory(Type serviceType, Type implementationType,
        Action<CoreScope, IService>? injectFunc)
    {
        var method = new DynamicMethod($"service_factory_{serviceType.Name}", typeof(IService), [typeof(CoreScope)]);
        var generator = method.GetILGenerator();

        //instantiate a implementation type and flush all services and filters
        return new RuntimeServiceFactory
        {
            Resolver = (Func<CoreScope, IService>)method.CreateDelegate(typeof(Func<CoreScope, IService>))
        };
    }

    private class RuntimeServiceFactory
    {
        public Func<CoreScope, IService> Resolver;
    }
}