using System.IO;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS charset rule.</summary>
public sealed class CharsetRule : Rule, ICharsetRule
{
    internal CharsetRule(StylesheetParser parser)
        : base(RuleType.Charset, parser)
    {
    }

    /// <summary>Gets or sets the character set.</summary>
    public string CharacterSet { get; set; }

    /// <summary>Performs the to css operation.</summary>
    public override void ToCss(TextWriter writer, IStyleFormatter formatter)
    {
        writer.Write(formatter.Rule("@charset", CharacterSet.StylesheetString()));
    }

    /// <summary>The replace with member.</summary>
    protected override void ReplaceWith(IRule rule)
    {
        var newRule = rule as CharsetRule;
        CharacterSet = newRule?.CharacterSet;
        base.ReplaceWith(rule);
    }
}
