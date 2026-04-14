namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Specifies the list style options.</summary>
public enum ListStyle : byte
{
    /// <summary>The none value.</summary>
    None,
    /// <summary>The disc value.</summary>
    Disc,
    /// <summary>The circle value.</summary>
    Circle,
    /// <summary>The square value.</summary>
    Square,
    /// <summary>The decimal value.</summary>
    Decimal,
    /// <summary>The decimal leading zero value.</summary>
    DecimalLeadingZero,
    /// <summary>The lower roman value.</summary>
    LowerRoman,
    /// <summary>The upper roman value.</summary>
    UpperRoman,
    /// <summary>The lower greek value.</summary>
    LowerGreek,
    /// <summary>The lower latin value.</summary>
    LowerLatin,
    /// <summary>The upper latin value.</summary>
    UpperLatin,
    /// <summary>The armenian value.</summary>
    Armenian,
    /// <summary>The georgian value.</summary>
    Georgian
}
