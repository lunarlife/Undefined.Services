using Undefined.Services.Lambdas.Abstraction.Blocks.Variables;
using Undefined.Services.Lambdas.Abstraction.Descriptors;

namespace Undefined.Services.Lambdas.Abstraction.Stack;

public interface IBlockStack
{
    public IStackVariable Push(ITypeDescriptor descriptor);
}