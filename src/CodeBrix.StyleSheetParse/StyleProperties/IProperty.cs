namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS property node.</summary>
public interface IProperty : IStylesheetNode
{
    /// <summary>The name member.</summary>
    string Name { get; }
    /// <summary>The value member.</summary>
    string Value { get; }
    /// <summary>The original member.</summary>
    string Original { get; }
    /// <summary>The is important member.</summary>
    bool IsImportant { get; }
}
