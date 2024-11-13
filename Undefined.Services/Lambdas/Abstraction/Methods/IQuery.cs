namespace Undefined.Services.Lambdas.Abstraction.Methods;

public interface IQueryLink;

public interface IQueryCurrent : IQueryLink
{
    public Type QueryType { get; }
}

public interface IQueryCurrent<TCurrent> : IQueryCurrent
{
    Type IQueryCurrent.QueryType => QueryType;
    public new Type QueryType => typeof(TCurrent);
}

public interface IQueryNext<TCurrent, out TNext> : IQueryCurrent<TCurrent>
{
    public TNext Query(Func<TCurrent> value);
}

public interface IQueryEndPoint : IQueryLink
{
    public void Build();
}

public interface IQueryEndPoint<out TPull> : IQueryEndPoint
{
    public TPull Pull();
}

public interface IQuery<T, out TFinal> : IQueryNext<T, IQueryEndPoint<TFinal>>;

public interface IQuery<T, T1, out TFinal> : IQueryNext<T, IQuery<T1, TFinal>>;

public interface IQuery<T, T1, T2, out TFinal> : IQueryNext<T, IQuery<T1, T2, TFinal>>;

public interface IQuery<T, T1, T2, T3, out TFinal> : IQueryNext<T, IQuery<T1, T2, T3, TFinal>>;

public interface IQuery<T, T1, T2, T3, T4, out TFinal>
    : IQueryNext<T, IQuery<T1, T2, T3, T4, TFinal>>;