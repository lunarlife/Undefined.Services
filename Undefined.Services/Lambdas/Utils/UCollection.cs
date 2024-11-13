using System.Collections;
using System.Data;

namespace Undefined.Services.Lambdas.Utils;

internal abstract class UCollection<T> : Abstraction.IUCollection<T>
{
    private readonly IList<T> _collection = [];

    public bool IsReadOnly { get; private set; }
    public int Count => _collection.Count;

    public T this[int index]
    {
        set
        {
            CheckReadOnly();
            _collection[index] = value;
        }
        get => _collection[index];
    }

    public void MakeReadOnly()
    {
        CheckReadOnly();
        IsReadOnly = true;
    }

    public void Add(T item)
    {
        CheckReadOnly();
        _collection.Add(item);
    }

    public void Clear()
    {
        CheckReadOnly();
        _collection.Clear();
    }

    public bool Remove(T item)
    {
        CheckReadOnly();
        return _collection.Remove(item);
    }

    public bool Contains(T item) => _collection.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => _collection.CopyTo(array, arrayIndex);


    protected void CheckReadOnly()
    {
        if (IsReadOnly)
            throw new ReadOnlyException("Collection is read-only.");
    }

    public IEnumerator<T> GetEnumerator() => _collection.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_collection).GetEnumerator();
}