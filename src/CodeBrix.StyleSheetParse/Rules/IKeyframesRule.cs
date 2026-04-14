namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS keyframesrule node.</summary>
public interface IKeyframesRule : IRule
{
    /// <summary>The name member.</summary>
    string Name { get; set; }
    /// <summary>The rules member.</summary>
    IRuleList Rules { get; }
    /// <summary>The add member.</summary>
    void Add(string rule);
    /// <summary>The remove member.</summary>
    void Remove(string key);
    /// <summary>The find member.</summary>
    IKeyframeRule Find(string key);
}
