using System.Collections;

namespace Undefined.Systents;

public class MultiList<T> : IList<T>, IReadOnlyList<T>
{
    private readonly IList<T> _last;
    private readonly IEnumerable<IList<T>> _lists;
    private readonly object _listsLock = new();

    public int ListsCount { get; }

    public int Count
    {
        get
        {
            lock (_listsLock) return _lists.Sum(list => list.Count);
        }
    }

    public bool IsReadOnly => false;

    public MultiList(IEnumerable<IList<T>> lists)
    {
        _lists = lists;
        foreach (var list in _lists)
        {
            _last = list;
            ListsCount++;
        }
    }

    public void Add(T item)
    {
        lock (_listsLock) _last.Add(item);
    }

    public void Clear()
    {
        lock (_listsLock)
            foreach (var list in _lists)
                list.Clear();
    }

    public bool Contains(T item)
    {
        lock (_listsLock)
            foreach (var list in _lists)
                if (list.Contains(item))
                    return true;

        return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        
    }

    public bool Remove(T item)
    {
        lock (_listsLock)
            foreach (var list in _lists)
                if (list.Remove(item))
                    return true;

        return false;
    }

    public int IndexOf(T item)
    {
        var index = 0;
        lock (_listsLock)
            foreach (var list in _lists)
            {
                var i = list.IndexOf(item);
                if (i != -1)
                    return index + i;
                index += list.Count;
            }

        return -1;
    }

    public void Insert(int index, T item)
    {
        lock (_listsLock)
            foreach (var list in _lists)
            {
                if (index - list.Count < 0)
                {
                    list.Insert(index, item);
                    return;
                }

                index -= list.Count;
            }
    }

    public void RemoveAt(int index)
    {
        lock (_listsLock)
            foreach (var list in _lists)
            {
                if (index - list.Count < 0)
                {
                    list.RemoveAt(index);
                    return;
                }

                index -= list.Count;
            }
    }

    public T this[int index]
    {
        get
        {
            if (index < 0) throw new IndexOutOfRangeException($"Index({index}) cant be less than 0.");
            lock (_listsLock)
                foreach (var list in _lists)
                {
                    if (index - list.Count < 0)
                        return list[index];
                    index -= list.Count;
                }

            throw new IndexOutOfRangeException($"Index {index}.");
        }
        set
        {
            if (index < 0) throw new IndexOutOfRangeException($"Index({index}) cant be less than 0.");
            lock (_listsLock)
                foreach (var list in _lists)
                {
                    if (index - list.Count < 0)
                    {
                        list[index] = value;
                        return;
                    }

                    index -= list.Count;
                }

            throw new IndexOutOfRangeException($"Index {index}.");
        }
    }

    public IEnumerator<T> GetEnumerator() => new MultiListEnumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private class MultiListEnumerator : IEnumerator<T>
    {
        private readonly IEnumerator<IList<T>> _listsEnumerator;
        private IEnumerator<T> _currentEnumerator;
        public T Current => _currentEnumerator.Current;
        object IEnumerator.Current => Current!;

        public MultiListEnumerator(MultiList<T> list)
        {
            _listsEnumerator = list._lists.GetEnumerator();
            _listsEnumerator.MoveNext();
            _currentEnumerator = _listsEnumerator.Current.GetEnumerator();
        }

        public bool MoveNext()
        {
            while (!_currentEnumerator.MoveNext())
            {
                if (!_listsEnumerator.MoveNext())
                    return false;
                _currentEnumerator.Dispose();
                _currentEnumerator = _listsEnumerator.Current.GetEnumerator();
            }

            return true;
        }

        public void Reset()
        {
            _listsEnumerator.Reset();
            _listsEnumerator.MoveNext();
            _currentEnumerator.Dispose();
            _currentEnumerator = _listsEnumerator.Current.GetEnumerator();
        }

        public void Dispose()
        {
            _listsEnumerator.Dispose();
            _currentEnumerator.Dispose();
        }
    }
}