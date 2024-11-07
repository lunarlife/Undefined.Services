namespace Undefined.Services.Tests;

internal class TestService : ITestService
{
    private readonly ITestService _testService;

    public ITestService Service => _testService;
}