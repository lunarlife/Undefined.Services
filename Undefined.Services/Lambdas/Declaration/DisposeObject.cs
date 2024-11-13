namespace Undefined.Services.Lambdas.Declaration;

internal class DisposeObject : IDisposable
{
    public bool IsDisposed { get; private set; }

    public void CheckDisposed()
    {
        if (IsDisposed) throw new ObjectDisposedException("Object has been disposed.");
    }

    public void Dispose()
    {
        IsDisposed = true;
    }
}