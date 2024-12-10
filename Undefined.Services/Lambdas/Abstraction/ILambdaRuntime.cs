using Undefined.Services.Lambdas.Abstraction.Methods;
using Undefined.Services.Lambdas.Abstraction.Methods.Delegators;
using Undefined.Services.Lambdas.Abstraction.Presentations;

namespace Undefined.Services.Lambdas.Abstraction;

public static class Lambda
{
    public static Func<TReturn> Func<TReturn>(Func<TReturn> func) 
    {
        return new Func<TReturn>;
    }

    public static IAction<IpT> Action<T>(Action<T> valueFunc)
    {
    }
    public static IFunc<T> Query<T>(Func<T> queryFunc) 
}

public interface ILambdaRuntime : ILambdaCompiled
{
    public IUCollection<IRuntimeDeclarable> LambdaTree { get; }
}

public interface ILambdaCompiled
{
    public ICompiledMethod CompiledMethod { get; }
    public IAction DefaultDelegate { get; }
}