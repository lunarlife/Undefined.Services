using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using Undefined.Services.Entities;
using Undefined.Services.Entities.Components;
using Undefined.Services.Exceptions;

namespace Undefined.Services;

internal record ComponentInitializeData(Entity Entity, bool IsDefault, bool IsImmutable);

internal class ComponentsInitializer

{
    private delegate Component ComponentInitFunc(ComponentInitializeData obj);


    private static readonly MethodInfo ComponentInitMethod =
        typeof(Component).GetMethod(Component.INITIALIZE_METHOD_NAME, BindingFlags.NonPublic | BindingFlags.Instance)!;

    private static readonly Type[] ComponentInitFuncParameters = [typeof(ComponentInitializeData)];


    private readonly ConcurrentDictionary<Type, ComponentInitFunc> _componentsTypes = [];

    public Component CreateComponent(Type componentType, ComponentInitializeData data) =>
        _componentsTypes.GetOrAdd(componentType, _ =>
        {
            if (componentType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .FirstOrDefault(c => c.GetParameters().Length == 0) is not { } ctor)
                throw new ComponentException($"Component {componentType.Name} does not have empty constructor.");
            var method = new DynamicMethod($"component_init_{componentType.Name}", typeof(Component),
                ComponentInitFuncParameters,
                false);
            var generator = method.GetILGenerator();

            generator.DeclareLocal(componentType);

            generator.Emit(OpCodes.Nop);

            generator.EmitGetUninitializedObject(componentType);
            generator.Emit(OpCodes.Stloc_0);
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Call, ComponentInitMethod);
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Call, ctor);
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ret);
            return (ComponentInitFunc)method.CreateDelegate(typeof(ComponentInitFunc));
        })(data);
}