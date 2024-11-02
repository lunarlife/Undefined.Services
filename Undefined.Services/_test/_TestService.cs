using Undefined.Services.Entities.Components;
using Undefined.Services.Entities.Filters;

namespace Undefined.Services._test;

public class TestService : IService
{
    [Filter] private readonly IFilter<Component> _filter;
    private readonly IFilter<Component> _filter1;

    public TestService()
    {
    }
}