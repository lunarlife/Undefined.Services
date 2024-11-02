namespace Undefined.Services.Scopes;

public class SingletonScope : Scope
{
    private readonly Dictionary<Type, IService> _services = [];

    protected override IService ResolveService(Type serviceType)
    {
        if (!_services.TryGetValue(serviceType, out var service))
        {
            service = Space.Installer.GetOrGenerateInstallFunc(serviceType)(this);
            _services.Add(serviceType, service);
        }

        return service;
    }
}