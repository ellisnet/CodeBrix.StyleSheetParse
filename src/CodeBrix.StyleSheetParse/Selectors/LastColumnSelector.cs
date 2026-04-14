namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS last column selector.</summary>
public sealed class LastColumnSelector : ChildSelector
{
    /// <summary>Initializes a new instance of the <see cref="LastColumnSelector"/> class.</summary>
    public LastColumnSelector()
        : base(PseudoClassNames.NthLastColumn)
    {
    }
}
