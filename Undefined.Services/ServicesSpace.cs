using Undefined.Events;
using Undefined.Services.Exceptions;
using Undefined.Verifying;

namespace Undefined.Services;

public class ServicesSpace : IDisposable
{
    private static readonly List<ServicesSpace> Spaces = [];
    private static readonly ComponentsInitializer ComponentsInitializer = new();
    private static readonly ServicesInitializer ServicesInitializer = new();

    public static ServicesSpace StaticSpace { get; } = Create();

    private readonly Dictionary<Type, List<ComponentBase>> _allComponents = [];
    private readonly Dictionary<Type, ServiceInstanceData> _servicesInitializers = [];

    private readonly List<SObject> _objects = [];
    private readonly List<ServiceBase> _updatableServices = [];
    private readonly List<ServiceBase> _services = [];

    private bool _isDisposed;

    private readonly object _objectsLock = new();
    private readonly object _servicesInitializersLock = new();
    private readonly object _servicesLock = new();
    private readonly object _updatableLock = new();
    private readonly object _componentsLock = new();

    private Listener? _updateListener;

    public IReadOnlyList<Type> DefaultComponents { get; }

    public IReadOnlyList<SObject> Objects
    {
        get
        {
            lock (_objectsLock)
                return _objects.AsReadOnly();
        }
    }

    public IEventAccess<IEventArgs>? OnUpdate
    {
        set
        {
            _updateListener?.Detach();
            _updateListener = value?.AddListener(UpdateHandler);
        }
    }

    private ServicesSpace(IEventAccess<IEventArgs>? onUpdate, Type[] defaultComponents)
    {
        OnUpdate = onUpdate;
        DefaultComponents = Array.AsReadOnly(defaultComponents);
        if (StaticSpace is null)
            return;
        lock (StaticSpace._servicesInitializersLock)
        {
            foreach (var (key, value) in StaticSpace._servicesInitializers)
                _servicesInitializers.Add(key, value);
            Spaces.Add(this);
        }
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
            for (var i = 0; i < _updatableServices.Count; i++)
            {
                var service = _updatableServices[i];
                if (ServicesInitializer.TryGetServiceUpdateFunc(service.GetType(), out var func)) func!(service);

                ((IUpdatable)service).Update();
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

    public ServicesSpace DeclareService<T>(Scope scope, bool isStatic = false) where T : ServiceBase, new() =>
        DeclareService<T, T>(scope, isStatic);

    public ServicesSpace DeclareService<T, T1>(Scope scope, bool isStatic = false)
        where T : class, IService where T1 : ServiceBase, T, new()
    {
        CheckIsNotDisposed();
        var association = typeof(T);
        if (association == typeof(IService) || association == typeof(ServiceBase))
            throw new SpaceException($"Association cant be {nameof(IService)} or {nameof(ServiceBase)}.");

        if (StaticSpace != this && isStatic)
            return StaticSpace.DeclareService<T, T1>(scope, isStatic);

        var type = typeof(T1);
        if (_updateListener is null && typeof(IUpdatable).IsAssignableFrom(type))
            throw new SpaceException($"Space does not have Update Event. Service {type.Name} is {nameof(IUpdatable)}.");
        lock (_servicesInitializersLock)
        {
            if (_servicesInitializers.ContainsKey(association))
                throw new SpaceException($"Service with type {type.Name} already declared.");
            var func = ServicesInitializer.CreateServiceInitializer(this, type, scope);
            var instanceData = new ServiceInstanceData(this, func, scope);
            _servicesInitializers.Add(association, instanceData);
            if (StaticSpace == this && isStatic)
                foreach (var space in Spaces)
                    space._servicesInitializers.Add(association, instanceData);
        }


        return this;
    }

    public T GetService<T>() where T : class, IService => (T)(IService)_InternalGetService(typeof(T));


    public bool HasDeclaredService(Type type)
    {
        lock (_servicesInitializersLock)
            return _servicesInitializers.ContainsKey(type);
    }

    public void Dispose()
    {
        CheckIsNotDisposed();
        _isDisposed = true;
        _updateListener.Detach();
        lock (_servicesLock)
            foreach (var service in _services)
                if (service is IDisposable disposable)
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


    internal ServiceBase _InternalGetService(Type association)
    {
        lock (_servicesInitializersLock)
            if (!_servicesInitializers.TryGetValue(association, out var data))
                throw new SpaceException($"Association for {association.Name} is not declared.");
            else
                return data.InstanceFunc(data.Scope is Scope.Singleton ? data.Owner : this);
    }

    internal void _InternalInstanceService(ServiceBase service)
    {
        lock (_servicesLock) _services.Add(service);
        if (service is IUpdatable)
            lock (_updatableLock)
                _updatableServices.Add(service);
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
            Verify.Argument(_allComponents.TryGetValue(component.GetType(), out var list) && list.Remove(component),
                "Something was wrong.");
    }

    internal void _InternalDestroyObject(SObject obj)
    {
        CheckIsNotDisposed();
        lock (_objectsLock)
            _objects.Remove(obj);
    }


    internal void _InternalDisposeService(ServiceBase service)
    {
        if (service is IUpdatable)
            lock (_updatableLock)
                _updatableServices.Remove(service);
    }

    public static ServicesSpace Create() => new(null, Type.EmptyTypes);

    public static ServicesSpace Create<T>() where T : ComponentBase =>
        new(null, [typeof(T)]);

    public static ServicesSpace Create<T, T1>() where T : ComponentBase
        where T1 : ComponentBase => new(null, [typeof(T), typeof(T1)]);

    public static ServicesSpace Create<T, T1, T2>() where T : ComponentBase
        where T1 : ComponentBase
        where T2 : ComponentBase => new(null, [typeof(T), typeof(T1), typeof(T2)]);


    public static ServicesSpace Create(IEventAccess<IEventArgs> onUpdate) => new(onUpdate, Type.EmptyTypes);

    public static ServicesSpace Create<T>(IEventAccess<IEventArgs> onUpdate) where T : ComponentBase =>
        new(onUpdate, [typeof(T)]);

    public static ServicesSpace Create<T, T1>(IEventAccess<IEventArgs> onUpdate) where T : ComponentBase
        where T1 : ComponentBase => new(onUpdate, [typeof(T), typeof(T1)]);

    public static ServicesSpace Create<T, T1, T2>(IEventAccess<IEventArgs> onUpdate) where T : ComponentBase
        where T1 : ComponentBase
        where T2 : ComponentBase => new(onUpdate, [typeof(T), typeof(T1), typeof(T2)]);
}