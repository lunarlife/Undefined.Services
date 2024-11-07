namespace Undefined.Services.Application.Services.Lookup;

internal class ValueCallSite : ServiceCallSite
{
    public IService Service { get; }
    
    public override Type ImplementationType => Service.GetType();

    public ValueCallSite(Type serviceType, IService service) : base(CallSiteCache.Nowhere(serviceType))
    {
        Service = service;
    }
}