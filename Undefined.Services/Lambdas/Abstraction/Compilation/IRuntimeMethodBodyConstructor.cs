using Undefined.Services.Lambdas.Abstraction.Descriptors;
using Undefined.Services.Lambdas.Abstraction.Methods;

namespace Undefined.Services.Lambdas.Abstraction.Compilation;

public interface IRuntimeMethodBodyConstructor
{
    public void EmitDelegateInvoke(DelegateParameterized del);
    public IRuntimeMethodBody Build();
    public void Descriptor(ITypeDescriptor descriptor);
}