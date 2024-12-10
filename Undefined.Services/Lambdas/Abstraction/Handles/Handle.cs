namespace Undefined.Services.Lambdas.Abstraction.Handles;



public readonly struct Handle<TRuntimeType>
{
    public IRuntimeHandleable Handleable { get; }

    private Handle(IRuntimeHandleable handleable)
    {
        Handleable = handleable;
    }

    public IRuntimeValue GetHandle() =>

    public static implicit operator Handle<TRuntimeType>(IRuntimeHandleable<TRuntimeType> value) =>
        new(value);

    public static implicit operator Handle<TRuntimeType>(IRuntimeHandleable value) => new(value)
}

public struct Handle : IRuntimeValue
{

    public Handle( )
    {
    }

    public static implicit operator Handle(IRuntimeHandleable value) => new(value);

    public static Handle From(Action action)
    {
        action.Method.GetBaseDefinition
    }
}