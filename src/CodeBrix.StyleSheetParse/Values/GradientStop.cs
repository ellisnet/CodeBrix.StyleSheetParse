namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS gradient stop value.</summary>
public struct GradientStop
{
    /// <summary>Initializes a new instance of the <see cref="GradientStop"/> class.</summary>
    public GradientStop(Color color, Length location)
    {
        Color = color;
        Location = location;
    }

    /// <summary>Gets the color.</summary>
    public Color Color { get; }
    /// <summary>Gets the location.</summary>
    public Length Location { get; }
}
