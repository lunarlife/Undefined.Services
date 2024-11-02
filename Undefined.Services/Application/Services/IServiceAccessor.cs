using Undefined.Services.Application.Services.Scopes;

namespace Undefined.Services.Application.Services;

public interface IServiceAccessor
{
    public ServiceId ServiceId { get; }
    public Func<IServiceScope, IService> AccessFunc { get; }
}