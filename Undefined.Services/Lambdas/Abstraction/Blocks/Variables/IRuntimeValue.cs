namespace Undefined.Services.Lambdas.Abstraction.Handles;

public interface IRuntimeValue
{
    public IRuntimeType Type { get; }
}

public interface IRuntimeValue<T> : IRuntimeValue
{
    IRuntimeType IRuntimeValue.Type => Type;
    public new IRuntimeType<T> Type { get; }
}