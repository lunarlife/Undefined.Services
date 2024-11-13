using Undefined.Services.Lambdas.Abstraction.Blocks.Variables;
using Undefined.Services.Lambdas.Abstraction.Descriptors;
using Undefined.Services.Lambdas.Abstraction.Stack;

namespace Undefined.Services.Lambdas.Declaration.Stack;


public class LambdaStack : ILambdaStack
{
    public LambdaStack()
    {
        
    }
    public IStackVariable Push(ITypeDescriptor descriptor)
    {
        
    }

    public IStackVariable Input(ITypeDescriptor descriptor, IBlockStack stack) => throw new NotImplementedException();
}