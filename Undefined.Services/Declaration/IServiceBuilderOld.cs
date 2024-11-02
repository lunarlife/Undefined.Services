using Undefined.Services.Application.Builder;
using Undefined.Services.Scopes;

namespace Undefined.Services.Declaration;

public interface IServiceBuilderOld<in TService> where TService : class, IService
{
    public IServiceBuilderOld<TService> As<TImplementation>() where TImplementation : class, TService, new();
    public IServiceBuilderOld<TService> Static();
    public IApplicationBuilder Build<TScope>() where TScope : Scope, new();
}