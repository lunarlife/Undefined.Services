using System.Reflection;
using System.Runtime.CompilerServices;

namespace Undefined.Services.Application.Services.Lookup.Runtime.Dependencies.Methods;

public static class ServiceMethodBuilderExtensions
{
    public static IServiceRuntimeMethodBuilder Attribute<TAttribute>(this IServiceRuntimeMethodBuilder builder,
        Func<TAttribute, bool> predicate)
        where TAttribute : Attribute =>
        builder.Attribute(e => e.Any(a => a.GetType() == typeof(TAttribute) && predicate((TAttribute)a)));

    public static IServiceRuntimeMethodBuilder Attribute<TAttribute>(this IServiceRuntimeMethodBuilder builder)
        where TAttribute : Attribute =>
        builder.Attribute(typeof(TAttribute));

    public static IServiceRuntimeMethodBuilder
        Attribute(this IServiceRuntimeMethodBuilder builder, Type attributeType) =>
        builder.Attribute(e => e.Any(a => a.GetType() == attributeType));

    public static IServiceRuntimeMethodBuilder AttributesAll(this IServiceRuntimeMethodBuilder builder,
        IEnumerable<Type>
            attributesTypes) =>
        builder.Attribute(attributes => attributes.All(a => attributesTypes.Any(b => a.GetType() == b)));

    public static IServiceRuntimeMethodBuilder AttributeAssignableTo<TAttribute>(
        this IServiceRuntimeMethodBuilder builder)
        where TAttribute : Attribute =>
        builder.Attribute(e => e.Any(a => a is TAttribute));

    public static IServiceRuntimeMethodBuilder AttributeAssignableTo<TAttribute>(
        this IServiceRuntimeMethodBuilder builder,
        Func<TAttribute, bool> predicate)
        where TAttribute : Attribute =>
        builder.Attribute(e => e.Any(a => a is TAttribute attribute && predicate(attribute)));

    public static IServiceRuntimeMethodBuilder Parameters<T>(this IServiceRuntimeMethodBuilder builder) =>
        builder.Parameters(typeof(T));

    public static IServiceRuntimeMethodBuilder Parameters<T, T1>(this IServiceRuntimeMethodBuilder builder) =>
        builder.Parameters(typeof(T), typeof(T1));

    public static IServiceRuntimeMethodBuilder Parameters<T, T1, T2>(this IServiceRuntimeMethodBuilder builder) =>
        builder.Parameters(typeof(T), typeof(T1), typeof(T2));

    public static IServiceRuntimeMethodBuilder Parameters<T, T1, T2, T3>(this IServiceRuntimeMethodBuilder builder) =>
        builder.Parameters(typeof(T), typeof(T1), typeof(T2), typeof(T3));

    public static IServiceRuntimeMethodBuilder
        ParametersAreAssignableFrom<T>(this IServiceRuntimeMethodBuilder builder) =>
        builder.ParametersAreAssignableFrom(typeof(T));

    public static IServiceRuntimeMethodBuilder
        Parameters(this IServiceRuntimeMethodBuilder builder, params Type[] parameterTypes) =>
        builder.Parameters(parameterTypes.AsEnumerable());

    public static IServiceRuntimeMethodBuilder
        Parameters(this IServiceRuntimeMethodBuilder builder, IEnumerable<Type> parameterTypes) =>
        builder.Parameters(parameters =>
            parameterTypes.SequenceEqual(parameters.Select(p => p.ParameterType)));

    public static IServiceRuntimeMethodBuilder ParametersAreAssignableFrom<T, T1>(
        this IServiceRuntimeMethodBuilder builder) =>
        builder.ParametersAreAssignableFrom(typeof(T), typeof(T1));

    public static IServiceRuntimeMethodBuilder ParametersAreAssignableFrom<T, T1, T2>(
        this IServiceRuntimeMethodBuilder builder) =>
        builder.ParametersAreAssignableFrom(typeof(T), typeof(T1), typeof(T2));

    public static IServiceRuntimeMethodBuilder
        ParametersAreAssignableFrom<T, T1, T2, T3>(this IServiceRuntimeMethodBuilder builder) =>
        builder.ParametersAreAssignableFrom(typeof(T), typeof(T1), typeof(T2), typeof(T3));

    public static IServiceRuntimeMethodBuilder ParametersAreAssignableFrom(this IServiceRuntimeMethodBuilder builder,
        params Type[] parameterTypes) =>
        builder.Parameters(types =>
            parameterTypes.SequenceEqual(types.Select(p => p.ParameterType), new AssignableToEqualityComparer()));

    public static IServiceRuntimeMethodBuilder ParametersAreAssignableFrom(this IServiceRuntimeMethodBuilder builder,
        IEnumerable<Type> parameterTypes) =>
        builder.Parameters(types => parameterTypes.SequenceEqual(types, new AssignableToEqualityComparer()));

    public static IServiceRuntimeMethodBuilder Name(this IServiceRuntimeMethodBuilder builder, string name) =>
        builder.Name(n => n == name);

    public static IServiceRuntimeMethodBuilder
        NameStartsWith(this IServiceRuntimeMethodBuilder builder, string value) =>
        builder.Name(name => name.StartsWith(value));

    public static IServiceRuntimeMethodBuilder NameEndsWith(this IServiceRuntimeMethodBuilder builder, string value) =>
        builder.Name(name => name.EndsWith(value));

    public static IServiceRuntimeMethodBuilder GenericArguments<T>(this IServiceRuntimeMethodBuilder builder) =>
        builder.GenericArguments(typeof(T));

