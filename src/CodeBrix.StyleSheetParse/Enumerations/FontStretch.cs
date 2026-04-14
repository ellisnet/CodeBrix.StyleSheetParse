namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Specifies the font stretch options.</summary>
public enum FontStretch : byte
{
    /// <summary>The normal value.</summary>
    Normal,
    /// <summary>The ultra condensed value.</summary>
    UltraCondensed,
    /// <summary>The extra condensed value.</summary>
    ExtraCondensed,
    /// <summary>The condensed value.</summary>
    Condensed,
    /// <summary>The semi condensed value.</summary>
    SemiCondensed,
    /// <summary>The semi expanded value.</summary>
    SemiExpanded,
    /// <summary>The expanded value.</summary>
    Expanded,
    /// <summary>The extra expanded value.</summary>
    ExtraExpanded,
    /// <summary>The ultra expanded value.</summary>
    UltraExpanded
}
