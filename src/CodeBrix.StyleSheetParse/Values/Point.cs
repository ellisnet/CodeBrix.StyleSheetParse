// ReSharper disable UnusedMember.Global

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS point value.</summary>
public struct Point
{
    /// <summary>
    ///     Gets the (50%, 50%) point.
    /// </summary>
    public static readonly Point Center = new(Length.Half, Length.Half);

    /// <summary>
    ///     Gets the (0, 0) point.
    /// </summary>
    public static readonly Point LeftTop = new(Length.Zero, Length.Zero);

    /// <summary>
    ///     Gets the (100%, 0) point.
    /// </summary>
    public static readonly Point RightTop = new(Length.Full, Length.Zero);

    /// <summary>
    ///     Gets the (100%, 100%) point.
    /// </summary>
    public static readonly Point RightBottom = new(Length.Full, Length.Full);

    /// <summary>
    ///     Gets the (0, 100%) point.
    /// </summary>
    public static readonly Point LeftBottom = new(Length.Zero, Length.Full);

    /// <summary>Initializes a new instance of the <see cref="Point"/> class.</summary>
    public Point(Length x, Length y)
    {
        X = x;
        Y = y;
    }

    /// <summary>Gets the x.</summary>
    public Length X { get; }
    /// <summary>Gets the y.</summary>
    public Length Y { get; }
}
