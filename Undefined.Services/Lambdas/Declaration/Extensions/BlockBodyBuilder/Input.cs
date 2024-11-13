using Undefined.Services.Lambdas.Abstraction.Blocks.Bodies;
using Undefined.Services.Lambdas.Abstraction.Blocks.Variables;
using Undefined.Services.Lambdas.Abstraction.Contexts;
using Undefined.Services.Lambdas.Abstraction.Descriptors;
using Undefined.Services.Lambdas.Abstraction.Values.Input;
using Undefined.Services.Lambdas.Declaration.Blocks.Bodies;
using Undefined.Services.Lambdas.Declaration.Blocks.Variables;

namespace Undefined.Services.Lambdas.Declaration.Extensions.BlockBodyBuilder;

public delegate InputValue InputValueResolver(LambdaBlockBuilderContext context);

public static partial class LambdaBodyBuilderExtensions
{
    public static IValueReturned Descriptor(this IBodyBuilder builder, ITypeDescriptor descriptor)
    {
        builder.Add(p => p.Descriptor(descriptor));
        return new ValueReturned();
    }
    public static IValueReturned New(this IBodyBuilder builder, Type type)
    {
        return builder.Descriptor(TypeDescriptors.Type(type));
    }

}