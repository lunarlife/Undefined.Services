using Undefined.Services.Application.Services.Scopes;

namespace Undefined.Services.Application.Services;

public interface IServiceProvider
{
    public IServiceScope CreateScope();

    public T GetService<T>() where T : IService;
    public IService GetService(Type serviceType);
}