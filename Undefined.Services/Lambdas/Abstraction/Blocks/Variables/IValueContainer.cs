using Undefined.Services.Lambdas.Abstraction.Handles;
using Undefined.Services.Lambdas.Abstraction.Presentations;

namespace Undefined.Services.Lambdas.Abstraction.Blocks.Variables;

public interface IValueContainer 
{
    public IRuntimeType Type { get; }
}

public interface IValueContainer<T> : IValueContainer
{
    IRuntimeType IValueContainer.Type => Type;

    public new IRuntimeType<T> Type { get; }
}