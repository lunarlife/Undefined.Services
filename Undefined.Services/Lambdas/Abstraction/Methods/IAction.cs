namespace Undefined.Services.Lambdas.Abstraction.Methods;

public interface IDelegate
{
    public ICompiledMethod TargetMethod { get; }
}

public interface IAction<T> : IDelegate where T : IParameterized
{
}

public interface IFunc : IDelegate
{
}

public interface IFuncParameterized : IFunc
{
}

public interface IFunc<TReturn> : IFunc
{
}

public interface IFunc<TReturn, TParameters> : IFunc<TReturn>, IFuncParameterized
    where TParameters : struct, IParametersDeclare<TParameters>, IParameterized
{
    public TReturn Invoke(TParameters parameters);
}