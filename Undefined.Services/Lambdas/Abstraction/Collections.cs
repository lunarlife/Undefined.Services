using Undefined.Services.Lambdas.Abstraction.Blocks.Variables;
using Undefined.Services.Lambdas.Abstraction.Methods;
using Undefined.Services.Lambdas.Abstraction.Presentations;

namespace Undefined.Services.Lambdas.Abstraction;

public interface IVariableCollection :  IUCollection<IVariable>;
public interface IBlockCollection : IUCollection<ILambdaBlock>;
public interface IMethodParameterCollection : IUReadOnlyCollection<ICompiledMethodParameter>;
