namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS conditionrule node.</summary>
public interface IConditionRule : IGroupingRule
{
    /// <summary>The condition text member.</summary>
    string ConditionText { get; set; }
}
