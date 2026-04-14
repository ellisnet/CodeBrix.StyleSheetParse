namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS conditionfunction node.</summary>
public interface IConditionFunction : IStylesheetNode
{
    /// <summary>The check member.</summary>
    bool Check();
}
