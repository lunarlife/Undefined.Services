using Undefined.Services.Application;
using Undefined.Services.Application.Services;
using Undefined.Services.Application.Services.Extensions;
using Undefined.Services.Entities.Templates.Builder;
using Undefined.Services.Scopes;

namespace Undefined.Services._test;

public class _Test
{
    public _Test()
    {
        var builder = new EntityTemplateBuilder();
        builder.Component<TestComponent>().Preset(Console.WriteLine).Variable().Build().Build();


        var services = new ServiceCollection();
        var provider = services.BuildProvider();
       // provider.CreateScope().ServiceProvider.
    }
}

public interface ITest : IService
{
}

public class Test : ITest
{
}