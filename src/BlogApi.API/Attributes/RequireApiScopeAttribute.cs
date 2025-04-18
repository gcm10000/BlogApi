namespace BlogApi.API.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class RequireApiScopeAttribute : Attribute
{
    public string Scope { get; }

    public RequireApiScopeAttribute(string scope)
    {
        Scope = scope;
    }
}
