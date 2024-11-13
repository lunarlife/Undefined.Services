using Undefined.Services.Lambdas.Abstraction.Blocks.Bodies;
using Undefined.Services.Lambdas.Abstraction.Utils;

namespace Undefined.Services.Lambdas.Abstraction.Blocks.Variables;

public interface IAssignable : ISuitable<IStorable>
{
    public Type Type { get; }
    public IBodyBuilder AssignTo(Func<IStorable> storable);
}