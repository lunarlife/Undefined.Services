using Undefined.Services.Lambdas.Abstraction.Blocks;
using Undefined.Services.Lambdas.Abstraction.Blocks.Bodies;
using Undefined.Services.Lambdas.Abstraction.Compilation;

namespace Undefined.Services.Lambdas.Abstraction;

public interface ILambdaBuilder
{
    public IBody Body { get; }
    public ILambdaRuntime Compile(IRuntimeMethodBodyConstructor constructor);

}
