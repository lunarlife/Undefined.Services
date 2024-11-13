namespace Undefined.Services.Application.Services.Lookup.Runtime.Dependencies.Methods;

[Flags]
public enum ServiceRuntimeMethodAutoInvokeOptions
{
    None,
    Resolve,
    Dispose,
    GetService,
    
}