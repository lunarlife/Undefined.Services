namespace Undefined.Systents;

public abstract class System
{
    public Space Space { get; private set; }

    internal void _InternalInit(Space space)
    {
        Space = space;
    }
}