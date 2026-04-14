namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>The combinators member.</summary>
public static class Combinators
{
    /// <summary>Gets the exactly value.</summary>
    public static readonly string Exactly = "=";
    /// <summary>Gets the unlike value.</summary>
    public static readonly string Unlike = "!=";
    /// <summary>Gets the in list value.</summary>
    public static readonly string InList = "~=";
    /// <summary>Gets the in token value.</summary>
    public static readonly string InToken = "|=";
    /// <summary>Gets the begins value.</summary>
    public static readonly string Begins = "^=";
    /// <summary>Gets the ends value.</summary>
    public static readonly string Ends = "$=";
    /// <summary>Gets the in text value.</summary>
    public static readonly string InText = "*=";
    /// <summary>Gets the column value.</summary>
    public static readonly string Column = "||";
    /// <summary>Gets the pipe value.</summary>
    public static readonly string Pipe = "|";
    /// <summary>Gets the adjacent value.</summary>
    public static readonly string Adjacent = "+";
    /// <summary>Gets the descendent value.</summary>
    public static readonly string Descendent = " ";
    /// <summary>Gets the deep value.</summary>
    public static readonly string Deep = ">>>";
    /// <summary>Gets the child value.</summary>
    public static readonly string Child = ">";
    /// <summary>Gets the sibling value.</summary>
    public static readonly string Sibling = "~";
}
