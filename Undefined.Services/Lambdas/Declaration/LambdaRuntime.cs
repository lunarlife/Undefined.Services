using Undefined.Services.Lambdas.Abstraction;
using Undefined.Services.Lambdas.Abstraction.Methods;
using Undefined.Services.Lambdas.Abstraction.Presentations;

namespace Undefined.Services.Lambdas.Declaration;

internal class LambdaRuntime : LambdaCompiled, ILambdaRuntime
{
    public LambdaRuntime( ICompiledMethod compiledMethod) : base(compiledMethod)
    {
    }

    public IUCollection<IRuntimeDeclarable> LambdaTree { get; }
}