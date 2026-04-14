namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS attr ends selector.</summary>
public sealed class AttrEndsSelector : AttrSelectorBase
{
    /// <summary>Initializes a new instance of the <see cref="AttrEndsSelector"/> class.</summary>
    public AttrEndsSelector(string attribute, string value) 
        : base(attribute, value, $"[{attribute}$={value.StylesheetString()}]")
    {
    }
}
