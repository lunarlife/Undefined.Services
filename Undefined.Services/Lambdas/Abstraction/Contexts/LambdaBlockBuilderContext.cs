using Undefined.Services.Lambdas.Abstraction.Blocks.Bodies;

namespace Undefined.Services.Lambdas.Abstraction.Contexts;

public readonly struct LambdaBlockBuilderContext
{
    public IBodyBuilder Block { get; }
    
    public LambdaBlockBuilderContext(IBodyBuilder block)
    {
        Block = block;
    }

}