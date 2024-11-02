using Undefined.Services.Application.Services.Scopes;

namespace Undefined.Services.Application.Services;

public class ServiceHelper
{
    public static Func<IServiceScope, IService> CreateServiceFactory(Type implementationType)
    {
        
    }
}