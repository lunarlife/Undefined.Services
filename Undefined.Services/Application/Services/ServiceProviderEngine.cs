using Undefined.Services.Application.Services.Lookup;

namespace Undefined.Services.Application.Services;

internal abstract class ServiceProviderEngine
{
    public abstract Func<CoreScope, IService> RealizeService(ServiceCallSite callsite);
}