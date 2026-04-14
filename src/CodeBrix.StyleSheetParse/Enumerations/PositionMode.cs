namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Specifies the position mode options.</summary>
public enum PositionMode : byte
{
    /// <summary>The static value.</summary>
    Static,
    /// <summary>The relative value.</summary>
    Relative,
    /// <summary>The absolute value.</summary>
    Absolute,
    /// <summary>The fixed value.</summary>
    Fixed,
    /// <summary>The sticky value.</summary>
    Sticky
}
