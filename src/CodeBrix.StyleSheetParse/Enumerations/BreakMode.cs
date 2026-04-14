namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Specifies the break mode options.</summary>
public enum BreakMode : byte
{
    /// <summary>The auto value.</summary>
    Auto,
    /// <summary>The always value.</summary>
    Always,
    /// <summary>The avoid value.</summary>
    Avoid,
    /// <summary>The left value.</summary>
    Left,
    /// <summary>The right value.</summary>
    Right,
    /// <summary>The page value.</summary>
    Page,
    /// <summary>The column value.</summary>
    Column,
    /// <summary>The avoid page value.</summary>
    AvoidPage,
    /// <summary>The avoid column value.</summary>
    AvoidColumn,
    /// <summary>The avoid region value.</summary>
    AvoidRegion
}
