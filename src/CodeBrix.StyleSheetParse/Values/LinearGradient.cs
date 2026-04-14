using System.Collections.Generic;
using System.Linq;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS linear gradient.</summary>
public sealed class LinearGradient : IGradient
{
    /// <summary>Initializes a new instance of the <see cref="LinearGradient"/> class.</summary>
    public LinearGradient(Angle angle, GradientStop[] stops, bool repeating = false)
    {
        _stops = stops;
        Angle = angle;
        IsRepeating = repeating;
    }

    private readonly GradientStop[] _stops;

    /// <summary>Gets the angle.</summary>
    public Angle Angle { get; }
    /// <summary>Gets the stops.</summary>
    public IEnumerable<GradientStop> Stops => _stops.AsEnumerable();
    /// <summary>Gets the is repeating.</summary>
    public bool IsRepeating { get; }
}
