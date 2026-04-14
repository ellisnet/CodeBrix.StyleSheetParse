namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS charsetrule node.</summary>
public interface ICharsetRule : IRule
{
    /// <summary>The character set member.</summary>
    string CharacterSet { get; set; }
}
