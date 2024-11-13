namespace Undefined.Services.Lambdas.Abstraction.Handles;

public interface IHandleBuilder
{
    public IRuntimeType HandleType { get; }
}

public interface IHandleBuilder<T> : IHandleBuilder
{
    IRuntimeType IHandleBuilder.HandleType => HandleType;
    public new IRuntimeType<T> HandleType { get; }
}

public static class RuntimeType
{
    public static IRuntimeType<T> From<T>() => new RuntimeTypeImpl()
}

public class RuntimeTypeImpl<T> : IRuntimeType<T>
{
}