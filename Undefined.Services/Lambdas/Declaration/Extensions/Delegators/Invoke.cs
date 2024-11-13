using Undefined.Services.Lambdas.Abstraction.Methods;

namespace Undefined.Services.Lambdas.Declaration.Extensions.Delegators;

public static class DelegateExtensions
{
    public static TReturn Invoke<TReturn, TParametersDeclare>(this IFuncParameterized func,
        TParametersDeclare parameters)
        where TParametersDeclare : struct, IParametersDeclare<TParametersDeclare>, IParameterized =>
        ((IFunc<TReturn, TParametersDeclare>)func).Invoke(parameters);

    public static TReturn Invoke<TReturn, TParametersDeclare>(this IFuncParameterized func,
        TParametersDeclare parameters)
        where TParametersDeclare : struct, IParametersDeclare<TParametersDeclare>, IParameterized =>
        ((IFunc<TReturn, TParametersDeclare>)func).Invoke(parameters);
}