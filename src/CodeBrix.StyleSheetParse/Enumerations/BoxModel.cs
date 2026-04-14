namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Specifies the box model options.</summary>
public enum BoxModel : byte
{
    /// <summary>The border box value.</summary>
    BorderBox,
    /// <summary>The padding box value.</summary>
    PaddingBox,
    /// <summary>The content box value.</summary>
    ContentBox
}
