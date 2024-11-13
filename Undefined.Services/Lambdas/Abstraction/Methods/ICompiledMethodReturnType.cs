using Undefined.Services.Lambdas.Abstraction.Handles;
using Undefined.Services.Lambdas.Abstraction.Values;

namespace Undefined.Services.Lambdas.Abstraction.Methods;

public interface ICompiledMethodReturnType
{
    public IRuntimeType ReturnType { get; }
}

public interface ICompiledMethodReturnType<TReturn> : ICompiledMethodReturnType
{
    IRuntimeType ICompiledMethodReturnType.ReturnType => ReturnType;

    public new  IRuntimeType<TReturn> ReturnType { get; }

}