namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS fontfacerule node.</summary>
public interface IFontFaceRule : IRule, IProperties
{
    /// <summary>The family member.</summary>
    string Family { get; set; }
    /// <summary>The source member.</summary>
    string Source { get; set; }
    /// <summary>The style member.</summary>
    string Style { get; set; }
    /// <summary>The weight member.</summary>
    string Weight { get; set; }
    /// <summary>The stretch member.</summary>
    string Stretch { get; set; }
    /// <summary>The range member.</summary>
    string Range { get; set; }
    /// <summary>The variant member.</summary>
    string Variant { get; set; }
    /// <summary>The features member.</summary>
    string Features { get; set; }
}
