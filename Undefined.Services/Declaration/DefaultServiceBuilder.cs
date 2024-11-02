using Undefined.Services.Application.Builder;
using Undefined.Services.Scopes;

namespace Undefined.Services.Declaration;

public class DefaultServiceBuilder<TService> : IServiceBuilderOld<TService> where TService : class, IService
{
    private readonly IApplicationBuilder _builder;
    private bool _isStatic;

    private Type _implementationType;

    public DefaultServiceBuilder(IApplicationBuilder builder)
    {
        _builder = builder;
    }

    public IServiceBuilderOld<TService> As<TImplementation>() where TImplementation : class, TService, new()
    {
        _implementationType = typeof(TImplementation);
        return this;
    }

    public IServiceBuilderOld<TService> Static()
    {
        _isStatic = true;
        return this;
    }

    public IApplicationBuilder Build<TScope>() where TScope : Scope, new()
    {
        _builder.DeclareResolver(new DefaultServiceResolver(_builder.GetScope<TScope>(), typeof(TService),
            _implementationType, _builder, _isStatic));
        return _builder;
    }
}