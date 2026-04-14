using System.IO;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS compound selector.</summary>
public sealed class CompoundSelector : Selectors, ISelector
{
    /// <summary>Performs the to css operation.</summary>
    public override void ToCss(TextWriter writer, IStyleFormatter formatter)
    {
        foreach (var selector in _selectors) writer.Write(selector.Text);
    }
}
