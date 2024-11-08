using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Undefined.Services;

internal static class RuntimeUtils
{
    private unsafe delegate void* MakeRef(object obj);

    private static readonly MethodInfo GetUninitializedObjectMethod = typeof(RuntimeHelpers).GetMethod(
        "GetUninitializedObject",
        BindingFlags.Public | BindingFlags.Static)!;

    private static MakeRef MakeRefFunc { get; }

    static RuntimeUtils()
    {
        var dm = new DynamicMethod("make_ref", typeof(void*), [typeof(object)]);
        var il = dm.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ret);
        MakeRefFunc = (MakeRef)dm.CreateDelegate(typeof(MakeRef));
    }


    public static void EmitGetUninitializedObject(this ILGenerator generator, Type type)
    {
        generator.EmitPointer(type);

        generator.Emit(OpCodes.Call, GetUninitializedObjectMethod);
    }

    public static unsafe void EmitObjectPtrStraight(this ILGenerator generator, object obj, Type type) =>
        generator.EmitLoadPtr((nint)MakeRefFunc(obj), type);

    public static nint EmitObject(this ILGenerator generator, object obj)
    {
            
        var ptr = GCHandle.ToIntPtr(GCHandle.Alloc(obj));
        generator.EmitLoadPtr(ptr, obj.GetType());
        return ptr;
    }

    public static void EmitPtr(this ILGenerator generator, nint ptr)
    {
        if (nint.Size == sizeof(int))
            generator.Emit(OpCodes.Ldc_I4, ptr.ToInt32());
        else
            generator.Emit(OpCodes.Ldc_I8, ptr.ToInt64());
    }

    public static void EmitLoadPtr(this ILGenerator generator, nint ptr, Type type)
    {
        generator.EmitPtr(ptr);
        generator.Emit(OpCodes.Ldobj, type);
    }
}