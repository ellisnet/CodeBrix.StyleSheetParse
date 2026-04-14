namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS namespace selector.</summary>
public sealed class NamespaceSelector : SelectorBase
{
    private NamespaceSelector(string prefix) : base(Priority.Zero, prefix)
    {
    }

    /// <summary>Performs the create operation.</summary>
    public static NamespaceSelector Create(string prefix)
    {
        return new NamespaceSelector(prefix);
    }
}
