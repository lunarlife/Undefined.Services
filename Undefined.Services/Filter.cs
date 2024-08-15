using System.Collections;

namespace Undefined.Services;

internal interface IFilter;

public readonly struct Filter<T> : IFilter, IReadOnlyList<T> where T : ComponentBase
{
    private readonly IList<ComponentBase> _list;

    public int Count => _list.Count;

    public T this[int index] => (T)_list[index];

    private Filter(IList<ComponentBase> list)
    {
        _list = list;
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var component in _list)
            yield return (T)component;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}