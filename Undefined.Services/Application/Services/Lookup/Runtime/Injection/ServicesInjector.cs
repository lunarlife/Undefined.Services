using System.Reflection;
using Undefined.Services.Application.Services.Extensions;

namespace Undefined.Services.Application.Services.Lookup.Runtime.Injection;

public class ServicesInjector : ServiceInjector<IService>
{
    public override ResolveFunc<TField>? GetFieldResolveFunction<TField>(FieldInfo field)
    {
        if (!field.IsInitOnly) return null;
        return (provider, _) => (TField)provider.GetService(typeof(TField));
    }
}