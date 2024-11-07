using System.Runtime.CompilerServices;
using Undefined.Services.Application.Services.Lookup.Runtime.Injection;

namespace Undefined.Services.Application.Services.Extensions;

public static class ServiceCollectionExtensions
{
    #region SINGLETONE

    public static IServiceCollection AddSingleton(this IServiceCollection services, Type serviceType,
        Type implementationType) =>
        services.AddDescriptor(new ServiceDescriptor(ServiceLifetime.Singleton, serviceType, implementationType));

    public static IServiceCollection AddSingleton(this IServiceCollection services, Type implementationType) =>
        services.AddDescriptor(new ServiceDescriptor(ServiceLifetime.Singleton, implementationType,
            implementationType));

    public static IServiceCollection AddSingleton(this IServiceCollection services, Type serviceType,
        IService implementation) =>
        services.AddDescriptor(new ServiceDescriptor(serviceType, implementation));

    public static IServiceCollection AddSingleton(this IServiceCollection services,
        IService implementation) =>
        services.AddDescriptor(new ServiceDescriptor(implementation));

    public static IServiceCollection AddSingleton(this IServiceCollection services, Type serviceType,
        Func<IServiceProvider, IService> implementationFactory) =>
        services.AddDescriptor(new ServiceDescriptor(ServiceLifetime.Singleton, serviceType, implementationFactory));
    
    public static IServiceCollection AddSingleton<TService>(this IServiceCollection services, Type implementationType)
        where TService : IService =>
        services.AddSingleton(typeof(TService), implementationType);

    public static IServiceCollection AddSingleton<TService>(this IServiceCollection services,
        TService implementation)
        where TService : IService =>
        services.AddSingleton(typeof(TService), implementation);

    public static IServiceCollection AddSingleton<TImplementation>(this IServiceCollection services)
        where TImplementation : IService, new() =>
        services.AddSingleton(typeof(TImplementation), typeof(TImplementation));

    public static IServiceCollection AddSingleton<TService, TImplementation>(this IServiceCollection services)
        where TService : IService
        where TImplementation : TService, new() =>
        services.AddSingleton(typeof(TService), typeof(TImplementation));

    public static IServiceCollection AddSingleton<TService>(this IServiceCollection services,
        Func<IServiceProvider, TService> implementationFactory)
        where TService : IService =>
        services.AddSingleton(typeof(TService), provider => implementationFactory(provider));

    #endregion


    #region SCOPED

    public static IServiceCollection AddScoped(this IServiceCollection services, Type serviceType,
        Type implementationType) =>
        services.AddDescriptor(new ServiceDescriptor(ServiceLifetime.Scoped, serviceType, implementationType));

    public static IServiceCollection AddScoped(this IServiceCollection services, Type implementationType) =>
        services.AddDescriptor(new ServiceDescriptor(ServiceLifetime.Scoped, implementationType,
            implementationType));
    
    public static IServiceCollection AddScoped(this IServiceCollection services, Type serviceType,
        Func<IServiceProvider, IService> implementationFactory) =>
        services.AddDescriptor(new ServiceDescriptor(ServiceLifetime.Scoped, serviceType, implementationFactory));


    public static IServiceCollection AddScoped<TService>(this IServiceCollection services, Type implementationType)
        where TService : IService =>
        services.AddScoped(typeof(TService), implementationType);
    
    public static IServiceCollection AddScoped<TImplementation>(this IServiceCollection services)
        where TImplementation : IService, new() =>
        services.AddScoped(typeof(TImplementation), typeof(TImplementation));

    public static IServiceCollection AddScoped<TService, TImplementation>(this IServiceCollection services)
        where TService : IService
        where TImplementation : TService, new() =>
        services.AddScoped(typeof(TService), typeof(TImplementation));
    
    public static IServiceCollection AddScoped<TService>(this IServiceCollection services,
        Func<IServiceProvider, TService> implementationFactory)
        where TService : IService =>
        services.AddScoped(typeof(TService), provider => implementationFactory(provider));

    #endregion

    
    #region TRANSIENT

    public static IServiceCollection AddTransient(this IServiceCollection services, Type serviceType,
        Type implementationType) =>
        services.AddDescriptor(new ServiceDescriptor(ServiceLifetime.Transient, serviceType, implementationType));

    public static IServiceCollection AddTransient(this IServiceCollection services, Type implementationType) =>
        services.AddDescriptor(new ServiceDescriptor(ServiceLifetime.Transient, implementationType,
            implementationType));

    public static IServiceCollection AddTransient(this IServiceCollection services, Type serviceType,
        Func<IServiceProvider, IService> implementationFactory) =>
        services.AddDescriptor(new ServiceDescriptor(ServiceLifetime.Transient, serviceType, implementationFactory));
    
    public static IServiceCollection AddTransient<TService>(this IServiceCollection services, Type implementationType)
        where TService : IService =>
        services.AddTransient(typeof(TService), implementationType);
    
    public static IServiceCollection AddTransient<TImplementation>(this IServiceCollection services)
        where TImplementation : IService, new() =>
        services.AddTransient(typeof(TImplementation), typeof(TImplementation));

    public static IServiceCollection AddTransient<TService, TImplementation>(this IServiceCollection services)
        where TService : IService
        where TImplementation : TService, new() =>
        services.AddTransient(typeof(TService), typeof(TImplementation));
    
    public static IServiceCollection AddTransient<TService>(this IServiceCollection services,
        Func<IServiceProvider, TService> implementationFactory)
        where TService : IService =>
        services.AddTransient(typeof(TService), provider => implementationFactory(provider));

    #endregion

    public static IServiceProvider BuildProvider(this IServiceCollection services) => new ServiceProvider(services, new List<ServiceInjectorBase>());
    public static IServiceProvider BuildProvider(this IServiceCollection services, ICollection<ServiceInjectorBase> injectors) => new ServiceProvider(services, injectors);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IServiceCollection AddDescriptor(this IServiceCollection services, IServiceDescriptor descriptor)
    {
        services.Add(descriptor);
        return services;
    }
}