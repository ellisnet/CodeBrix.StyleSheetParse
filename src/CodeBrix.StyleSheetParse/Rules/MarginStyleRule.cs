using System.IO;
using System.Linq;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS margin style rule.</summary>
public sealed class MarginStyleRule : Rule, IStyleRule
{
    /// <summary>Initializes a new instance of the <see cref="MarginStyleRule"/> class.</summary>
    public MarginStyleRule(StylesheetParser parser) : base(RuleType.Style, parser)
    {
        AppendChild(new StyleDeclaration(this));
    }

    /// <summary>Performs the to css operation.</summary>
    public override void ToCss(TextWriter writer, IStyleFormatter formatter)
    {
        writer.Write(formatter.Style(SelectorText, Style));
    }

    /// <summary>The selector member.</summary>
    public ISelector Selector
    {
        get => Children.OfType<ISelector>().FirstOrDefault();
        set => ReplaceSingle(Selector, value);
    }

    /// <summary>The selector text member.</summary>
    public string SelectorText
    {
        get => $"@{Selector.Text}";
        set => Selector = Parser.ParseSelector(value);
    }

    /// <summary>Gets the style.</summary>
    public StyleDeclaration Style => Children.OfType<StyleDeclaration>().FirstOrDefault();
}
