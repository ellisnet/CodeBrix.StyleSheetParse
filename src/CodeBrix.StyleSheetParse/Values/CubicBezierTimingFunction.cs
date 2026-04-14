namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS cubic bezier timing function.</summary>
public sealed class CubicBezierTimingFunction : ITimingFunction
{
    /// <summary>Initializes a new instance of the <see cref="CubicBezierTimingFunction"/> class.</summary>
    public CubicBezierTimingFunction(float x1, float y1, float x2, float y2)
    {
        X1 = x1;
        Y1 = y1;
        X2 = x2;
        Y2 = y2;
    }

    /// <summary>Gets the x1.</summary>
    public float X1 { get; }
    /// <summary>Gets the y1.</summary>
    public float Y1 { get; }
    /// <summary>Gets the x2.</summary>
    public float X2 { get; }
    /// <summary>Gets the y2.</summary>
    public float Y2 { get; }
}
