namespace Undefined.Services.Application.Services;

public readonly struct ServiceId : IEquatable<ServiceId>
{
    public Type ServiceType { get; }

    public ServiceId(Type serviceType)
    {
        ServiceType = serviceType;
    }

    public bool Equals(ServiceId other) => ServiceType == other.ServiceType;

    public override bool Equals(object? obj) => obj is ServiceId other && Equals(other);

    public override int GetHashCode() => ServiceType.GetHashCode();

    public static bool operator ==(ServiceId left, ServiceId right) => left.Equals(right);

    public static bool operator !=(ServiceId left, ServiceId right) => !left.Equals(right);
    public override string ToString() => ServiceType.Name;
}