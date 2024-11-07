using Undefined.Verifying;

namespace Undefined.Services.Application.Services;

public class ServiceDescriptor : IServiceDescriptor
{
    public ServiceLifetime Lifetime { get; }
    public Type ServiceType { get; }
    public Type? ImplementationType { get; }
    public IService? Implementation { get; }

    public Func<IServiceProvider, IService>? ImplementationFactory { get; }
    public bool ImplementationIsServiceType => ServiceType == ImplementationType;

    public ServiceDescriptor(ServiceLifetime lifetime, Type serviceType,
        Func<IServiceProvider, IService> implementationFactory)
    {
        Lifetime = lifetime;
        ServiceType = serviceType;
        ImplementationFactory = implementationFactory;
    }

    public ServiceDescriptor(Type serviceType, IService implementation)
    {
        VerifyService(serviceType, implementation.GetType());
        Lifetime = ServiceLifetime.Singleton;
        ServiceType = serviceType;
        Implementation = implementation;
    }

    public ServiceDescriptor(IService implementation)
    {
        Lifetime = ServiceLifetime.Singleton;
        ServiceType = implementation.GetType();
        Implementation =  implementation;
    }


    public ServiceDescriptor(ServiceLifetime lifetime, Type serviceType, Type implementationType)
    {
        VerifyService(serviceType, implementationType);
        Lifetime = lifetime;
        ServiceType = serviceType;
        ImplementationType = implementationType;
    }

    public ServiceDescriptor(ServiceLifetime lifetime, Type implementationType)
    {
        VerifyService(implementationType, implementationType);
        Lifetime = lifetime;
        ServiceType = implementationType;
        ImplementationType = implementationType;
    }

    private static void VerifyService(Type serviceType)
    {
        Verify.Argument(typeof(IService).IsAssignableFrom(serviceType),
            $"{serviceType} must implement {nameof(IService)}");
    }

    private static void VerifyService(Type serviceType, Type implementationType)
    {
        VerifyService(serviceType);
        Verify.Argument(implementationType is { IsClass: true, IsAbstract: false },
            $"{implementationType} must be a non-abstract class.");
        if (serviceType != implementationType)
            Verify.Argument(serviceType.IsAssignableFrom(implementationType),
                $"{implementationType} must be assignable to {serviceType}.");
    }
}