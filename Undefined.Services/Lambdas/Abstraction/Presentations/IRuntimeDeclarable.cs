using Undefined.Services.Lambdas.Abstraction.Compilation;

namespace Undefined.Services.Lambdas.Abstraction.Presentations;

public interface IRuntimeDeclarable

{
    public void Declare(IRuntimeMethodBodyConstructor constructor);
}