namespace Undefined.Services.Lambdas.Abstraction.Methods;

public interface ICompiledMethod
{
    public IMethodParameterCollection Parameters { get; }
    public bool RequiresTarget { get; }
}

public interface ICompiledMethodReturnable : ICompiledMethod, IRuntimeReturnable
{
    
}
public interface ICompiledMethodReturnable<TResult> : ICompiledMethodReturnable, IRuntimeReturnable<TResult>
{
    public ICompiledMethodReturnType ReturnType { get; }
}


public interface ICompiledMethodParameterized<TParameters>
    : ICompiledMethod
{
}

public interface ICompiledMethodParameterized<TReturn, TParameters>
    : ICompiledMethodParameterized<TParameters>, IRuntimeReturnable<TReturn>
{
}

