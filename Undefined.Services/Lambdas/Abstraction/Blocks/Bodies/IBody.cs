using Undefined.Services.Lambdas.Abstraction.Presentations;

namespace Undefined.Services.Lambdas.Abstraction.Blocks.Bodies;

public interface IBody : IRuntimeDeclarable
{
    public ILambdaBlock Block { get; }
}