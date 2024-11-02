using System.Collections;
using System.Data;

namespace Undefined.Services.Application.Services;

public class ServiceCollection : IServiceCollection
{
    private readonly List<IServiceDescriptor> _descriptors = [];
    
    public int Count => _descriptors.Count;

    public bool IsReadOnly { get; private set; }


    public IServiceDescriptor this[int index]
    {
        get => _descriptors[index];
        set
        {
            CheckReadOnly();
            _descriptors[index] = value;
        }
    }

    public void MakeReadOnly() => IsReadOnly = true;

    public void Add(IServiceDescriptor item)
    {
        CheckReadOnly();
        _descriptors.Add(item);
    }

    public void Clear()
    {
        CheckReadOnly();
        _descriptors.Clear();
    }

    public bool Contains(IServiceDescriptor item) => _descriptors.Contains(item);

    public void CopyTo(IServiceDescriptor[] array, int arrayIndex) => _descriptors.CopyTo(array, arrayIndex);

    public bool Remove(IServiceDescriptor item)
    {
        CheckReadOnly();
        return _descriptors.Remove(item);
    }


    public int IndexOf(IServiceDescriptor item) => _descriptors.IndexOf(item);

    public void Insert(int index, IServiceDescriptor item)
    {
        CheckReadOnly();
        _descriptors.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        CheckReadOnly();
        _descriptors.RemoveAt(index);
    }


    public IEnumerator<IServiceDescriptor> GetEnumerator() => _descriptors.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_descriptors).GetEnumerator();

    private void CheckReadOnly()
    {
        if (!IsReadOnly) return;
        throw new ReadOnlyException();
    }
}