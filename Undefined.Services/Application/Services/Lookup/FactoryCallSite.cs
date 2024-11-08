namespace Undefined.Services.Application.Services.Lookup;

internal class FactoryCallSite : ServiceCallSite
{
    public Func<IServiceProvider, IService> Factory { get; }


    public FactoryCallSite(CallSiteCache cache, Func<IServiceProvider, IService> factory) : base(cache)
    {
        Factory = factory;
    }

    public override Type? ImplementationType => ValueCached?.GetType();
}