using Undefined.Services.Lambdas.Abstraction.Blocks.Variables;
using Undefined.Services.Lambdas.Abstraction.Descriptors;

namespace Undefined.Services.Lambdas.Abstraction.Stack;

public interface ILambdaStack : IBlockStack
{
    public IStackVariable Input(ITypeDescriptor descriptor, IBlockStack stack);
}