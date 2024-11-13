using System.Reflection;
using System.Runtime.CompilerServices;

namespace Undefined.Services.Application.Services.Lookup.Runtime.Dependencies.Methods;

public class ServiceRuntimeMethodBuilder : IServiceRuntimeMethodBuilder
{
    private readonly Type _implementationType;

    private Func<Type?, bool>? _returnsPredicate;
    private Func<string, bool>? _namePredicate;
    private Func<IEnumerable<Type>, bool> _parametersPredicate;
    private Func<IEnumerable<Type>, bool> _genericArgumentsPredicate;
    private Func<IEnumerable<Attribute>, bool>? _attributePredicate;

    private bool _declaredOnly;
    private bool _static;

    private MemberAccessModifiers _accessModifiers = MemberAccessModifiers.None;

    public ServiceRuntimeMethodBuilder(Type implementationType)
    {
        _implementationType = implementationType;
    }

    public IServiceRuntimeMethodBuilder Attribute(Func<IEnumerable<Attribute>, bool> predicate)
    {
        _attributePredicate = predicate;
        return this;
    }

    public IServiceRuntimeMethodBuilder GenericArguments(Func<IEnumerable<Type>, bool> predicate)
    {
        _genericArgumentsPredicate = predicate;
        return this;
    }

    public IServiceRuntimeMethodBuilder NoGenericArguments()
    {
        _genericArgumentsPredicate = EnumerableExtensions.IsEmpty;
        return this;
    }

    public IServiceRuntimeMethodBuilder AccessModifier(MemberAccessModifiers modifier)
    {
        _accessModifiers |= modifier;
        return this;
    }

    public IServiceRuntimeMethodBuilder NoParameters()
    {
        _parametersPredicate = EnumerableExtensions.IsEmpty;
        return this;
    }


    public IServiceRuntimeMethodBuilder Parameters(Func<IEnumerable<Type>, bool> predicate)
    {
        _parametersPredicate = predicate;
        return this;
    }

    public IServiceRuntimeMethodBuilder Name(Func<string, bool> predicate)
    {
        _namePredicate = predicate;
        return this;
    }

    public IServiceRuntimeMethodBuilder Returns(Func<Type, bool> predicate)
    {
        _returnsPredicate = type => type is not null && predicate(type);
        return this;
    }

    public IServiceRuntimeMethodBuilder NoReturnValue()
    {
        _returnsPredicate = type => type is null;
        return this;
    }

    public IServiceRuntimeMethodBuilder DeclaredOnly()
    {
        _declaredOnly = true;
        return this;
    }

    public IServiceRuntimeMethodBuilder Static()
    {
        _static = true;
        return this;
    }

    public void Build()
    {
        var flags = BindingFlags.Default;

        if (_declaredOnly)
            flags |= BindingFlags.DeclaredOnly;

        if (_static)
            flags |= BindingFlags.Static;
        else
            flags |= BindingFlags.Instance;

        if (_accessModifiers == MemberAccessModifiers.None)
            flags |= BindingFlags.Public;
        else
        {
            if ((_accessModifiers & MemberAccessModifiers.Public) != 0)
                flags |= BindingFlags.Public;
            if ((_accessModifiers & MemberAccessModifiers.NonPublic) != 0)
                flags |= BindingFlags.NonPublic;
        }


        var methods = _implementationType.GetMethods(flags);
        foreach (var method in methods)
        {
            if (!_declaredOnly && method.DeclaringType == typeof(object)) continue;
            if (method.GetCustomAttribute<CompilerGeneratedAttribute>() is not null) continue;
            if (_namePredicate is not null && !_namePredicate.Invoke(method.Name)) continue;
            if (_attributePredicate is not null && !_attributePredicate.Invoke(method.GetCustomAttributes())) continue;
            if (_parametersPredicate is not null &&
                !_parametersPredicate.Invoke(method.GetParameters().Select(p => p.ParameterType))) continue;
            if (_returnsPredicate is not null &&
                !_returnsPredicate.Invoke(method.ReturnType == typeof(void) ? null : method.ReturnType)) continue;
            if(_genericArgumentsPredicate is not null && !_genericArgumentsPredicate.Invoke(method.GetGenericArguments())) continue;
            
            //BUILD THE INVOKE METHOD
        }
    }
}