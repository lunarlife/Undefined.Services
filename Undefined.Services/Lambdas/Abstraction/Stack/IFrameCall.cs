using Undefined.Services.Lambdas.Abstraction.Blocks;
using Undefined.Services.Lambdas.Abstraction.Presentations;

namespace Undefined.Services.Lambdas.Abstraction.Stack;

public interface IFrameCall
{
    public ILambdaBlockBaseBuilder Base { get; }
    public ILambdaBlockBaseBuilder Current { get; }
    public IUCollection<IRuntimeDeclarable> InstructionsBetweenBlocks { get; }
}