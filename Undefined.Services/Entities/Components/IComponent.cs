using Undefined.Events;
using Undefined.Services.Events.Components;

namespace Undefined.Services.Entities.Components;

public interface IComponent : IDisposable
{
    public IEntity Entity { get; }

    public IEventAccess<ComponentRemoveEventArgs> OnRemove { get; }

    public void Remove();
}