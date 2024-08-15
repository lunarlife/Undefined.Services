using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Undefined.Services.Exceptions;

namespace Undefined.Services;

public delegate ServiceBase ServiceInstanceFunc(ServicesSpace space);

public delegate void ServiceUpdateFunc(ServiceBase service);

internal class ServicesInitializer
{
    private static readonly MethodInfo ServiceDestroyMethod =
        typeof(ServiceBase).GetMethod("Destroy", BindingFlags.Public | BindingFlags.Instance)!;

    private static readonly MethodInfo SpaceServiceInstanceMethod =
        typeof(ServicesSpace).GetMethod("_InternalInstanceService", BindingFlags.Instance | BindingFlags.NonPublic)!;

    private static readonly MethodInfo SpaceGetFilterMethod =
        typeof(ServicesSpace).GetMethod("_InternalGetFilter", BindingFlags.Instance | BindingFlags.NonPublic)!;

    private static readonly FieldInfo ServiceSpaceField =
        typeof(ServiceBase).GetField("<Space>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!;

    private static readonly FieldInfo FilterListField =
        typeof(Filter<>).GetField("_list", BindingFlags.Instance | BindingFlags.NonPublic)!;

    private static readonly MethodInfo SpaceGetServiceMethod = typeof(ServicesSpace).GetMethod(
        "_InternalGetService",
        BindingFlags.NonPublic | BindingFlags.Instance)!;

    private readonly Dictionary<Type, ServiceUpdateFunc> _servicesUpdaters = [];

    public ServiceInstanceFunc CreateServiceInitializer(ServicesSpace space, Type type,
        Scope scope)
    {
        if (type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(c => c.GetParameters().Length == 0) is not { } ctor)
            throw new SpaceException($"Service type {type.Name} does not have an empty constructor.");
        ServiceInstanceFunc func;
        var updateFields = new List<FieldData>();
        switch (scope)
        {
            case Scope.Singleton:
            {
                var service = (ServiceBase)RuntimeHelpers.GetUninitializedObject(type);
                var fields = IterateFields(space, type).ToArray();
                foreach (var data in fields)
                {
                    if (data.FillType != FillType.Update) continue;
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
                        if (data.FillType == FillType.Update)
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
            case Scope.Factory:
            {
                var method = new DynamicMethod($"{type.Name}_service_init", typeof(ServiceBase), [typeof(ServicesSpace)],
                    false);
                var generator = method.GetILGenerator();
                generator.DeclareLocal(type);
                generator.DeclareLocal(typeof(ServiceBase));

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
                    if (data.FillType == FillType.Update)
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
                func = (ServiceInstanceFunc)method.CreateDelegate(typeof(ServiceInstanceFunc));
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(scope), scope, null);
        }

        if (updateFields.Count != 0)
        {
            var updateMethod = new DynamicMethod($"{type.Name}_service_update", null, [typeof(ServiceBase)],
                false);
            var updateGenerator = updateMethod.GetILGenerator();

            updateGenerator.Emit(OpCodes.Nop);

            updateGenerator.DeclareLocal(typeof(IFilter));
            updateGenerator.DeclareLocal(SpaceGetFilterMethod.ReturnType);
            updateGenerator.DeclareLocal(typeof(ServicesSpace));
            updateGenerator.DeclareLocal(typeof(ServiceBase));

            updateGenerator.Emit(OpCodes.Ldarg_0);
            updateGenerator.Emit(OpCodes.Ldfld, ServiceSpaceField); // get space from service
            updateGenerator.Emit(OpCodes.Stloc_2);
            foreach (var data in updateFields)
            {
                var field = data.Field;
                if (data.IsFilter)
                {
                    updateGenerator.Emit(OpCodes.Ldloc_2);
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
                    updateGenerator.Emit(OpCodes.Call, ServiceDestroyMethod);

                    updateGenerator.Emit(OpCodes.Ldloc_2);
                    updateGenerator.EmitObj(field.FieldType);
                    updateGenerator.Emit(OpCodes.Call, SpaceGetServiceMethod);
                    updateGenerator.Emit(OpCodes.Stloc_3);

                    updateGenerator.Emit(OpCodes.Ldarg_0);
                    updateGenerator.Emit(OpCodes.Ldloc_3);
                    updateGenerator.Emit(OpCodes.Stfld, field);
                }
            }

            updateGenerator.Emit(OpCodes.Ret);
            var updateFunc = (ServiceUpdateFunc)updateMethod.CreateDelegate(typeof(ServiceUpdateFunc));
            _servicesUpdaters.Add(type, updateFunc);
        }

        return func;
    }

    private static ConstructorInfo GetFilterConstructor(Type type) =>
        type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).First();

    public bool TryGetServiceUpdateFunc(Type type, out ServiceUpdateFunc? func) =>
        _servicesUpdaters.TryGetValue(type, out func);

    internal static void DisposeService(ServiceBase? service)   
    {
        service?.Space._InternalDisposeService(service);
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

    private static IEnumerable<FieldData> IterateFields(ServicesSpace space, Type type)
    {
        var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public |
                                    BindingFlags.Static);
        var isUpdatable = typeof(IUpdatable).IsAssignableFrom(type);
        foreach (var field in fields)
        {
            if (field.GetCustomAttribute<FillAttribute>() is not { } attribute) continue;
            if (field.IsStatic || !field.IsInitOnly || !field.IsPrivate)
                throw new SpaceException(
                    $"Fields with attribute {nameof(FillAttribute)} must be 'private readonly' [Field {field.Name} in type {type.Name}].");
            var fieldType = field.FieldType;
            if (!isUpdatable && attribute.Type == FillType.Update)
                throw new FilterException(
                    $"Fill type {FillType.Update} is not allowed without interface {nameof(IUpdatable)} [Field {field.Name} in type {type.Name}].");

            if (space.HasDeclaredService(fieldType))
                yield return new FieldData(field, attribute.Type, false);
            else if (typeof(IFilter).IsAssignableFrom(fieldType))
                yield return new FieldData(field, attribute.Type, true);
            else
                throw new SpaceException(
                    $"Fields with attribute {nameof(FillAttribute)} must be declared in {nameof(ServicesSpace)} [Field {field.Name} in type {type.Name}].");
        }
    }

    private struct FieldData
    {
        public FieldInfo Field { get; }
        public FillType FillType { get; }
        public bool IsFilter { get; }

        public FieldData(FieldInfo field, FillType fillType, bool isFilter)
        {
            Field = field;
            FillType = fillType;
            IsFilter = isFilter;
        }
    }
}