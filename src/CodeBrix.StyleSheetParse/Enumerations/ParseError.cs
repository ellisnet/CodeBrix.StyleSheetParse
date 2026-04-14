namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Specifies the parse error options.</summary>
public enum ParseError : byte
{
    /// <summary>The eof value.</summary>
    EOF = 0,
    /// <summary>The invalid character value.</summary>
    InvalidCharacter = 16, // 0x10,
    /// <summary>The invalid block start value.</summary>
    InvalidBlockStart = 17, // 0x11,
    /// <summary>The invalid token value.</summary>
    InvalidToken = 18, // 0x12,
    /// <summary>The colon missing value.</summary>
    ColonMissing = 19, // 0x13,
    /// <summary>The ident expected value.</summary>
    IdentExpected = 20, // 0x14,
    /// <summary>The input unexpected value.</summary>
    InputUnexpected = 21, // 0x15,
    /// <summary>The line break unexpected value.</summary>
    LineBreakUnexpected = 22, // 0x16,
    /// <summary>The unknown at rule value.</summary>
    UnknownAtRule = 32, // 0x20,
    /// <summary>The invalid selector value.</summary>
    InvalidSelector = 48, // 0x30,
    /// <summary>The invalid keyframe value.</summary>
    InvalidKeyframe = 49, // 0x31,
    /// <summary>The value missing value.</summary>
    ValueMissing = 64, // 0x40,
    /// <summary>The invalid value value.</summary>
    InvalidValue = 65, // 0x41,
    /// <summary>The unknown declaration name value.</summary>
    UnknownDeclarationName = 80 // 0x50
}
