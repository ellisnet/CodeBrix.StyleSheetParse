namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS mediafeature node.</summary>
public interface IMediaFeature : IStylesheetNode
{
    /// <summary>The name member.</summary>
    string Name { get; }
    /// <summary>The value member.</summary>
    string Value { get; }
    /// <summary>The has value member.</summary>
    bool HasValue { get; }
}
