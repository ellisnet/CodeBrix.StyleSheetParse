namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS last type selector.</summary>
public sealed class LastTypeSelector : ChildSelector
{
    /// <summary>Initializes a new instance of the <see cref="LastTypeSelector"/> class.</summary>
    public LastTypeSelector()
        : base(PseudoClassNames.NthLastOfType)
    {
    }
}
