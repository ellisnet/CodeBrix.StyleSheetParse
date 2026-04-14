namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS shadow.</summary>
public sealed class Shadow
{
    /// <summary>Initializes a new instance of the <see cref="Shadow"/> class.</summary>
    public Shadow(bool inset, Length offsetX, Length offsetY, Length blurRadius, Length spreadRadius, Color color)
    {
        IsInset = inset;
        OffsetX = offsetX;
        OffsetY = offsetY;
        BlurRadius = blurRadius;
        SpreadRadius = spreadRadius;
        Color = color;
    }

    /// <summary>Gets the color.</summary>
    public Color Color { get; }
    /// <summary>Gets or sets the offset x.</summary>
    public Length OffsetX { get; }
    /// <summary>Gets or sets the offset y.</summary>
    public Length OffsetY { get; }
    /// <summary>Gets the blur radius.</summary>
    public Length BlurRadius { get; }
    /// <summary>Gets the spread radius.</summary>
    public Length SpreadRadius { get; }
    /// <summary>Gets or sets the is inset.</summary>
    public bool IsInset { get; }
}
