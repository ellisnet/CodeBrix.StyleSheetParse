using System.Collections.Generic;
using System.Linq;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS radial gradient.</summary>
public sealed class RadialGradient : IImageSource
{
    /// <summary>Specifies the size mode options.</summary>
    public enum SizeMode : byte
    {
        /// <summary>The none value.</summary>
        None,
        /// <summary>The closest corner value.</summary>
        ClosestCorner,
        /// <summary>The closest side value.</summary>
        ClosestSide,
        /// <summary>The farthest corner value.</summary>
        FarthestCorner,
        /// <summary>The farthest side value.</summary>
        FarthestSide
    }

    /// <summary>Initializes a new instance of the <see cref="RadialGradient"/> class.</summary>
    public RadialGradient(bool circle, Point pt, Length width, Length height, SizeMode sizeMode,
        GradientStop[] stops, bool repeating = false)
    {
        _stops = stops;
        Position = pt;
        MajorRadius = width;
        MinorRadius = height;
        IsRepeating = repeating;
        IsCircle = circle;
        Mode = sizeMode;
    }

    private readonly GradientStop[] _stops;

    /// <summary>Gets the is circle.</summary>
    public bool IsCircle { get; }

    /// <summary>Gets the mode.</summary>
    public SizeMode Mode { get; }
    /// <summary>Gets the position.</summary>
    public Point Position { get; }
    /// <summary>Gets the major radius.</summary>
    public Length MajorRadius { get; }
    /// <summary>Gets the minor radius.</summary>
    public Length MinorRadius { get; }
    /// <summary>Gets the stops.</summary>
    public IEnumerable<GradientStop> Stops => _stops.AsEnumerable();
    /// <summary>Gets the is repeating.</summary>
    public bool IsRepeating { get; }
}
