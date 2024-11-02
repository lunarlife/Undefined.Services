using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Undefined.Verifying;

namespace Undefined.Services;

public static class ListExtensions
{
    private static class ArrayAccessor<T>
    {
        private const string LIST_ITEMS_FIELD_NAME = "_items";
        private const string LIST_SIZE_FIELD_NAME = "_size";
        public static readonly Func<List<T>, T[]> ItemsGetter;
        public static readonly Action<List<T>, T[]> ItemsSetter;

        static ArrayAccessor()
        {
            var itemsField =
                typeof(List<T>).GetField(LIST_ITEMS_FIELD_NAME, BindingFlags.NonPublic | BindingFlags.Instance)!;
            {
                var dm = new DynamicMethod("list_get_items", MethodAttributes.Static | MethodAttributes.Public,
                    CallingConventions.Standard, typeof(T[]),
                    [typeof(List<T>)], typeof(ArrayAccessor<T>), true);
                var il = dm.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, itemsField);
                il.Emit(OpCodes.Ret);
                ItemsGetter = (Func<List<T>, T[]>)dm.CreateDelegate(typeof(Func<List<T>, T[]>));
            }
            {
                var listSizeField = typeof(List<T>).GetField(LIST_SIZE_FIELD_NAME,
                    BindingFlags.NonPublic | BindingFlags.Instance)!;

                var dm = new DynamicMethod("list_set_items", MethodAttributes.Static | MethodAttributes.Public,
                    CallingConventions.Standard, null,
                    [typeof(List<T>), typeof(T[])], typeof(ArrayAccessor<T>), true);
                var il = dm.GetILGenerator();


                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Stfld, itemsField);


                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldlen);
                il.Emit(OpCodes.Conv_I4);
                il.Emit(OpCodes.Stfld, listSizeField);

                il.Emit(OpCodes.Ret);
                ItemsSetter = (Action<List<T>, T[]>)dm.CreateDelegate(typeof(Action<List<T>, T[]>));
            }
        }
    }

    public static T[] GetInternalArray<T>(this List<T> list) => ArrayAccessor<T>.ItemsGetter(list);
    public static void SetInternalArray<T>(this List<T> list, T[] array) => ArrayAccessor<T>.ItemsSetter(list, array);

    public static List<TDestination> CloneAs<TSource, TDestination>(this List<TSource> source, int index) => source.CloneAs<TSource, TDestination>(index, source.Count - index);
    public static List<TDestination> CloneAs<TSource, TDestination>(this List<TSource> source) => source.CloneAs<TSource, TDestination>(0, source.Count);

    public static List<TSource> Clone<TSource>(this List<TSource> source) => CloneAs<TSource, TSource>(source, 0, source.Count);

    public static unsafe List<TDestination> CloneAs<TSource, TDestination>(this List<TSource> source, int index,
        int count)
    {
        Verify.Array(source, index, count);
        var array = new TDestination[count];
        var byteCount = count * IntPtr.Size;
        
        var src = Unsafe.AsPointer(ref Unsafe.AddByteOffset(
            ref new Span<TSource>(GetInternalArray(source)).GetPinnableReference(), (uint)(index * IntPtr.Size)));
        var dst = Unsafe.AsPointer(ref new Span<TDestination>(array).GetPinnableReference());
        Unsafe.CopyBlockUnaligned(dst, src, (uint)byteCount);
        return array.AsListKeepsReference();
    }


    public static List<T> AsListKeepsReference<T>(this T[] array)
    {
        var list = new List<T>();
        list.SetInternalArray(array);
        return list;
    }
}