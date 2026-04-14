namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS selector node.</summary>
public interface ISelector : IStylesheetNode
{
    /// <summary>The specificity member.</summary>
    Priority Specificity { get; }
    /// <summary>The text member.</summary>
    string Text { get; }
}
