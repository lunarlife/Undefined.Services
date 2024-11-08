using System.Reflection;

namespace Undefined.Services.Application.Services.Lookup.Runtime.Dependencies;

public class ServiceInjectorCached
{
    private Action<IServiceProvider, IService>? _action;

    public Action<IServiceProvider, IService> Action =>
        _action ??= InjectDelegateAction;

    public MethodInfo Method { get; }
    public Type ImplementationType { get; }

    public ServiceInjectorCached(MethodInfo method, Type implementationType)
    {
        Method = method;
        ImplementationType = implementationType;
    }
    
    private void InjectDelegateAction(IServiceProvider provider, IService service)
    {
        if (!ImplementationType.IsInstanceOfType(service)) throw new InvalidOperationException();
        Method.Invoke(null, [provider, service]);
    }
}