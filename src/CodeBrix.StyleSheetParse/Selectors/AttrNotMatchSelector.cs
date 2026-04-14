namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS attr not match selector.</summary>
public sealed class AttrNotMatchSelector : AttrSelectorBase
{
    /// <summary>Initializes a new instance of the <see cref="AttrNotMatchSelector"/> class.</summary>
    public AttrNotMatchSelector(string attribute, string value) 
        : base(attribute, value, $"[{attribute}!={value.StylesheetString()}]")
    {
    }
}
