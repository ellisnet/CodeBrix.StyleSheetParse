namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS namespacerule node.</summary>
public interface INamespaceRule : IRule
{
    /// <summary>The namespace uri member.</summary>
    string NamespaceUri { get; set; }
    /// <summary>The prefix member.</summary>
    string Prefix { get; set; }
}
