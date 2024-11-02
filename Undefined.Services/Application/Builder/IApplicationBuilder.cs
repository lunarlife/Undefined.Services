
namespace Undefined.Services.Application.Builder;

public interface IApplicationBuilder
{
    public IApplicationProvider ApplicationProvider { get; }
    
    public IApplicationHost Build();
}