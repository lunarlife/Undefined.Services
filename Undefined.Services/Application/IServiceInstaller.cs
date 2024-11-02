using Undefined.Services.Scopes;

namespace Undefined.Services.Application;

public delegate IService ServiceInstallFunc(Scope scope);

public interface IServiceInstaller
{
    public ServiceInstallFunc GenerateInstallFunc(Type serviceType);
    public ServiceInstallFunc GetOrGenerateInstallFunc(Type serviceType);
    public void HasGeneratedInstallFunc(Type serviceType);
    public bool TryGetGeneratedInstallFunc(Type serviceType, out ServiceInstallFunc installFunc);
}