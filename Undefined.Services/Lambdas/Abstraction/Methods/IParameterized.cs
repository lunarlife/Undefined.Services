using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Undefined.Services.Lambdas.Abstraction.Values;

namespace Undefined.Services.Lambdas.Abstraction.Methods;


public struct Prms<T, T1> : IRequiredParameters<>
{
    public Func<T> Builder { get; }

    public static implicit operator Prms<T, T1>(ValueTuple<T, T1> parameters) =>
        Unsafe.As<ValueTuple<T, T1>, Prms<T, T1>>(ref parameters);
}