namespace Undefined.Services.Application.Services.Lookup;

internal class ValueCallSite : ServiceCallSite
{
    public IService Value { get; }
    
    public override Type ImplementationType => Value.GetType();

    public ValueCallSite(Type serviceType, IService service) : base(CallSiteCache.Nowhere(serviceType))
    {
        Value = service;
        ValueCached = service;
    }
}