namespace Undefined.Services.Lambdas.Abstraction.Methods;

public interface IParameterBuilder<TParameters>
{
    public Func<TParameters> Builder { get; }
}