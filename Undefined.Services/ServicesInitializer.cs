/*using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Undefined.Services.Application;
using Undefined.Services.Exceptions;

namespace Undefined.Services;


public delegate void ServiceUpdateFunc(IService service);

internal class ServicesInitializer
{
    private readonly Dictionary<Type, ServiceUpdateFunc> _servicesUpdaters = [];


    private static readonly MethodInfo InitializerDestroyServiceAndGetNewMethod = typeof(ServicesInitializer)
        .GetMethod("DestroyServiceAndGetNew", BindingFlags.Public | BindingFlags.Static)!;

    public ServiceInstallFunc CreateServiceInitializer(ApplicationSpace space, Type type,
        ScopeOld scope)
    {
        if (type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(c => c.GetParameters().Length == 0) is not { } ctor)
            throw new SpaceException($"Service type {type.Name} does not have an empty constructor.");
        ServiceInstallFunc func;
        var updateFields = new List<FieldData>();
        switch (scope)
        {
            case ScopeOld.Singleton:
            {
                var service = (IService)RuntimeHelpers.GetUninitializedObject(type);
                var fields = IterateFields(space, type).ToArray();
                foreach (var data in fields)
                {
                    if (data.FilterUpdateType != FilterUpdateType.EveryTick) continue;
                    updateFields.Add(data);
                }

                func = initSpace =>
                {
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                    if (service.Space is not null) return service;
                    ServiceSpaceField.SetValue(service, initSpace);
                    foreach (var data in fields)
                    {
                        var field = data.Field;
                        if (data.FilterUpdateType == FilterUpdateType.EveryTick)
                            continue;

                        if (data.IsFilter)
                        {
                            var filter = initSpace._InternalGetFilter(field.FieldType.GetGenericArguments().First());
                            field.SetValue(service,
                                GetFilterConstructor(field.FieldType).Invoke([filter]));
                        }
                        else
                            field.SetValue(service, initSpace._InternalGetService(field.FieldType));
                    }

                    try
                    {
                        ctor.Invoke(service, null);
                    }
                    catch (Exception e)
                    {
                        throw e is TargetInvocationException tie ? tie.InnerException ?? e : e;
                    }

                    initSpace._InternalInstanceService(service);
                    return service;
                };
                break;
            }
            case ScopeOld.Factory:
            {
                var method = new DynamicMethod($"{type.Name}_service_init", typeof(IService), [typeof(ApplicationSpace)],
                    false);
                var generator = method.GetILGenerator();
                generator.DeclareLocal(type);
                generator.DeclareLocal(typeof(IService));

                generator.DeclareLocal(SpaceGetFilterMethod.ReturnType);
                generator.DeclareLocal(typeof(IFilter));

                generator.Emit(OpCodes.Nop);

                generator.EmitGetUninitializedObject(type);
                generator.Emit(OpCodes.Stloc_0);
                generator.Emit(OpCodes.Ldloc_0);

                // set the Space property to service 
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Stfld, ServiceSpaceField);


                // fill all fields
                foreach (var data in IterateFields(space, type))
                {
                    var field = data.Field;
                    if (data.FilterUpdateType == FilterUpdateType.EveryTick)
                    {
                        updateFields.Add(data);
                        continue;
                    }

                    if (data.IsFilter)
                    {
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.EmitObj(field.FieldType.GetGenericArguments().First());
                        generator.Emit(OpCodes.Call, SpaceGetFilterMethod);
                        generator.Emit(OpCodes.Newobj,
                            GetFilterConstructor(field.FieldType));
                        generator.Emit(OpCodes.Stloc_2);

                        generator.Emit(OpCodes.Ldloc_0);
                        generator.Emit(OpCodes.Ldloc_2);
                        generator.Emit(OpCodes.Stfld, field);
                    }
                    else

                    {
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.EmitObj(field.FieldType);
                        generator.Emit(OpCodes.Call, SpaceGetServiceMethod);
                        generator.Emit(OpCodes.Stloc_1);
                        generator.Emit(OpCodes.Ldloc_0);
                        generator.Emit(OpCodes.Ldloc_1);
                        generator.Emit(OpCodes.Stfld, field);
                    }
                }

                // call the constructor
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Call, ctor);

                // call the instance method in class Space
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Call, SpaceServiceInstanceMethod);

                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Ret);
                func = (ServiceInstallFunc)method.CreateDelegate(typeof(ServiceInstallFunc));
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(scope), scope, null);
        }

        if (updateFields.Count != 0)
        {
            var updateMethod = new DynamicMethod($"{type.Name}_service_update", null,
                [typeof(IService), typeof(ApplicationSpace)],
                false);
            var updateGenerator = updateMethod.GetILGenerator();

            updateGenerator.Emit(OpCodes.Nop);

            updateGenerator.DeclareLocal(typeof(IFilter));
            updateGenerator.DeclareLocal(SpaceGetFilterMethod.ReturnType);
            updateGenerator.DeclareLocal(typeof(IService));
            foreach (var data in updateFields)
            {
                var field = data.Field;
                if (data.IsFilter)
                {
                    updateGenerator.Emit(OpCodes.Ldarg_1);
                    updateGenerator.EmitObj(field.FieldType.GetGenericArguments().First());
                    updateGenerator.Emit(OpCodes.Call, SpaceGetFilterMethod);
                    updateGenerator.Emit(OpCodes.Newobj,
                        field.FieldType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).First());
                    updateGenerator.Emit(OpCodes.Stloc_0);

                    updateGenerator.Emit(OpCodes.Ldarg_0);
                    updateGenerator.Emit(OpCodes.Ldloc_0);
                    updateGenerator.Emit(OpCodes.Stfld, field);
                }
                else
                {
                    updateGenerator.Emit(OpCodes.Ldarg_0);
                    updateGenerator.Emit(OpCodes.Ldfld, field);
                    updateGenerator.EmitObj(field.FieldType);
                    updateGenerator.Emit(OpCodes.Call, InitializerDestroyServiceAndGetNewMethod);
                    updateGenerator.Emit(OpCodes.Stloc_2);

                    updateGenerator.Emit(OpCodes.Ldarg_0);
                    updateGenerator.Emit(OpCodes.Ldloc_2);
                    updateGenerator.Emit(OpCodes.Stfld, field);
                }
            }

            updateGenerator.Emit(OpCodes.Ret);

            var dg = (Action<IService, ApplicationSpace>)updateMethod.CreateDelegate(
                typeof(Action<IService, ApplicationSpace>));
            _servicesUpdaters.Add(type, service => dg(service, service.Space));
        }

        return func;
    }

    private static ConstructorInfo GetFilterConstructor(Type type) =>
        type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).First();

    public bool TryGetServiceUpdateFunc(Type type, out ServiceUpdateFunc? func) =>
        _servicesUpdaters.TryGetValue(type, out func);

    private static IService DestroyServiceAndGetNew(IService? service, Type type)
    {
        DestroyService(service);
        return service?.Space().GetService(type)?? throw new SpaceException("");
    }

    public static void DestroyService(IService? service)
    {
        if (service is not null)
        {
            service.Space().DestroyService(service);
        }

        switch (service)
        {
            case IDisposable disposable:
                disposable.Dispose();
                break;
            case IAsyncDisposable asyncDisposable:
                asyncDisposable.DisposeAsync();
                break;
        }
    }

    private static IEnumerable<FieldData> IterateFields(ApplicationSpace space, Type type)
    {
        var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public |
                                    BindingFlags.Static);
        var isUpdatable = typeof(IUpdatable).IsAssignableFrom(type);
        foreach (var field in fields)
        {
            if (field.GetCustomAttribute<FilterAttribute>() is not { } attribute) continue;
            if (field.IsStatic || !field.IsInitOnly || !field.IsPrivate)
                throw new SpaceException(
                    $"Fields with attribute {nameof(FilterAttribute)} must be 'private readonly' [Field {field.Name} in type {type.Name}].");
            var fieldType = field.FieldType;
            if (!isUpdatable && attribute.UpdateType == FilterUpdateType.EveryTick)
                throw new FilterException(
                    $"Fill type {FilterUpdateType.EveryTick} is not allowed without interface {nameof(IUpdatable)} [Field {field.Name} in type {type.Name}].");

            if (space.HasDeclaredService(fieldType))
                yield return new FieldData(field, attribute.UpdateType, false);
            else if (typeof(IFilter).IsAssignableFrom(fieldType))
                yield return new FieldData(field, attribute.UpdateType, true);
            else
                throw new SpaceException(
                    $"Fields with attribute {nameof(FilterAttribute)} must be declared in {nameof(ApplicationSpace)} [Field {field.Name} in type {type.Name}].");
        }
    }

    private struct FieldData
    {
        public FieldInfo Field { get; }
        public FilterUpdateType FilterUpdateType { get; }
        public bool IsFilter { get; }

        public FieldData(FieldInfo field, FilterUpdateType filterUpdateType, bool isFilter)
        {
            Field = field;
            FilterUpdateType = filterUpdateType;
            IsFilter = isFilter;
        }
    }
}*/