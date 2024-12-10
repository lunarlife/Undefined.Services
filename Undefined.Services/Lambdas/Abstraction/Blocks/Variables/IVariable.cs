using Undefined.Services.Lambdas.Abstraction.Presentations;

namespace Undefined.Services.Lambdas.Abstraction.Blocks.Variables;

public interface IVariable : IValueContainer, IRuntimeDeclarable
{
    public string Name { get; }
    public Type VariableType { get; }
    public ILambdaBlock DeclaredBlock { get; }
}

public interface IVariable<T> : IVariable
{
}