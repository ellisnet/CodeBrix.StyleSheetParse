namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS attr match selector.</summary>
public sealed class AttrMatchSelector : AttrSelectorBase
{
    /// <summary>Initializes a new instance of the <see cref="AttrMatchSelector"/> class.</summary>
    public AttrMatchSelector(string attribute, string value) 
        : base(attribute, value, $"[{attribute}={value.StylesheetString()}]")
    {
    }
}
