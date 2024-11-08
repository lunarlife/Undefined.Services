using Undefined.Services.Application.Services.Extensions;
using Undefined.Services.Application.Services.Lookup;
using Undefined.Services.Application.Services.Lookup.Runtime;
using Undefined.Services.Application.Services.Lookup.Runtime.Factory;
using Undefined.Services.Exceptions;
using Undefined.Verifying;

namespace Undefined.Services.Application.Services;

internal class ServiceResolveMachine : IServiceResolveMachine
{
    private readonly IServiceDescriptor[] _descriptors;
    private readonly Dictionary<ServiceId, ServiceCallSite> _serviceCallSites = [];

    public ServiceResolveMachine(ICollection<IServiceDescriptor> descriptors)
    {
        _descriptors = new IServiceDescriptor[descriptors.Count];
        descriptors.CopyTo(_descriptors, 0);
    }

    public ServiceCallSite CreateCallSite(ServiceId serviceId)
    {
        foreach (var descriptor in _descriptors)
        {
            if (descriptor.ServiceType != serviceId.ServiceType)
                continue;
            return CreateCallSite(descriptor);
        }

        throw new ServiceResolveException($"Service descriptor for service {serviceId} not found.");
    }

    public ServiceCallSite GetCachedOrCreateCallSite(ServiceId serviceId)
    {
        if (!_serviceCallSites.TryGetValue(serviceId, out var callSite))
            _serviceCallSites.Add(serviceId, callSite = CreateCallSite(serviceId));
        return callSite;
    }

    private ServiceCallSite CreateCallSite(IServiceDescriptor descriptor)
    {
        var serviceType = descriptor.ServiceType;
        ServiceCallSite? callSite = null;
        if (descriptor.HasImplementation())
        {
            descriptor.Lifetime.VerifyEquals(ServiceLifetime.Singleton,
                $"{nameof(ServiceDescriptor)} with implementation should be singleton.");
            callSite = new ValueCallSite(serviceType, descriptor.Implementation!);
        }
        else if (descriptor.HasFactory())
        {
            callSite = new FactoryCallSite(new CallSiteCache(descriptor.Lifetime, new ServiceId(serviceType)),
                descriptor.ImplementationFactory!);
        }
        else if (descriptor.HasImplementationType())
        {
            callSite = new TypeCallSite(descriptor.Lifetime, descriptor.ServiceType, descriptor.ImplementationType!);
        }

        if (callSite is null) throw new InvalidServiceDescriptorException();
        return callSite;
    }
}