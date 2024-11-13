using Undefined.Services.Lambdas.Abstraction.Blocks.Bodies;
using Undefined.Services.Lambdas.Abstraction.Blocks.Variables;

namespace Undefined.Services.Lambdas.Declaration.Blocks.Variables;

internal class ValueReturned : IValueReturned
{
    private DisposeObject _do;
    public Type Type { get; }
    public void AssignTo(Func<IStorable> storable) => throw new NotImplementedException();

    public void AssignTo(IStorable storable)
    {
        _do.CheckDisposed();
        storable.Assign(this);
    }

    public bool IsSuitable<T1>() where T1 : IStorable => throw new NotImplementedException();

    public bool IsSuitable(object target) => throw new NotImplementedException();
}