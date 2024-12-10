using Undefined.Services.Lambdas._Tests.Entries;
using Undefined.Services.Lambdas.Abstraction;
using Undefined.Services.Lambdas.Abstraction.Methods;
using Undefined.Services.Lambdas.Declaration;
using Undefined.Services.Lambdas.Declaration.Extensions.BlockBodyBuilder;

namespace Undefined.Services.Lambdas._Tests;

public class LambdaTests
{
    public LambdaTests()
    {
        LambdaBuilder.BuildLambda(builder =>
        {
            builder.Invoke(() =>
            {
                Console.WriteLine("Hello World!");
            });
        })
    }

    public (int i, string a) Test()
    {
    }
}