namespace Undefined.Services.Lambdas.Abstraction;

public interface IUCollection<T> :  IUReadOnlyCollection<T>
{
    public new T this[int index] {  set; }
}
public interface IUReadOnlyCollection<T> : ICollection<T>
{
    public T this[int index] { get; }
}
