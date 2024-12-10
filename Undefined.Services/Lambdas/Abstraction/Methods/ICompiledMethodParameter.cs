using Undefined.Services.Lambdas.Abstraction.Handles;
using Undefined.Services.Lambdas.Abstraction.Values;

namespace Undefined.Services.Lambdas.Abstraction.Methods;

public interface  ICompiledMethodParameter
{
    public IRuntimeType ParameterType { get; }
    public bool IsOptional { get; }
    public string DefaultValue { get; }
}