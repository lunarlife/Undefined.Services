using Undefined.Services.Entities.Components;

namespace Undefined.Services;

public static class ServiceExtensions
{
    public static IIsolatedFilterBuilder<T> InitIsolatedFilter<T>(this IService service) where T : Component => new IsolatedFilterBuilder<T>();

    public static bool IsDestroyed<T>(this T service) where T : class, IService
    {
    }

    public static void Destroy<T>(this T service) where T : class, IService
    {
        service.Space().
    }
}