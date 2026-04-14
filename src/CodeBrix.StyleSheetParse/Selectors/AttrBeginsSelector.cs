namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS attr begins selector.</summary>
public sealed class AttrBeginsSelector : AttrSelectorBase
{
    /// <summary>Initializes a new instance of the <see cref="AttrBeginsSelector"/> class.</summary>
    public AttrBeginsSelector(string attribute, string value) 
        : base(attribute, value, $"[{attribute}^={value.StylesheetString()}]")
    {
    }
}
