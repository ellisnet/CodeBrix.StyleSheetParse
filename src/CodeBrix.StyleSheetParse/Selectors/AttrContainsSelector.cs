namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS attr contains selector.</summary>
public sealed class AttrContainsSelector : AttrSelectorBase
{
    /// <summary>Initializes a new instance of the <see cref="AttrContainsSelector"/> class.</summary>
    public AttrContainsSelector(string attribute, string value) 
        : base(attribute, value, $"[{attribute}*={value.StylesheetString()}]")
    {
    }
}
