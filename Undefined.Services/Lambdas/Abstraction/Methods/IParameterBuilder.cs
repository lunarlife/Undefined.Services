namespace Undefined.Services.Lambdas.Abstraction.Methods;

public interface IParameterBuilder<T>
{
    public Func<T> Builder { get; }
}