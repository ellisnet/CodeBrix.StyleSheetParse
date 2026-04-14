namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS combinator selector value.</summary>
public struct CombinatorSelector
{
    /// <summary>Gets or sets the delimiter.</summary>
    public string Delimiter { get; internal set; }
    /// <summary>Gets or sets the selector.</summary>
    public ISelector Selector { get; internal set; }
}
