namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS stylerule node.</summary>
public interface IStyleRule : IRule
{
    /// <summary>The selector text member.</summary>
    string SelectorText { get; set; }
    /// <summary>The style member.</summary>
    StyleDeclaration Style { get; }
    /// <summary>The selector member.</summary>
    ISelector Selector { get; set; }
}
