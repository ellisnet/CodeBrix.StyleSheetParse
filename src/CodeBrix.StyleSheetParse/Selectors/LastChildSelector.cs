namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS last child selector.</summary>
public sealed class LastChildSelector : ChildSelector
{
    /// <summary>Initializes a new instance of the <see cref="LastChildSelector"/> class.</summary>
    public LastChildSelector()
        : base(PseudoClassNames.NthLastChild)
    {
    }
}
