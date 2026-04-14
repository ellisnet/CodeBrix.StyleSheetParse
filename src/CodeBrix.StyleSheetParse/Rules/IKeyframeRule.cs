namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS keyframerule node.</summary>
public interface IKeyframeRule : IRule
{
    /// <summary>The key text member.</summary>
    string KeyText { get; set; }
    /// <summary>The style member.</summary>
    StyleDeclaration Style { get; }
    /// <summary>The key member.</summary>
    KeyframeSelector Key { get; set; }
}
