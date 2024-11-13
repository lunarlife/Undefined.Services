using Undefined.Services.Lambdas.Abstraction.Blocks.Variables;

namespace Undefined.Services.Lambdas.Declaration.Extensions;

public static class VarsExtensions
{
    public static void AssignTo(this IAssignable assignable, IStorable storable) => assignable.AssignTo(() => storable);
}