using Undefined.Services.Lambdas.Abstraction.Blocks.Bodies;
using Undefined.Services.Lambdas.Abstraction.Handles;
using Undefined.Services.Lambdas.Abstraction.Values;

namespace Undefined.Services.Lambdas.Abstraction.Blocks;

public interface ILambdaBlockIfBuilder : ILambdaBlockBaseBuilder
{
    public IBodyBuilder Condition(IRuntimeType<bool> runtimeType);

}