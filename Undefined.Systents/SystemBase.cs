namespace Undefined.Systents;

public interface ISystem
{
    public Space Space { get; }
}

public abstract class SystemBase : ISystem
{
    public Space Space { get; }
}