using Undefined.Services.Lambdas.Abstraction.Blocks;
using Undefined.Services.Lambdas.Abstraction.Blocks.Bodies;
using Undefined.Services.Lambdas.Abstraction.Values;
using Undefined.Services.Lambdas.Declaration.Blocks.Bodies;

namespace Undefined.Services.Lambdas.Declaration.Blocks;

internal class LambdaBlockDefaultBuilder : LambdaBlockBaseBuilder, ILambdaBlockDefaultBuilder
{
    public IBodyBuilder Body { get; } = new BodyBuilder();
}