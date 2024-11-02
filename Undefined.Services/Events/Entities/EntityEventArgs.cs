using Undefined.Events;
using Undefined.Services.Entities;

namespace Undefined.Services.Events.Entities;

public abstract record EntityEventArgs(IEntity Entity) : IEventArgs;

public record EntityRemoveEventArgs(IEntity Entity) : EntityEventArgs(Entity);

public record EntityInstantiateEventArgs(IEntity Entity) : EntityEventArgs(Entity);