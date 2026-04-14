using System.IO;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS selector base.</summary>
public abstract class SelectorBase : StylesheetNode, ISelector
{
    /// <summary>The selector base member.</summary>
    protected SelectorBase(Priority specificity, string text)
    {
        Specificity = specificity;
        Text = text;
    }

    /// <summary>Performs the to css operation.</summary>
    public override void ToCss(TextWriter writer, IStyleFormatter formatter)
    {
        writer.Write(Text);
    }

    /// <summary>Gets the specificity.</summary>
    public Priority Specificity { get; }
    /// <summary>Gets the text.</summary>
    public string Text { get; }
}
