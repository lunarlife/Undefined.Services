using Undefined.Services.Lambdas.Abstraction.Compilation;
using Undefined.Services.Lambdas.Abstraction.Values;

namespace Undefined.Services.Lambdas.Declaration.Values;

internal class RuntimeValueImpl : IRuntimeValue<>
{
    public RuntimeValueImpl()
    {
        
    }
    public void Declare(IRuntimeMethodBodyConstructor constructor) => throw new NotImplementedException();
}
internal class RuntimeValueImpl<T> : RuntimeValueImpl
{
    public void Emit(IRuntimeMethodBodyConstructor constructor)
    {
    }
}