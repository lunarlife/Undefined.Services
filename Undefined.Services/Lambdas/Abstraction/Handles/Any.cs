namespace Undefined.Services.Lambdas.Abstraction.Handles;



public readonly struct Handle<TRuntimeType>
{
    public IRuntimeHandleable Handleable { get; }

    private Handle(IRuntimeHandleable handleable)
    {
        Handleable = handleable;
    }

    public IRuntimeHandle GetHandle() =>

    public static implicit operator Handle<TRuntimeType>(IRuntimeHandleable<TRuntimeType> value) =>
        new(value);

    public static implicit operator Handle<TRuntimeType>(IRuntimeHandleable value) => new(value)
}

public struct Any<T>
{
    public T Value { get; }

    public Any(T value)
    {
        Value = value;
    }

    public static implicit operator Any<T>(T value) => new(value);
}