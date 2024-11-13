using Undefined.Services.Lambdas.Abstraction.Blocks.Variables;
using Undefined.Services.Lambdas.Abstraction.Presentations;
using Undefined.Services.Lambdas.Abstraction.Stack;

namespace Undefined.Services.Lambdas.Abstraction.Blocks.Bodies;

public interface IBodyBuilder : IUCollection<Presentation>
{
    public IVariableCollection Vars { get; }

    public IBlockCollection Blocks { get; }
    public ILambdaBlock? BaseBlock { get; }
    public IBody Build();

}
