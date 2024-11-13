using Undefined.Services.Lambdas.Abstraction.Blocks.Bodies;
using Undefined.Services.Lambdas.Abstraction.Values;

namespace Undefined.Services.Lambdas.Abstraction.Blocks;

public interface ILambdaBlockDefaultBuilder : ILambdaBlockBaseBuilder
{
    public IBodyBuilder Body { get; }
}