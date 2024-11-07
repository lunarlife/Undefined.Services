using Undefined.Services.Application.Services;
using Undefined.Services.Application.Services.Extensions;
using IServiceProvider = Undefined.Services.Application.Services.IServiceProvider;

namespace Undefined.Services.Tests;

public class Tests
{
    private IServiceProvider _provider;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ITestService>(new TestService());
        _provider = services.BuildProvider();
    }

    [Test]
    public void SingletonTest()
    {
        var testService = _provider.GetService<ITestService>();
        var testService1 = _provider.GetService<ITestService>();
        if (!ReferencesEquals(testService, testService1, testService.Service)) Assert.Fail();
        Assert.Pass();
    }

    private bool ReferencesEquals(params object[] objects)
    {
        if (objects.Length < 2) return true;
        var obj = objects[0];
        for (var i = 1; i < objects.Length; i++)
            if (obj != objects[i])
                return false;
        return true;
    }
}