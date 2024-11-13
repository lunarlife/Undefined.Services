using Undefined.Services.Lambdas.Abstraction.Blocks.Bodies;
using Undefined.Services.Lambdas.Abstraction.Blocks.Variables;
using Undefined.Services.Lambdas.Abstraction.Contexts;
using Undefined.Services.Lambdas.Abstraction.Methods;
using Undefined.Services.Lambdas.Abstraction.Methods.Parameters;

namespace Undefined.Services.Lambdas.Declaration.Extensions.BlockBodyBuilder;

public delegate DelegateParameterized InvokeResolver(LambdaBlockBuilderContext context);

public static partial class LambdaBodyBuilderExtensions
{
    public static IStackVariable Invoke(this IBodyBuilder builder, IAction del, IDelegateInputValueCollection collection)
    {
    }

    public static IStackVariable Invoke(this IBodyBuilder builder, InvokeResolver resolver)
    {
        return 
    }
}