using Undefined.Services.Lambdas.Abstraction;
using Undefined.Services.Lambdas.Abstraction.Blocks.Bodies;
using Undefined.Services.Lambdas.Abstraction.Compilation;
using Undefined.Services.Lambdas.Declaration.Blocks.Bodies;

namespace Undefined.Services.Lambdas.Declaration;

internal class LambdaBuilder : ILambdaBuilder
{
    public LambdaBuilder(IBody body)
    {
        Body = body;
    }

    public IBody Body { get; }

    public ILambdaRuntime Compile(IRuntimeMethodBodyConstructor constructor)
    {
        MakeReadOnly();

        return new LambdaRuntime()
    }

    public static ILambdaBuilder BuildLambda(Func<IBodyBuilder, IBody> bodyBuilder) =>
        new LambdaBuilder(bodyBuilder(new BodyBuilder()));
}