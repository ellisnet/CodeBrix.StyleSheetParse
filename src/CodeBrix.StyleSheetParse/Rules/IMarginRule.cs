namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS marginrule node.</summary>
public interface IMarginRule : IRule
{
    /// <summary>The name member.</summary>
    string Name { get; }
    /// <summary>The style member.</summary>
    StyleDeclaration Style { get; }
}
