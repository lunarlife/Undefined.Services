using Undefined.Services.Lambdas.Abstraction;
using Undefined.Services.Lambdas.Abstraction.Blocks.Bodies;

namespace Undefined.Services.Lambdas.Declaration.Extensions.BlockBodyBuilder;

public static partial class LambdaBodyBuilderExtensions
{
    public static IVariableCollection Vars(this IBodyBuilder body) => body.Block.DeclaredVariables;
}