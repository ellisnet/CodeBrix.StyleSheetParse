namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS documentfunction node.</summary>
public interface IDocumentFunction : IStylesheetNode
{
    /// <summary>The name member.</summary>
    string Name { get; }
    /// <summary>The data member.</summary>
    string Data { get; }
}
