namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Specifies the line style options.</summary>
public enum LineStyle : byte
{
    /// <summary>The none value.</summary>
    None,
    /// <summary>The hidden value.</summary>
    Hidden,
    /// <summary>The dotted value.</summary>
    Dotted,
    /// <summary>The dashed value.</summary>
    Dashed,
    /// <summary>The solid value.</summary>
    Solid,
    /// <summary>The double value.</summary>
    Double,
    /// <summary>The groove value.</summary>
    Groove,
    /// <summary>The ridge value.</summary>
    Ridge,
    /// <summary>The inset value.</summary>
    Inset,
    /// <summary>The outset value.</summary>
    Outset
}
