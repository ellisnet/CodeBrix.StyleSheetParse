namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS shape.</summary>
public sealed class Shape
{
    /// <summary>Initializes a new instance of the <see cref="Shape"/> class.</summary>
    public Shape(Length top, Length right, Length bottom, Length left)
    {
        Top = top;
        Right = right;
        Bottom = bottom;
        Left = left;
    }

    /// <summary>Gets the top.</summary>
    public Length Top { get; }
    /// <summary>Gets the right.</summary>
    public Length Right { get; }
    /// <summary>Gets the bottom.</summary>
    public Length Bottom { get; }
    /// <summary>Gets the left.</summary>
    public Length Left { get; }
}
