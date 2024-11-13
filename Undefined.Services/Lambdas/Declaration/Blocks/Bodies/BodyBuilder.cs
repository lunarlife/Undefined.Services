using Undefined.Services.Lambdas.Abstraction;
using Undefined.Services.Lambdas.Abstraction.Blocks.Bodies;
using Undefined.Services.Lambdas.Abstraction.Presentations;
using Undefined.Services.Lambdas.Utils;

namespace Undefined.Services.Lambdas.Declaration.Blocks.Bodies;

internal class BodyBuilder : UCollection<Presentation>, IBodyBuilder
{
    public ILambdaBlock Block { get; }
    public IBlockCollection Blocks { get; }
    public ILambdaBlock? BaseBlock { get; }
    public ILambdaFrame Frame { get; }
    public IBody Build() => new Body(Block, Blocks, BaseBlock, Frame);
}