using System.IO;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS unknown selector.</summary>
public sealed class UnknownSelector : StylesheetNode, ISelector
{
    /// <summary>Gets the specificity.</summary>
    public Priority Specificity => Priority.Zero;

    /// <summary>Gets the text.</summary>
    public string Text => this.ToCss();

    /// <summary>Performs the to css operation.</summary>
    public override void ToCss(TextWriter writer, IStyleFormatter formatter)
    {
        writer.Write(StylesheetText?.Text);
    }
}
