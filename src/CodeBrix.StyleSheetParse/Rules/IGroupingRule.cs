namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS groupingrule node.</summary>
public interface IGroupingRule : IRule, IRuleCreator
{
    /// <summary>The rules member.</summary>
    IRuleList Rules { get; }
    /// <summary>The insert member.</summary>
    int Insert(string rule, int index);
    /// <summary>The remove at member.</summary>
    void RemoveAt(int index);
}
