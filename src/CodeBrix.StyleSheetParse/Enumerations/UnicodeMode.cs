namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Specifies the unicode mode options.</summary>
public enum UnicodeMode : byte
{
    /// <summary>The normal value.</summary>
    Normal,
    /// <summary>The embed value.</summary>
    Embed,
    /// <summary>The isolate value.</summary>
    Isolate,
    /// <summary>The bidirectional override value.</summary>
    BidirectionalOverride,
    /// <summary>The isolate override value.</summary>
    IsolateOverride,
    /// <summary>The plaintext value.</summary>
    Plaintext
}
