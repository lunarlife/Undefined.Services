namespace Undefined.Services;

public class ServiceInstanceData
{
    public ServicesSpace Owner { get; }
    public ServiceInstanceFunc InstanceFunc { get; }
    public Scope Scope { get; }
    public ServiceInstanceData(ServicesSpace owner, ServiceInstanceFunc instanceFunc, Scope scope)
    {
        Owner = owner;
        InstanceFunc = instanceFunc;
        Scope = scope;
    }
}