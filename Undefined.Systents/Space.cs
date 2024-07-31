using Undefined.Events;
using Undefined.Systents.Exceptions;
using Undefined.Verify;

namespace Undefined.Systents;

public class Space : IDisposable
{
    private static readonly ComponentsInitializer ComponentsInitializer = new();
    private static readonly SystemsInitializer SystemsInitializer = new();

    private readonly Dictionary<Type, List<ComponentBase>> _allComponents = [];
    private readonly Dictionary<Type, SystemInstanceFunc> _systemsInitializers = [];

    private readonly List<SObject> _objects = [];
    private readonly List<SystemBase> _updatableSystems = [];
    private readonly List<SystemBase> _systems = [];

    private bool _isDisposed;

    private readonly object _objectsLock = new();
    private readonly object _systemsInitializersLock = new();
    private readonly object _systemsLock = new();
    private readonly object _updatableLock = new();
    private readonly object _componentsLock = new();

    private readonly Listener _updateListener;

    public IReadOnlyList<Type> DefaultComponents { get; }

    public IReadOnlyList<SObject> Objects
    {
        get
        {
            lock (_objectsLock)
                return _objects.AsReadOnly();
        }
    }

    private Space(IEventAccess<IEventArgs> onUpdate, Type[] defaultComponents)
    {
        DefaultComponents = Array.AsReadOnly(defaultComponents);
        _updateListener = onUpdate.AddListener(UpdateHandler);
    }


    public IReadOnlyList<ComponentBase> GetComponents(Type type)
    {
        lock (_componentsLock)
            if (_allComponents.TryGetValue(type, out var list))
                return list.AsReadOnly();
        if (!type.IsAbstract) return [];
        var components = new List<IList<ComponentBase>>();
        lock (_componentsLock)
            foreach (var (t, l) in _allComponents)
            {
                if (!t.IsAssignableFrom(type)) continue;
                components.Add(l);
            }

        return new MultiList<ComponentBase>(components);
    }

    public IReadOnlyList<T> GetComponents<T>() where T : ComponentBase => (IReadOnlyList<T>)GetComponents(typeof(T));


    private void UpdateHandler(IEventArgs args)
    {
        lock (_updatableLock)
            for (var i = 0; i < _updatableSystems.Count; i++)
            {
                var system = _updatableSystems[i];
                if (SystemsInitializer.TryGetSystemUpdateFunc(system.GetType(), out var func)) func!(system);

                ((IUpdatable)system).Update();
            }
    }

    public SObject Instantiate()
    {
        CheckIsNotDisposed();
        var obj = new SObject(this);
        lock (_objectsLock) _objects.Add(obj);
        return obj;
    }

    public T Instantiate<T>() where T : ComponentBase, new()
    {
        CheckIsNotDisposed();
        var obj = Instantiate();
        return obj.AddComponent<T>();
    }

    public void DeclareSystem<T>(SystemScope scope) where T : SystemBase, new() => DeclareSystem<T, T>(scope);

    public void DeclareSystem<T, T1>(SystemScope scope) where T : class, ISystem where T1 : SystemBase, T, new()
    {
        CheckIsNotDisposed();
        var association = typeof(T);
        var type = typeof(T1);
        lock (_systemsInitializersLock)
        {
            if (_systemsInitializers.ContainsKey(association))
                throw new SpaceException($"System with type {type.Name} already declared.");
            _systemsInitializers.Add(association, SystemsInitializer.CreateSystemInitializer(this, type, scope));
        }
    }

    public T GetSystem<T>() where T : class, ISystem => (T)(ISystem)_InternalGetSystem(typeof(T));


    public bool HasDeclaredSystem(Type type)
    {
        lock (_systemsInitializersLock)
            return _systemsInitializers.ContainsKey(type);
    }

    public void Dispose()
    {
        CheckIsNotDisposed();
        _isDisposed = true;
        _updateListener.Detach();
        lock (_systemsLock)
            foreach (var system in _systems)
                if (system is IDisposable disposable)
                    disposable.Dispose();
    }

    private void CheckIsNotDisposed()
    {
        if (_isDisposed) throw new ObjectDisposedException("Space already disposed.");
    }

    internal IList<ComponentBase> _InternalGetFilter(Type baseType)
    {
        if (baseType.IsAbstract || baseType.IsInterface)
        {
            var subList = new List<List<ComponentBase>>();
            lock (_componentsLock)
                foreach (var pair in _allComponents)
                {
                    if (!baseType.IsAssignableFrom(pair.Key))
                        continue;
                    subList.Add(pair.Value);
                }

            return new MultiList<ComponentBase>(subList);
        }

        lock (_componentsLock)
            if (_allComponents.TryGetValue(baseType, out var list))
                return list.AsReadOnly();
            else
                return Array.Empty<ComponentBase>();
    }


    internal SystemBase _InternalGetSystem(Type association)
    {
        lock (_systemsInitializersLock)
            if (!_systemsInitializers.TryGetValue(association, out var func))
                throw new SpaceException($"Association for {association.Name} is not declared.");
            else
                return func(this);
    }

    internal void _InternalInstanceSystem(SystemBase system)
    {
        lock (_systemsLock) _systems.Add(system);
        if (system is IUpdatable)
            lock (_updatableLock)
                _updatableSystems.Add(system);
    }

    internal ComponentBase _InternalDeclareComponent(SObject obj, Type componentType)
    {
        CheckIsNotDisposed();
        var component = ComponentsInitializer.CreateComponent(obj, componentType);
        List<ComponentBase>? list;
        lock (_componentsLock)
            if (!_allComponents.TryGetValue(componentType, out list))
            {
                list = [];
                _allComponents.Add(componentType, list);
            }

        list.Add(component);
        return component;
    }

    internal void _InternalDestroyComponent(ComponentBase component)
    {
        CheckIsNotDisposed();
        lock (_componentsLock)
            Verifying.Argument(_allComponents.TryGetValue(component.GetType(), out var list) && list.Remove(component),
                "Something was wrong.");
    }

    internal void _InternalDestroyObject(SObject obj)
    {
        CheckIsNotDisposed();
        lock (_objectsLock)
            _objects.Remove(obj);
    }


    internal void _InternalDisposeSystem(SystemBase system)
    {
        if (system is IUpdatable)
            lock (_updatableLock)
                _updatableSystems.Remove(system);
    }

    public static Space Create(IEventAccess<IEventArgs> onUpdate) => new(onUpdate, Type.EmptyTypes);

    public static Space Create<T>(IEventAccess<IEventArgs> onUpdate) where T : ComponentBase =>
        new(onUpdate, [typeof(T)]);

    public static Space Create<T, T1>(IEventAccess<IEventArgs> onUpdate) where T : ComponentBase
        where T1 : ComponentBase => new(onUpdate, [typeof(T), typeof(T1)]);

    public static Space Create<T, T1, T2>(IEventAccess<IEventArgs> onUpdate) where T : ComponentBase
        where T1 : ComponentBase
        where T2 : ComponentBase => new(onUpdate, [typeof(T), typeof(T1), typeof(T2)]);
}