namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS attr hyphen selector.</summary>
public sealed class AttrHyphenSelector : AttrSelectorBase
{
    /// <summary>Initializes a new instance of the <see cref="AttrHyphenSelector"/> class.</summary>
    public AttrHyphenSelector(string attribute, string value) 
        : base(attribute, value, $"[{attribute}|={value.StylesheetString()}]")
    {
    }
}
