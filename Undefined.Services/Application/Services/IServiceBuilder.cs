using Undefined.Services.Application.Builder;

namespace Undefined.Services.Application.Services;

public interface IServiceBuilder<in TService> where TService : IService
{
    public IServiceBuilder<TService> As<TImplementation>() where TImplementation : TService, new();
    public IServiceBuilder<TService> Static();
    public IApplicationBuilder Build<TLifetime>() where TLifetime : Scope, new();
}