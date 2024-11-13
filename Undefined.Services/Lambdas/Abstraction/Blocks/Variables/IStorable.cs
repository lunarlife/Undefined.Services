using Undefined.Services.Lambdas.Abstraction.Utils;

namespace Undefined.Services.Lambdas.Abstraction.Blocks.Variables;

public interface IStorable : ISuitable<IAssignable>
{
    public Type Type { get; }
    public bool IsAssigned { get; }
    public void Assign(IAssignable assignable);
}