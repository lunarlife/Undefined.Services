using System.Reflection;
using System.Reflection.Emit;
using Undefined.Services.Application;
using Undefined.Services.Exceptions;

namespace Undefined.Services.Scopes;

public class DefaultServiceInstaller : IServiceInstaller
{
    private static readonly MethodInfo SpaceServiceInstanceMethod =
        typeof(ApplicationSpace).GetMethod("_InternalInstanceService", BindingFlags.Instance | BindingFlags.NonPublic)!;

    private static readonly MethodInfo SpaceGetFilterMethod =
        typeof(ApplicationSpace).GetMethod("_InternalGetFilter", BindingFlags.Instance | BindingFlags.NonPublic)!;

    private static readonly MethodInfo SpaceGetServiceMethod = typeof(ApplicationSpace).GetMethod(
        "_InternalGetService",
        BindingFlags.NonPublic | BindingFlags.Instance)!;

    private static readonly MethodInfo ResolveAndDisposeOldServiceMethod = typeof(DefaultServiceInstaller).GetMethod(
        "_InternalGetService",
        BindingFlags.NonPublic | BindingFlags.Static)!;

    public ServiceInstallFunc GenerateInstallFunc(Type serviceType)
    {
        var initMethod = new DynamicMethod($"{serviceType.Name}_service_init", typeof(IService),
            [typeof(Scope)],
            false);
        var ig = initMethod.GetILGenerator();
        var resolveMethod = new DynamicMethod($"{serviceType.Name}_service_resolve", null, [typeof(IService)],
            false);
        var rg = initMethod.GetILGenerator();
        ig.DeclareLocal(serviceType);
        ig.DeclareLocal(typeof(IService));

        ig.DeclareLocal(SpaceGetFilterMethod.ReturnType);
        ig.DeclareLocal(typeof(IFilter));

        ig.Emit(OpCodes.Nop);

        ig.EmitGetUninitializedObject(serviceType);
        ig.Emit(OpCodes.Stloc_0);

        // fill all fields
        foreach (var data in IterateFields(serviceType))
        {
            var field = data.Field;

            if (data.Type == FieldType.Service)
            {
                ig.Emit(OpCodes.Ldarg_0);
                ig.Emit(OpCodes.Ldfld, data.Field);
                ig.Emit(OpCodes.Ldarg_0);
                ig.Emit(OpCodes.Call, ResolveAndDisposeOldServiceMethod);
                continue;
            }



            if (data.Type == FieldType.Filter)
            {
                if (data.FilterUpdateType == FilterUpdateType.EveryTick)
                {
                    continue;
                }
                ig.Emit(OpCodes.Ldarg_0);
                ig.EmitObj(field.FieldType.GetGenericArguments().First());
                ig.Emit(OpCodes.Call, SpaceGetFilterMethod);
                ig.Emit(OpCodes.Newobj,
                    GetFilterConstructor(field.FieldType));
                ig.Emit(OpCodes.Stloc_2);

                ig.Emit(OpCodes.Ldloc_0);
                ig.Emit(OpCodes.Ldloc_2);
                ig.Emit(OpCodes.Stfld, field);
            }
            else

            {
                ig.Emit(OpCodes.Ldarg_0);
                ig.EmitObj(field.FieldType);
                ig.Emit(OpCodes.Call, SpaceGetServiceMethod);
                ig.Emit(OpCodes.Stloc_1);
                ig.Emit(OpCodes.Ldloc_0);
                ig.Emit(OpCodes.Ldloc_1);
                ig.Emit(OpCodes.Stfld, field);
            }
        }

        // call the constructor
        ig.Emit(OpCodes.Ldloc_0);
        ig.Emit(OpCodes.Call, ctor);

        // call the instance method in class Space
        ig.Emit(OpCodes.Ldarg_0);
        ig.Emit(OpCodes.Ldloc_0);
        ig.Emit(OpCodes.Call, SpaceServiceInstanceMethod);

        ig.Emit(OpCodes.Ldloc_0);
        ig.Emit(OpCodes.Ret);
        func = (ServiceInstallFunc)initMethod.CreateDelegate(typeof(ServiceInstallFunc));
    }


    private static void UpdateFilter(IFilter filter)
    {
        filter.Update();
    }

    private static ConstructorInfo GetFilterConstructor(Type type) =>
        type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).First();
    
    private static IService ResolveAndDisposeOldService(IService? oldService, Scope scope)
    {
        if (oldService is not null)
        {
            scope.ReleaseService(service);
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
    
    

    private static IEnumerable<FieldData> IterateFields(Type type)
    {
        var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public |
                                    BindingFlags.Static);
        var isUpdatable = typeof(IUpdatable).IsAssignableFrom(type);
        foreach (var field in fields)
        {
            if (field.GetCustomAttribute<FilterAttribute>() is not { } attribute) continue;
            if (field.IsStatic || !field.IsInitOnly || !field.IsPrivate)
                throw new SpaceException(
                    $"Fields with attribute {nameof(FilterAttribute)} must be 'private readonly' [Field {field.Name} in type {field.DeclaringType?.Name}].");
            var fieldType = field.FieldType;
            if (!isUpdatable && attribute.UpdateType == FilterUpdateType.EveryTick)
                throw new FilterException(
                    $"Fill type {FilterUpdateType.EveryTick} is not allowed without interface {nameof(IUpdatable)} [Field {field.Name} in type {type.Name}].");

            if (scope.HasDeclaredService(fieldType))
                yield return new FieldData(field, attribute.UpdateType, FieldType.Service);
            else if (typeof(IFilter).IsAssignableFrom(fieldType))
                yield return new FieldData(field, attribute.UpdateType, FieldType.Filter);
            else
                throw new SpaceException(
                    $"Fields with attribute {nameof(FilterAttribute)} must be declared in {nameof(ApplicationSpace)} [Field {field.Name} in type {type.Name}].");
        }
    }
}