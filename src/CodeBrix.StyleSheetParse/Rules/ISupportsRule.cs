namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS supportsrule node.</summary>
public interface ISupportsRule : IConditionRule
{
    /// <summary>The condition member.</summary>
    IConditionFunction Condition { get; }
}
