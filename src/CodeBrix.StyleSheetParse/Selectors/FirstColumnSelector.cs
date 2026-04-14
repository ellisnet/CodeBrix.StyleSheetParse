namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS first column selector.</summary>
public sealed class FirstColumnSelector : ChildSelector
{
    /// <summary>Initializes a new instance of the <see cref="FirstColumnSelector"/> class.</summary>
    public FirstColumnSelector()
        : base(PseudoClassNames.NthColumn)
    {
    }
}
