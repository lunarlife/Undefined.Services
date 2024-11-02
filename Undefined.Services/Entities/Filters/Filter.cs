using System.Collections;
using Undefined.Services.Entities.Components;

namespace Undefined.Services.Entities.Filters;


public interface IFilter
{
    public void ForceUpdate();
}

public interface IFilter<out T> : IFilter, IReadOnlyList<T> where T : Component;

internal class Filter<T> : IFilter<T> where T : Component
{
    private readonly IEntityContainer _container;
    private IReadOnlyList<T> _list;

    public int Count => _list.Count;
    public T this[int index] => _list[index];

    public Filter(IEntityContainer container)
    {
        _container = container;
        ForceUpdate();
    }
    

    public void ForceUpdate()
    {
        _list = _container.FindComponents<T>();
    }

    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}