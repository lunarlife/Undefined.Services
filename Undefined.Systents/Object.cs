namespace Undefined.Systents;


public abstract class IObject
{
    public IReadOnlyList<ComponentBase> Components { get; }
    
}
public interface IObject<out T> : IObject where T : ComponentBase
{
    public new IReadOnlyList<T> Components { get; }
    IReadOnlyList<ComponentBase> IObject.Components => Components;
}