using Undefined.Services.Lambdas.Abstraction.Blocks;
using Undefined.Services.Lambdas.Abstraction.Blocks.Bodies;
using Undefined.Services.Lambdas.Abstraction.Presentations;
using Undefined.Services.Lambdas.Declaration.Blocks;

namespace Undefined.Services.Lambdas.Declaration.Extensions.BlockBodyBuilder;

public delegate IBody BlockBodyBuilder<in T>(T builder) where T : ILambdaBlockBaseBuilder;

public static partial class LambdaBodyBuilderExtensions
{
    public static IBodyBuilder NewBlock<T>(this IBodyBuilder builder, T blockBuilder, BlockBodyBuilder<T> bodyBuilder) where T : ILambdaBlockBaseBuilder
    {
        builder.Blocks.Add(bodyBuilder(blockBuilder).Block);
        return builder;
    }

    public static IBodyBuilder DefaultBlock(this IBodyBuilder builder, Func<IBodyBuilder, IBody> bodyBuilder) =>
        builder.NewBlock(new LambdaBlockDefaultBuilder(),b => bodyBuilder(b.Body));

    public static IBodyBuilder IfBlock(this IBodyBuilder builder, Func<LambdaBlockIfBuilder, IBody> bodyBuilder) =>
        builder.NewBlock(new LambdaBlockIfBuilder(),b => bodyBuilder(b));
}