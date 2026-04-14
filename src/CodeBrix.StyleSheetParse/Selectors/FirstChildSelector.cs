namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS first child selector.</summary>
public sealed class FirstChildSelector : ChildSelector
{
    /// <summary>Initializes a new instance of the <see cref="FirstChildSelector"/> class.</summary>
    public FirstChildSelector()
        : base(PseudoClassNames.NthChild)
    {
    }
}
