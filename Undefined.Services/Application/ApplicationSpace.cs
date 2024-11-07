/*using Undefined.Events;
using Undefined.Services.Declaration;
using Undefined.Services.Entities;
using Undefined.Services.Entities.Components;
using Undefined.Services.Exceptions;
using Undefined.Verifying;

namespace Undefined.Services.Application;

public class ApplicationSpace : IDisposable
{
    private static readonly List<ApplicationSpace> Spaces = [];
    private static readonly ComponentsInitializer ComponentsInitializer = new();
    private static readonly ServicesInitializer ServicesInitializer = new();

    public static ApplicationSpace StaticSpace { get; } = Create();

    private readonly Dictionary<Type, List<Component>> _allComponents = [];
    private readonly Dictionary<Type, IServiceResolver> _declaredServicesTypes = [];
    private readonly List<IServiceResolver> _declaredServices = [];

    private readonly List<UObject> _objects = [];
    private readonly List<IService> _updatableServices = [];
    private readonly List<IService> _services = [];

    private bool _isDisposed;

    private readonly object _objectsLock = new();
    private readonly object _declaredServicesLock = new();
    private readonly object _servicesLock = new();
    private readonly object _updatableLock = new();
    private readonly object _componentsLock = new();

    private Listener? _updateListener;
    private static readonly Type BaseType = typeof(IService);

    public IReadOnlyList<Type> DefaultComponents { get; }

    public IReadOnlyList<UObject> Objects
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

    public IServiceInstaller Installer { get; }
    public IApplicationProvider Provider { get; }
    public IServiceRESOLVE_PROVIDER Services { get; }
    public IEntityContainer EntityContainer { get; }

    private ApplicationSpace(IEventAccess<IEventArgs>? onUpdate, Type[] defaultComponents)
    {
        OnUpdate = onUpdate;
        DefaultComponents = Array.AsReadOnly(defaultComponents);
        if (StaticSpace is null)
            return;
        lock (StaticSpace._declaredServicesLock)
        {
            foreach (var (key, value) in StaticSpace._servicesInitializers)
                _servicesInitializers.Add(key, value);
            Spaces.Add(this);
        }
    }


    public IList<Component> GetComponents(Type type)
    {
        if (type.IsAbstract || type.IsInterface)
        {
            var subList = new List<List<Component>>();
            lock (_componentsLock)
                foreach (var pair in _allComponents)
                {
                    if (!type.IsAssignableFrom(pair.Key))
                        continue;
                    subList.Add(pair.Value);
                }

            return new MultiList<Component>(subList);
        }

        lock (_componentsLock)
            if (_allComponents.TryGetValue(type, out var list))
                return list.AsReadOnly();
            else
                return [];
    }

    public IList<T> GetComponents<T>() where T : Component => (IList<T>)GetComponents(typeof(T));


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

    public UObject Instantiate()
    {
        CheckIsNotDisposed();
        var obj = new UObject(this);
        lock (_objectsLock) _objects.Add(obj);
        return obj;
    }

    public T Instantiate<T>() where T : Component, new()
    {
        CheckIsNotDisposed();
        var obj = Instantiate();
        return obj.AddComponent<T>();
    }

    public IServiceBuilderOld<TService> DeclareService<TService>()
        where TService : class, IService
    {
        var type = typeof(TService);
        Verify.Argument(_declaredServicesTypes.ContainsKey(type),
            $"{nameof(ApplicationSpace)} has already declared service {type.Name}.");
        return new DefaultServiceBuilder<TService>(this);
    }

    public ApplicationSpace DeclareService<T>(ScopeOld scope, bool isStatic = false) where T : class, IService, new() =>
        DeclareService<T, T>(scope, isStatic);

    public ApplicationSpace DeclareService<T, T1>(ScopeOld scope, bool isStatic = false)
        where T : class, IService where T1 : IService, T, new()
    {
        CheckIsNotDisposed();
        var association = typeof(T);
        if (association == BaseType)
            throw new SpaceException($"Association cant be {nameof(IService)}.");

        if (StaticSpace != this && isStatic)
            return StaticSpace.DeclareService<T, T1>(scope, isStatic);

        var type = typeof(T1);
        if (_updateListener is null && typeof(IUpdatable).IsAssignableFrom(type))
            throw new SpaceException($"Space does not have Update Event. Service {type.Name} is {nameof(IUpdatable)}.");
        lock (_declaredServicesLock)
        {
            if (_servicesInitializers.ContainsKey(association))
                throw new SpaceException($"Service with type {type.Name} already declared.");
            var func = ServicesInitializer.CreateServiceInitializer(this, type, scope);
            var instanceData = new ServiceInitializationData(this, func, scope);
            _servicesInitializers.Add(association, instanceData);
            if (StaticSpace == this && isStatic)
                foreach (var space in Spaces)
                    space._servicesInitializers.Add(association, instanceData);
        }


        return this;
    }

    public T GetService<T>() where T : class, IService => (T)GetService(typeof(T));

    public IService GetService(Type type)
    {
        type.VerifyNotEquals(BaseType, $"Service association cant be service {nameof(IService)}.");
        return _InternalGetService(type);
    }


    public bool HasDeclaredService(Type type)
    {
        lock (_declaredServicesLock)
            return _servicesInitializers.ContainsKey(type);
    }

    public IList<Component> FilterComponents(Type baseType)
    {
        if (baseType.IsAbstract || baseType.IsInterface)
        {
            var subList = new List<IList<Component>>();
            lock (_componentsLock)
                foreach (var pair in _allComponents)
                {
                    if (!baseType.IsAssignableFrom(pair.Key))
                        continue;
                    subList.Add(pair.Value.AsReadOnly());
                }

            return new MultiList<Component>(subList);
        }

        lock (_componentsLock)
            if (_allComponents.TryGetValue(baseType, out var list))
                return list.AsReadOnly();
            else
                return [];
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


    internal IService _InternalGetService(Type association)
    {
        lock (_declaredServicesLock)
            if (!_servicesInitializers.TryGetValue(association, out var data))
                throw new SpaceException($"Association for {association.Name} is not declared.");
            else
                return data.InstanceFunc(data.Scope is ScopeOld.Singleton ? data.Owner : this);
    }

    internal void _InternalInstanceService(IService service)
    {
        lock (_servicesLock) _services.Add(service);
        if (service is IUpdatable)
            lock (_updatableLock)
                _updatableServices.Add(service);
    }

    internal Component _InternalDeclareComponent(UObject obj, Type componentType)
    {
        CheckIsNotDisposed();
        var component = ComponentsInitializer.CreateComponent(obj, componentType);
        List<Component>? list;
        lock (_componentsLock)
            if (!_allComponents.TryGetValue(componentType, out list))
            {
                list = [];
                _allComponents.Add(componentType, list);
            }

        list.Add(component);
        return component;
    }

    internal void _InternalDestroyComponent(Component component)
    {
        CheckIsNotDisposed();
        lock (_componentsLock)
            Verify.Argument(_allComponents.TryGetValue(component.GetType(), out var list) && list.Remove(component),
                "Something was wrong.");
    }

    internal void _InternalDestroyObject(UObject obj)
    {
        CheckIsNotDisposed();
        lock (_objectsLock)
            _objects.Remove(obj);
    }


    public void DestroyService(IService service)
    {
        GC.SuppressFinalize(service);
        if (service is IUpdatable)
            lock (_updatableLock)
                _updatableServices.Remove(service);
    }

    public static ApplicationSpace Create() => new(null, Type.EmptyTypes);

    public static ApplicationSpace Create<T>() where T : Component =>
        new(null, [typeof(T)]);

    public static ApplicationSpace Create<T, T1>() where T : Component
        where T1 : Component => new(null, [typeof(T), typeof(T1)]);

    public static ApplicationSpace Create<T, T1, T2>() where T : Component
        where T1 : Component
        where T2 : Component => new(null, [typeof(T), typeof(T1), typeof(T2)]);


    public static ApplicationSpace Create(IEventAccess<IEventArgs> onUpdate) => new(onUpdate, Type.EmptyTypes);

    public static ApplicationSpace Create<T>(IEventAccess<IEventArgs> onUpdate) where T : Component =>
        new(onUpdate, [typeof(T)]);

    public static ApplicationSpace Create<T, T1>(IEventAccess<IEventArgs> onUpdate) where T : Component
        where T1 : Component => new(onUpdate, [typeof(T), typeof(T1)]);

    public static ApplicationSpace Create<T, T1, T2>(IEventAccess<IEventArgs> onUpdate) where T : Component
        where T1 : Component
        where T2 : Component => new(onUpdate, [typeof(T), typeof(T1), typeof(T2)]);
}*/