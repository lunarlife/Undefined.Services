namespace Undefined.Services.Application.Services;

public interface IServiceDescriptor
{
    public ServiceLifetime Lifetime { get; }
    public Type ServiceType { get; }
    public Type? ImplementationType { get; }
    public IService? Implementation { get; }
    public Func<IServiceProvider, IService>? ImplementationFactory { get; }
    public bool ImplementationIsServiceType { get; }
}