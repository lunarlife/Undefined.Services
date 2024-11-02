using Undefined.Services.Application;

namespace Undefined.Services;

public class ServiceInitializationData
{
    public ApplicationSpace Owner { get; }
    public ServiceInstallFunc InstallFunc { get; }
    public ScopeOld Scope { get; }

    public ServiceInitializationData(ApplicationSpace owner, ServiceInstallFunc installFunc, ScopeOld scope)
    {
        Owner = owner;
        InstallFunc = installFunc;
        Scope = scope;
    }
}