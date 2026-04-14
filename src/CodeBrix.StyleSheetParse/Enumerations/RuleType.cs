namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Specifies the rule type options.</summary>
public enum RuleType : byte
{
    /// <summary>The unknown value.</summary>
    Unknown,
    /// <summary>The style value.</summary>
    Style,
    /// <summary>The charset value.</summary>
    Charset,
    /// <summary>The import value.</summary>
    Import,
    /// <summary>The media value.</summary>
    Media,
    /// <summary>The font face value.</summary>
    FontFace,
    /// <summary>The page value.</summary>
    Page,
    /// <summary>The keyframes value.</summary>
    Keyframes,
    /// <summary>The keyframe value.</summary>
    Keyframe,
    /// <summary>The margin box value.</summary>
    MarginBox,
    /// <summary>The namespace value.</summary>
    Namespace,
    /// <summary>The counter style value.</summary>
    CounterStyle,
    /// <summary>The supports value.</summary>
    Supports,
    /// <summary>The document value.</summary>
    Document,
    /// <summary>The font feature values value.</summary>
    FontFeatureValues,
    /// <summary>The viewport value.</summary>
    Viewport,
    /// <summary>The region style value.</summary>
    RegionStyle,
    /// <summary>The container value.</summary>
    Container
}
