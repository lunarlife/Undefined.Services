namespace Undefined.Services.Application.Services.Lookup.Runtime.Dependencies.Methods;

[Flags]
public enum MemberAccessModifiers
{
    None = 1 << 0,
    Private = 1 << 1,
    Protected = 1 << 2,
    Internal = 1 << 3,
    Public = 1 << 4,
    All = Private | Protected | Internal | Public,
    NonPublic = Private | Protected | Internal
}