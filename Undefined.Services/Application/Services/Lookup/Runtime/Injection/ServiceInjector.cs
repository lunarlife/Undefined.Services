using System.Reflection;

namespace Undefined.Services.Application.Services.Lookup.Runtime.Injection;

public delegate TField ResolveFunc<TField>(IServiceProvider provider, IService service);

public abstract class ServiceInjectorBase
{
    public abstract Type? BaseInjectionType { get; }

    internal ServiceInjectorBase()
    {
    }
}

public abstract class ServiceInjector : ServiceInjectorBase
{
    internal const string GET_RESOLVE_FUNCTION_NAME = nameof(GetFieldResolveFunction);

    public override Type? BaseInjectionType => null;

    public abstract ResolveFunc<TField>? GetFieldResolveFunction<TField>(FieldInfo field);
}

public abstract class ServiceInjector<TBase> : ServiceInjectorBase
{
    public sealed override Type BaseInjectionType { get; } = typeof(TBase);
    public abstract ResolveFunc<TField>? GetFieldResolveFunction<TField>(FieldInfo field) where TField : TBase;
}