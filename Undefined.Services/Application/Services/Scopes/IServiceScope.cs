namespace Undefined.Services.Application.Services.Scopes;

public interface IServiceScope : IDisposable
{
    public IServiceProvider ServiceProvider { get; }
}