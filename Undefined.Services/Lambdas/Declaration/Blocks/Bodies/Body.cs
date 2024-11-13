using Undefined.Services.Lambdas.Abstraction.Blocks.Bodies;
using Undefined.Services.Lambdas.Abstraction.Compilation;
using Undefined.Services.Lambdas.Abstraction.Presentations;

namespace Undefined.Services.Lambdas.Declaration.Blocks.Bodies;

internal class Body : IBody
{
    public Body(ILambdaBlock block)
    {
        Block = block;
    }

    public void Declare(IRuntimeMethodBodyConstructor constructor)
    {
    }

    public ILambdaBlock Block { get; }
}