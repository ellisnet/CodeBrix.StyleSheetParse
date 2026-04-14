using System.Collections.Generic;
using System.IO;
using System.Linq;

// ReSharper disable UnusedMember.Global

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS stylesheet.</summary>
public sealed class Stylesheet : StylesheetNode
{
    private readonly StylesheetParser _parser;

    internal Stylesheet(StylesheetParser parser)
    {
        _parser = parser;
        Rules = new RuleList(this);
    }

    internal RuleList Rules { get; }

    /// <summary>Gets the character set rules.</summary>
    public IEnumerable<ICharsetRule> CharacterSetRules => Rules.Where(r => r is CharsetRule).Cast<ICharsetRule>();
    /// <summary>Gets the fontface set rules.</summary>
    public IEnumerable<IFontFaceRule> FontfaceSetRules => Rules.Where(r => r is FontFaceRule).Cast<IFontFaceRule>();
    /// <summary>Gets the media rules.</summary>
    public IEnumerable<IMediaRule> MediaRules => Rules.Where(r => r is MediaRule).Cast<IMediaRule>();
    /// <summary>Gets the container rules.</summary>
    public IEnumerable<IContainerRule> ContainerRules => Rules.Where(r => r is ContainerRule).Cast<IContainerRule>();
    /// <summary>Gets the import rules.</summary>
    public IEnumerable<IImportRule> ImportRules => Rules.Where(r => r is ImportRule).Cast<IImportRule>();

    /// <summary>Gets the namespace rules.</summary>
    public IEnumerable<INamespaceRule> NamespaceRules =>
        Rules.Where(r => r is NamespaceRule).Cast<INamespaceRule>();

    /// <summary>Gets the page rules.</summary>
    public IEnumerable<IPageRule> PageRules => Rules.Where(r => r is PageRule).Cast<IPageRule>();
    /// <summary>Gets the style rules.</summary>
    public IEnumerable<IStyleRule> StyleRules => Rules.Where(r => r is StyleRule).Cast<IStyleRule>();

    /// <summary>Performs the add operation.</summary>
    public IRule Add(RuleType ruleType)
    {
        var rule = _parser.CreateRule(ruleType);
        Rules.Add(rule);
        return rule;
    }

    /// <summary>Performs the remove at operation.</summary>
    public void RemoveAt(int index)
    {
        Rules.RemoveAt(index);
    }

    /// <summary>Performs the insert operation.</summary>
    public int Insert(string ruleText, int index)
    {
        var rule = _parser.ParseRule(ruleText);
        rule.Owner = this;
        Rules.Insert(index, rule);

        return index;
    }

    /// <summary>Performs the to css operation.</summary>
    public override void ToCss(TextWriter writer, IStyleFormatter formatter)
    {
        writer.Write(formatter.Sheet(Rules));
    }
}
