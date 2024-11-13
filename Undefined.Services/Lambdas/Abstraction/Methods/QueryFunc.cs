namespace Undefined.Services.Lambdas.Abstraction.Methods;

public class QueryEndPoint(IDelegate del) : IQueryEndPoint
{
    public void Build()
    {
        del.
    }
}

public class QueryEndPoint<TPull> : QueryEndPoint, IQueryEndPoint<TPull>
{
    public TPull Pull()
    {
    }
}

public class QueryCurrent<TCurrent> : IQueryCurrent<TCurrent>
{
}

public class QueryNext<TCurrent, TNext>(TNext next, IDelegate finalizer)
    : QueryCurrent<TCurrent>, IQueryNext<TCurrent, TNext>
    where TNext : IQueryLink
{
    public TNext Next { get; } = next;

    public TNext Query(Func<TCurrent> value)
    {
        return Next;
    }
}

public class QueryFunc<T, TFinal>(Delegate del)
    : QueryNext<T, IQueryEndPoint<TFinal>>(new QueryEndPoint<TFinal>(), del), IQuery<T, TFinal>
{
}

public class QueryFunc<T, T1, TFinal>(Action<Func<T1>> action)
    : QueryNext<T, IQuery<T1, TFinal>>(new QueryFunc<T1, TFinal>(), action), IQuery<T, T1, TFinal>
{
}

public record QueryFunc<T, T1, T2, TFinal>()
    : QueryNext<T, IQuery<T1, T2, TFinal>>(new QueryFunc<T1, T2, TFinal>()), IQuery<T, T1, T2, TFinal>;

public record QueryFunc<T, T1, T2, T3, TFinal>()
    : QueryNext<T, IQuery<T1, T2, T3, TFinal>>(new QueryFunc<T1, T2, T3, TFinal>()), IQuery<T, T1, T2, T3, TFinal>;

public record QueryFunc<T, T1, T2, T3, T4, TFinal>()
    : QueryNext<T, IQuery<T1, T2, T3, T4, TFinal>>(new QueryFunc<T1, T2, T3, T4, TFinal>()),
        IQuery<T, T1, T2, T3, T4, TFinal>;

public static class Query
{
    public static IQuery<T, TFinal> MakeQuery<T, TFinal>(Func<T, TFinal> finalizer)
    {
        return new QueryFunc<T, TFinal>(finalizer);
    }
}