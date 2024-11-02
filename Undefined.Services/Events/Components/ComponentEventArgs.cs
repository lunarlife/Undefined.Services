using Undefined.Events;
using Undefined.Services.Entities.Components;

namespace Undefined.Services.Events.Components;

public abstract record ComponentEventArgs(IComponent Component) : IEventArgs;

public record ComponentAddEventArgs(IComponent Component) : ComponentEventArgs(Component);

public record ComponentRemoveEventArgs(IComponent Component) : ComponentEventArgs(Component);