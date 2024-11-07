using Undefined.Services.Application.Services.Scopes;

namespace Undefined.Services.Application.Services;

public interface IServiceProvider
{
    public IServiceScope CreateScope();
    public bool TryGetService(Type serviceType, out IService? service);
}