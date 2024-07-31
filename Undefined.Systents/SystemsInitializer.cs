using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Undefined.Systents.Exceptions;
using Undefined.Systents.Filters;

namespace Undefined.Systents;

public delegate SystemBase SystemInstanceFunc(Space space);

public delegate void SystemUpdateFunc(SystemBase system);

internal class SystemsInitializer
{
    private static readonly MethodInfo InitializerSystemDisposeMethod =
        typeof(SystemsInitializer).GetMethod(nameof(DisposeSystem), BindingFlags.Static | BindingFlags.NonPublic)!;

    private static readonly MethodInfo SpaceSystemInstanceMethod =
        typeof(Space).GetMethod("_InternalInstanceSystem", BindingFlags.Instance | BindingFlags.NonPublic)!;

    private static readonly MethodInfo SpaceGetFilterMethod =
        typeof(Space).GetMethod("_InternalGetFilter", BindingFlags.Instance | BindingFlags.NonPublic)!;

    private static readonly FieldInfo SystemSpaceField =
        typeof(SystemBase).GetField("<Space>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!;

    private static readonly FieldInfo FilterListField =
        typeof(Filter<>).GetField("_list", BindingFlags.Instance | BindingFlags.NonPublic)!;

    private static readonly MethodInfo SpaceGetSystemMethod = typeof(Space).GetMethod(
        "_InternalGetSystem",
        BindingFlags.NonPublic | BindingFlags.Instance)!;

    private readonly Dictionary<Type, SystemUpdateFunc> _systemsUpdaters = [];

    public SystemInstanceFunc CreateSystemInitializer(Space space, Type type,
        SystemScope scope)
    {
        if (type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(c => c.GetParameters().Length == 0) is not { } ctor)
            throw new SpaceException($"System type {type.Name} does not have an empty constructor.");
        SystemInstanceFunc func;
        var updateFields = new List<FieldData>();
        switch (scope)
        {
            case SystemScope.Instance:
            {
                var system = (SystemBase)RuntimeHelpers.GetUninitializedObject(type);
                var fields = IterateFields(space, type).ToArray();
                foreach (var data in fields)
                {
                    if (data.FillType != FillType.Update) continue;
                    updateFields.Add(data);
                }

                func = initSpace =>
                {
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                    if (system.Space is not null) return system;
                    SystemSpaceField.SetValue(system, initSpace);
                    foreach (var data in fields)
                    {
                        var field = data.Field;
                        if (data.FillType == FillType.Update)
                            continue;

                        if (data.IsFilter)
                        {
                            var filter = initSpace._InternalGetFilter(field.FieldType.GetGenericArguments().First());
                            field.SetValue(system,
                                GetFilterConstructor(field.FieldType).Invoke([filter]));
                        }
                        else
                            field.SetValue(system, initSpace._InternalGetSystem(field.FieldType));
                    }

                    ctor.Invoke(system, null);
                    initSpace._InternalInstanceSystem(system);
                    return system;
                };
                break;
            }
            case SystemScope.Factory:
            {
                var method = new DynamicMethod($"{type.Name}_system_init", typeof(SystemBase), [typeof(Space)],
                    false);
                var generator = method.GetILGenerator();
                generator.DeclareLocal(type);
                generator.DeclareLocal(typeof(SystemBase));

                generator.DeclareLocal(SpaceGetFilterMethod.ReturnType);
                generator.DeclareLocal(typeof(IFilter));

                generator.Emit(OpCodes.Nop);

                generator.EmitGetUninitializedObject(type);
                generator.Emit(OpCodes.Stloc_0);
                generator.Emit(OpCodes.Ldloc_0);

                // set the Space property to system 
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Stfld, SystemSpaceField);


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
                        generator.Emit(OpCodes.Call, SpaceGetSystemMethod);
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
                generator.Emit(OpCodes.Call, SpaceSystemInstanceMethod);

                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Ret);
                func = (SystemInstanceFunc)method.CreateDelegate(typeof(SystemInstanceFunc));
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(scope), scope, null);
        }

        if (updateFields.Count != 0)
        {
            var updateMethod = new DynamicMethod($"{type.Name}_system_update", null, [typeof(SystemBase)],
                false);
            var updateGenerator = updateMethod.GetILGenerator();

            updateGenerator.Emit(OpCodes.Nop);

            updateGenerator.DeclareLocal(typeof(IFilter));
            updateGenerator.DeclareLocal(SpaceGetFilterMethod.ReturnType);
            updateGenerator.DeclareLocal(typeof(Space));
            updateGenerator.DeclareLocal(typeof(SystemBase));

            updateGenerator.Emit(OpCodes.Ldarg_0);
            updateGenerator.Emit(OpCodes.Ldfld, SystemSpaceField); // get space from system
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
                    updateGenerator.Emit(OpCodes.Call, InitializerSystemDisposeMethod);

                    updateGenerator.Emit(OpCodes.Ldloc_2);
                    updateGenerator.EmitObj(field.FieldType);
                    updateGenerator.Emit(OpCodes.Call, SpaceGetSystemMethod);
                    updateGenerator.Emit(OpCodes.Stloc_3);

                    updateGenerator.Emit(OpCodes.Ldarg_0);
                    updateGenerator.Emit(OpCodes.Ldloc_3);
                    updateGenerator.Emit(OpCodes.Stfld, field);
                }
            }

            updateGenerator.Emit(OpCodes.Ret);
            var updateFunc = (SystemUpdateFunc)updateMethod.CreateDelegate(typeof(SystemUpdateFunc));
            _systemsUpdaters.Add(type, updateFunc);
        }

        return func;
    }

    private static ConstructorInfo GetFilterConstructor(Type type) =>
        type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).First();

    public bool TryGetSystemUpdateFunc(Type type, out SystemUpdateFunc? func) =>
        _systemsUpdaters.TryGetValue(type, out func);

    private static void DisposeSystem(SystemBase? system)
    {
        system?.Space._InternalDisposeSystem(system);
        switch (system)
        {
            case IDisposable disposable:
                disposable.Dispose();
                break;
            case IAsyncDisposable asyncDisposable:
                asyncDisposable.DisposeAsync();
                break;
        }
    }

    private static IEnumerable<FieldData> IterateFields(Space space, Type type)
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

            if (space.HasDeclaredSystem(fieldType))
                yield return new FieldData(field, attribute.Type, false);
            else if (typeof(IFilter).IsAssignableFrom(fieldType))
                yield return new FieldData(field, attribute.Type, true);
            else
                throw new SpaceException(
                    $"Fields with attribute {nameof(FillAttribute)} must be declared in {nameof(Space)} [Field {field.Name} in type {type.Name}].");
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