namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS pagerule node.</summary>
public interface IPageRule : IRule
{
    /// <summary>The selector text member.</summary>
    string SelectorText { get; set; }
    /// <summary>The style member.</summary>
    StyleDeclaration Style { get; }
}
