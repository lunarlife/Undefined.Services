namespace Undefined.Services.Lambdas.Abstraction.Descriptors;

public interface ITypeDescriptor
{
    
    public Type? Type { get; }
    public Delegate? GetterDelegate { get; }
}

public class TypeDescriptors
{
    

    public static ITypeDescriptor Type(Type type) => new TypeDescriptor(type);
    public static ITypeDescriptor FromType(Type type) => new TypeDescriptor(type);
}
internal class TypeDescriptor : ITypeDescriptor
{
    
    public Type? Type { get; }
    public Delegate? GetterDelegate { get; }
    
    public TypeDescriptor(Type type)
    {
        Type = type;
    }


}