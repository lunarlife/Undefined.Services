namespace Undefined.Services.Lambdas.Abstraction.Methods;

public interface IRuntimeReturnable
{
}

public interface IRuntimeReturnable<TReturn> : IRuntimeReturnable
{
}

public interface IRuntimeReturnableParameterized<TReturn, TParameters> : IRuntimeReturnable<TReturn>
{
    public IParameterBuilder<TParameters> GetParameterBuilder();
}