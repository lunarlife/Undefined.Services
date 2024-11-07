namespace Undefined.Services.Application.Services.Lookup;

internal interface IServiceResolveMachine
{
    public ServiceCallSite CreateCallSite(ServiceId serviceId);
    public ServiceCallSite GetCachedOrCreateCallSite(ServiceId serviceId);
}