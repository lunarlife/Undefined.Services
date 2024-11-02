namespace Undefined.Services.Scopes;

public class FactoryScope : Scope
{
    protected override IService ResolveService(Type serviceType) => Space.Installer.GetOrGenerateInstallFunc(serviceType)(this);
}