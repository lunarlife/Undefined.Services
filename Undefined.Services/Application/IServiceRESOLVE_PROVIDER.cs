namespace Undefined.Services.Application;

public interface IServiceRESOLVE_PROVIDER
{
    public T Resolve<T>() where T : IService;
    public IService Resolve(Type serviceType);
}