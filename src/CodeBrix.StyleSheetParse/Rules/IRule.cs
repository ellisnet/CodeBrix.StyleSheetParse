namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS rule node.</summary>
public interface IRule : IStylesheetNode
{
    /// <summary>The type member.</summary>
    RuleType Type { get; }
    /// <summary>The text member.</summary>
    string Text { get; set; }
    /// <summary>The parent member.</summary>
    IRule Parent { get; }
    /// <summary>The owner member.</summary>
    Stylesheet Owner { get; }
}
