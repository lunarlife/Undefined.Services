using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Undefined.Services;

internal static class RuntimeUtils
{
    private static readonly MethodInfo GetUninitializedObjectMethod = typeof(RuntimeHelpers).GetMethod(
        "GetUninitializedObject",
        BindingFlags.Public | BindingFlags.Static)!;

    public static void EmitGetUninitializedObject(this ILGenerator generator, Type type)
    {
        generator.EmitObj(type);

        generator.Emit(OpCodes.Call, GetUninitializedObjectMethod);
    }

    public static void EmitObj(this ILGenerator generator, object obj)
    {
        var handle = GCHandle.Alloc(obj);
        var ptr = GCHandle.ToIntPtr(handle);
        if (IntPtr.Size == 4)
            generator.Emit(OpCodes.Ldc_I4, ptr.ToInt32());
        else
            generator.Emit(OpCodes.Ldc_I8, ptr.ToInt64());
        generator.Emit(OpCodes.Ldobj, obj.GetType());
    }
}