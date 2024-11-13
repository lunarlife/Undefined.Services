using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Undefined.Services.Lambdas.Abstraction.Values;

namespace Undefined.Services.Lambdas.Abstraction.Methods;

public interface IParametersDeclare<T> where T : struct
{
    public Func<T>
}

public interface IParameterized
{
}

public interface IParameterized<T> : IParameterized, IParameterBuilder<T>

{
}

public interface IParameterized<T, T1> : IParameterized, IParameterBuilder<T>
{
}

public struct Prms<T, T1> : IParametersDeclare<Prms<T, T1>>
{
    public Func<T> Builder { get; }

    public static implicit operator Prms<T, T1>(ValueTuple<T, T1> parameters) =>
        Unsafe.As<ValueTuple<T, T1>, Prms<T, T1>>(ref parameters);
}