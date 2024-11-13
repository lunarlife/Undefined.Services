using Undefined.Services.Lambdas.Abstraction.Blocks;
using Undefined.Services.Lambdas.Abstraction.Blocks.Bodies;
using Undefined.Services.Lambdas.Abstraction.Values;

namespace Undefined.Services.Lambdas.Declaration.Blocks;

public class LambdaBlockIfBuilder : ILambdaBlockIfBuilder
{
    public IBodyBuilder Condition(IRuntimeValue<> booleanRuntimeValue) => throw new NotImplementedException();
}