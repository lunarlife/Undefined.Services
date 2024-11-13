namespace Undefined.Services.Lambdas.Abstraction.TypeBuilding;

public interface ITypeFactoryBuilder
{
    public ITypeFactoryBuilder ParameterlessConstructor();
    public ITypeFactory Build();
}