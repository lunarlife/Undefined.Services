namespace Undefined.Services.Application.Services.Lookup;

internal class TypeCallSite : ServiceCallSite
{
    public TypeCallSite(ServiceLifetime lifetime, Type serviceType, Type implementationType) : base(new CallSiteCache(lifetime, new ServiceId(serviceType)))
    {
        ImplementationType = implementationType;
    }

    public override Type ImplementationType { get; }
}