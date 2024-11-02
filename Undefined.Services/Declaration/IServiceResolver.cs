using Undefined.Services.Scopes;

namespace Undefined.Services.Declaration;

public interface IServiceResolver
{
    public Type ServiceType { get; }
    public Type ImplementationType { get; }
    public IApplicationProvider Provider { get; }
    public bool IsStatic { get; }
    public Scope Scope { get; }
    public IService Resolve();
}