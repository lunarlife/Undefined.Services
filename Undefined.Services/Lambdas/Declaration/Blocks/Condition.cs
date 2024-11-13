using Undefined.Services.Lambdas.Abstraction.Blocks;
using Undefined.Services.Lambdas.Abstraction.Values;

namespace Undefined.Services.Lambdas.Declaration.Blocks;

public static class Condition
{
    public static ICondition True { get; }
    public static ICondition False { get; }
    
    public static ICondition Equals(IValueHandleAny first, IRuntimeValue<> second)
    {
        
    }
}