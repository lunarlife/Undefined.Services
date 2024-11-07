using Undefined.Services.Exceptions;

namespace Undefined.Services.Application.Services.Extensions;

public static class ServiceProviderExtensions
{
    public static T GetService<T>(this IServiceProvider provider) where T : IService =>
        (T)provider.GetService(typeof(T));

    public static IService GetService(this IServiceProvider provider, Type serviceType) =>
        provider.TryGetService(serviceType, out var service)
            ? service!
            : throw new ServiceResolveException("Service not found.");

    public static bool TryGetService<TService>(this IServiceProvider provider, out TService? service)
        where TService : IService
    {
        var got = provider.TryGetService(typeof(TService), out var value);
        service = got ? (TService)value! : default;
        return got;
    }
}