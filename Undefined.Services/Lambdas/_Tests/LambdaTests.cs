using Undefined.Services.Lambdas._Tests.Entries;
using Undefined.Services.Lambdas.Abstraction;
using Undefined.Services.Lambdas.Abstraction.Methods;

namespace Undefined.Services.Lambdas._Tests;

public class LambdaTests
{
    public LambdaTests()
    {

        
        IQueryNext<string, int> next = null;
        Query.MakeQuery<char>(next)
        
        IQuery<string, int, char> query = null;

        var build = query.Query("lol").Query(1).Pull();

        Lambda.Action<int>(Console.WriteLine).;
        IDelegate<int, string> del;
        del.Invoke()
    }

    public (int i, string a) Test()
    {
    }
}