    public static IServiceRuntimeMethodBuilder GenericArguments<T, T1>(this IServiceRuntimeMethodBuilder builder) =>
        builder.GenericArguments(typeof(T), typeof(T1));

    public static IServiceRuntimeMethodBuilder GenericArguments<T, T1, T2>(this IServiceRuntimeMethodBuilder builder) =>
        builder.GenericArguments(typeof(T), typeof(T1), typeof(T2));

    public static IServiceRuntimeMethodBuilder GenericArguments<T, T1, T2, T3>(
        this IServiceRuntimeMethodBuilder builder) =>
        builder.GenericArguments(typeof(T), typeof(T1), typeof(T2), typeof(T3));


    public static IServiceRuntimeMethodBuilder
        GenericArguments(this IServiceRuntimeMethodBuilder builder, params Type[] genericArgumentTypes) =>
        builder.GenericArguments(genericArgumentTypes.AsEnumerable());

    public static IServiceRuntimeMethodBuilder
        GenericArguments(this IServiceRuntimeMethodBuilder builder, IEnumerable<Type> genericArgumentTypes) =>
        builder.GenericArguments(genericArgumentTypes.SequenceEqual);


    public static IServiceRuntimeMethodBuilder GenericArgumentsAreAssignableFrom<T>(
        this IServiceRuntimeMethodBuilder builder) =>
        builder.GenericArgumentsAreAssignableFrom(typeof(T));

    public static IServiceRuntimeMethodBuilder GenericArgumentsAreAssignableFrom<T, T1>(
        this IServiceRuntimeMethodBuilder builder) =>
        builder.GenericArgumentsAreAssignableFrom(typeof(T), typeof(T1));

    public static IServiceRuntimeMethodBuilder
        GenericArgumentsAreAssignableFrom<T, T1, T2>(this IServiceRuntimeMethodBuilder builder) =>
        builder.GenericArgumentsAreAssignableFrom(typeof(T), typeof(T1), typeof(T2));

    public static IServiceRuntimeMethodBuilder GenericArgumentsAreAssignableFrom<T, T1, T2, T3>(
        this IServiceRuntimeMethodBuilder builder) =>
        builder.GenericArgumentsAreAssignableFrom(typeof(T), typeof(T1), typeof(T2), typeof(T3));


    public static IServiceRuntimeMethodBuilder GenericArgumentsAreAssignableFrom(
        this IServiceRuntimeMethodBuilder builder,
        params Type[] genericArgumentTypes) =>
        builder.GenericArguments(types =>
            genericArgumentTypes.SequenceEqual(types, new AssignableToEqualityComparer()));

    public static IServiceRuntimeMethodBuilder GenericArgumentsAreAssignableFrom(
        this IServiceRuntimeMethodBuilder builder,
        IEnumerable<Type> genericArgumentTypes) =>
        builder.GenericArguments(types =>
            genericArgumentTypes.SequenceEqual(types, new AssignableToEqualityComparer()));


    public static IServiceRuntimeMethodBuilder Returns<T>(this IServiceRuntimeMethodBuilder builder) =>
        builder.Returns(typeof(T));

    public static IServiceRuntimeMethodBuilder Returns(this IServiceRuntimeMethodBuilder builder, Type type) =>
        builder.Returns(t => t == type);

    public static IServiceRuntimeMethodBuilder ReturnsAssignableTo<T>(this IServiceRuntimeMethodBuilder builder) =>
        builder.Returns(t => typeof(T).IsAssignableFrom(t));

    public static IServiceRuntimeMethodBuilder Private(this IServiceRuntimeMethodBuilder builder) =>
        builder.AccessModifier(MemberAccessModifiers.Private);

    public static IServiceRuntimeMethodBuilder Protected(this IServiceRuntimeMethodBuilder builder) =>
        builder.AccessModifier(MemberAccessModifiers.Protected);

    public static IServiceRuntimeMethodBuilder Public(this IServiceRuntimeMethodBuilder builder) =>
        builder.AccessModifier(MemberAccessModifiers.Public);

    public static IServiceRuntimeMethodBuilder Internal(this IServiceRuntimeMethodBuilder builder) =>
        builder.AccessModifier(MemberAccessModifiers.Internal);

    public static IServiceRuntimeMethodBuilder As<TDelegate>(this IServiceRuntimeMethodBuilder builder)
        where TDelegate : Delegate
    {
        var delegateType = typeof(TDelegate);
        var method = delegateType.GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public)!;
        builder.Returns(method.ReturnType)
            .AttributesAll(method.GetCustomAttributes().Where(t => t is not CompilerGeneratedAttribute)
                .Select(a => a.GetType()))
            .Parameters(method.GetParameters().Select(p => p.ParameterType)).AccessModifier(MemberAccessModifiers.All);
        return builder;
    }

    public static IServiceRuntimeMethodBuilder InvokeOnResolve(this IServiceRuntimeMethodBuilder builder) =>
        builder.AutoInvoke(ServiceRuntimeMethodAutoInvokeOptions.Resolve);

    public static IServiceRuntimeMethodBuilder InvokeOnDispose(this IServiceRuntimeMethodBuilder builder) =>
        builder.AutoInvoke(ServiceRuntimeMethodAutoInvokeOptions.Dispose);

    public static IServiceRuntimeMethodBuilder InvokeOnDispose(this IServiceRuntimeMethodBuilder builder) =>
        builder.AutoInvoke(ServiceRuntimeMethodAutoInvokeOptions.Dispose);

    private class AssignableToEqualityComparer : EqualityComparer<Type>
    {
        public override bool Equals(Type? x, Type? y) => x != null && y != null && y.IsAssignableFrom(x);

        public override int GetHashCode(Type obj) => obj.GetHashCode();
    }
}