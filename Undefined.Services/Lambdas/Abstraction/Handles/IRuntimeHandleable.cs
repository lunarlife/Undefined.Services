using Undefined.Services.Lambdas.Abstraction.Values;

namespace Undefined.Services.Lambdas.Abstraction.Handles;

public interface IRuntimeHandleable<TBuilder, THandle> : IRuntimeHandleable<TBuilder>
    where TBuilder : IHandleBuilder<THandle>
{
    TBuilder IRuntimeHandleable<TBuilder>.NewBuilder() => NewBuilder();

    public new ILambdaCompiled<THandle> NewBuilder();
}

public interface IRuntimeHandleable where TBuilder : IHandleBuilder
{
    public IDelegate<> NewBuilder();
}