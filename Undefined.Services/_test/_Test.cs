using System.Reflection.Emit;
using Undefined.Services.Lambdas.Abstraction;
using Undefined.Services.Lambdas.Abstraction.Blocks.Variables;
using Undefined.Services.Lambdas.Abstraction.Handles;
using Undefined.Services.Lambdas.Abstraction.Utils;
using Undefined.Services.Lambdas.Abstraction.Values;
using Undefined.Services.Lambdas.Declaration;
using Undefined.Services.Lambdas.Declaration.Blocks.Variables;
using Undefined.Services.Lambdas.Declaration.Compilation;
using Undefined.Services.Lambdas.Declaration.Extensions.BlockBodyBuilder;

namespace Undefined.Services._test;

public class _Test
{
    public void LambdaBuild()
    {
        var stack = new Stack<int>();
        LambdaBuilder.BuildLambda(lb => lb
            .DefaultBlock(body =>
            {
                body.New(typeof(TestClass)).AssignTo(body.Vars().FirstOrDeclare);
                //  body
                //  .Invoke(method)  === IMethodHandleBuilder : IHandleBuilder
                //  .Parameters(...)
                //      === hasReturnValue ? value : finish  
                //      
                // .Build();
                return body.Build();
            })
            .DefaultBlock(b => b.Build())
            .Build()).Compile(new RuntimeMethodBodyConstructor());

        Lambdas.Abstraction.Values.IRuntimeValue<bool> value = default;
        AddTest(value)
    }

    public void AddTest(Handle<bool> handle)
    {
        var value = handle.Handleable;
        
    }

    public void Example()
    {
        {
            var v = new TestClass();
        }
        {
        }
    }

    public void TestAction(Action<ITest> test)
    {
    }
}

public static class EXT
{
    public static void ActionTest(this Action a)
    {
        Console.WriteLine(a);
    }
}

public interface ITest : IService
{
}

public class TestClass : ITest
{
}