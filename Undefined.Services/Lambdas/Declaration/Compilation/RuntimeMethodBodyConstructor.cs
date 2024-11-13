using Undefined.Services.Lambdas.Abstraction.Compilation;

namespace Undefined.Services.Lambdas.Declaration.Compilation;

public class RuntimeMethodBodyConstructor : IRuntimeMethodBodyConstructor
{
    public void EmitObject() => throw new NotImplementedException();

    public IRuntimeMethodBody Build() => throw new NotImplementedException();
}