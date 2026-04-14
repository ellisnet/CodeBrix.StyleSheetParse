namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS attr available selector.</summary>
public sealed class AttrAvailableSelector : AttrSelectorBase
{
    /// <summary>Initializes a new instance of the <see cref="AttrAvailableSelector"/> class.</summary>
    public AttrAvailableSelector(string attribute, string value)
        : base(attribute, value, $"[{attribute}]")
    {
    }
}
