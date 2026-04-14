namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS first type selector.</summary>
public sealed class FirstTypeSelector : ChildSelector
{
    /// <summary>Initializes a new instance of the <see cref="FirstTypeSelector"/> class.</summary>
    public FirstTypeSelector()
        : base(PseudoClassNames.NthOfType)
    {
    }
}
