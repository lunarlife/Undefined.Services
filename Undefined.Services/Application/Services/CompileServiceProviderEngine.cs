using Undefined.Services.Application.Services.Lookup;
using Undefined.Services.Application.Services.Lookup.Runtime.Factory;

namespace Undefined.Services.Application.Services;

internal class CompileServiceProviderEngine : ServiceProviderEngine
{
    public override Func<CoreScope, IService> RealizeService(ServiceCallSite callsite)
    {
        if (callsite is FactoryCallSite factoryCallSite)
        {
            
        }
        else if (callsite is TypeCallSite typeCallSite) 
            return ServiceRuntimeFactoryGenerator.Instance.GetCachedOrCreateFactoryFunction(typeCallSite.ServiceType, typeCallSite.ImplementationType);

        throw new InvalidOperationException();
    }

    public override Action<CoreScope, IService> InjectService(ServiceCallSite callsite)
    {
        
    }
}