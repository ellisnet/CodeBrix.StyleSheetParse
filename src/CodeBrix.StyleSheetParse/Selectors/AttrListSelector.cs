namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS attr list selector.</summary>
public sealed class AttrListSelector : AttrSelectorBase
{
    /// <summary>Initializes a new instance of the <see cref="AttrListSelector"/> class.</summary>
    public AttrListSelector(string attribute, string value) 
        : base(attribute, value, $"[{attribute}~={value.StylesheetString()}]")
    {
    }
}
