namespace Undefined.Services.Application.Services.Lookup.Runtime.Dependencies.Methods;

public interface IServiceRuntimeMethod
{
    public Type TargetType { get; }
    public Delegate Method { get; }
    
}