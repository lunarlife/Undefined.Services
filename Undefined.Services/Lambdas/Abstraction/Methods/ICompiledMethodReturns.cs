namespace Undefined.Services.Lambdas.Abstraction.Methods;

public interface ICompiledMethod
{
    public IMethodParameterCollection Parameters { get; }
    public bool RequiresTarget { get; }
}

public interface ICompiledMethodReturns<TResult> : ICompiledMethod
{
    public ICompiledMethodReturnType ReturnType { get; }
}

public interface ICompiledMethodParameterizedReturns<TReturn, TParameters>
    : ICompiledMethodReturns<TReturn>
{
    public ICompiledMethodReturnType ReturnType { get; }
}