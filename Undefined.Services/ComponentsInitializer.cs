using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using Undefined.Services.Exceptions;

namespace Undefined.Services;

internal class ComponentsInitializer
{
    private static readonly MethodInfo ComponentInitMethod =
        typeof(ComponentBase).GetMethod("Init", BindingFlags.NonPublic | BindingFlags.Instance)!;

    private readonly ConcurrentDictionary<Type, ComponentInitFunc> _componentsTypes = [];

    public ComponentBase CreateComponent(SObject obj, Type componentType) =>
        _componentsTypes.GetOrAdd(componentType, _ =>
        {
            if (componentType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .FirstOrDefault(c => c.GetParameters().Length == 0) is not { } ctor)
                throw new ComponentException($"Component {componentType.Name} does not have empty constructor.");
            var method = new DynamicMethod($"{componentType.Name}_component_init", typeof(ComponentBase),
                [typeof(SObject)],
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
        })(obj);

    private delegate ComponentBase ComponentInitFunc(SObject obj);
}