using Undefined.Services.Lambdas.Abstraction.Presentations;

namespace Undefined.Services.Lambdas.Abstraction.Values.Input;

public interface IInputRuntimeValueOld : IRuntimeValue<>
{
    public IRuntimeDeclarable RuntimeDeclarable { get; }
}