namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS rulecreator node.</summary>
public interface IRuleCreator
{
    /// <summary>The add new rule member.</summary>
    IRule AddNewRule(RuleType ruleType);
}
