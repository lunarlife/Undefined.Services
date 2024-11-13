using System.Reflection;

namespace Undefined.Services.Application.Services.Lookup.Runtime.Dependencies.Methods;

public interface IServiceRuntimeMethodBuilder
{
    public IServiceRuntimeMethodBuilder Attribute(Func<IEnumerable<Attribute>, bool> predicate);

    public IServiceRuntimeMethodBuilder GenericArguments(Func<IEnumerable<Type>, bool> predicate);
    public IServiceRuntimeMethodBuilder NoGenericArguments();
    public IServiceRuntimeMethodBuilder AccessModifier(MemberAccessModifiers modifiers);
    public IServiceRuntimeMethodBuilder Parameters(Func<IEnumerable<ParameterInfo>, bool> predicate);
    public IServiceRuntimeMethodBuilder NoParameters();
    public IServiceRuntimeMethodBuilder Name(Func<string, bool> predicate);
    public IServiceRuntimeMethodBuilder Returns(Func<ParameterInfo, bool> predicate);
    public IServiceRuntimeMethodBuilder NoReturnValue();
    public IServiceRuntimeMethodBuilder DeclaredOnly();
    public IServiceRuntimeMethodBuilder Static();
    public IServiceRuntimeMethodBuilder AutoInvoke(ServiceRuntimeMethodAutoInvokeOptions options);

    public void Build();
}