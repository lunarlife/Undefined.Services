using Undefined.Services.Lambdas.Abstraction.Blocks.Variables;
using Undefined.Services.Lambdas.Abstraction.Stack;

namespace Undefined.Services.Lambdas.Abstraction;

public interface ILambdaFrame : IUCollection<IStackVariable>
{
    public IFrameStackVariablesCollection FrameStackVariables { get; }
}