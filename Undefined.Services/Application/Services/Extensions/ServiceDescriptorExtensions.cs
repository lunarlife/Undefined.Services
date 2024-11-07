namespace Undefined.Services.Application.Services.Extensions;

public static class ServiceDescriptorExtensions
{
    public static bool HasImplementationType(this IServiceDescriptor descriptor) => descriptor.ImplementationType != null;
    public static bool HasImplementation(this IServiceDescriptor descriptor) => descriptor.Implementation != null;
    public static bool HasFactory(this IServiceDescriptor descriptor) => descriptor.ImplementationFactory != null;
}