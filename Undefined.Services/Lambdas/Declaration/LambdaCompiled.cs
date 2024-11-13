using Undefined.Services.Lambdas.Abstraction;
using Undefined.Services.Lambdas.Abstraction.Methods;

namespace Undefined.Services.Lambdas.Declaration;

internal class LambdaCompiled : ILambdaCompiled
{
    public LambdaCompiled(ICompiledMethod compiledMethod)
    {
        CompiledMethod = compiledMethod;
    }

    public ICompiledMethod CompiledMethod { get; }
    public IAction DefaultDelegate { get; }
}