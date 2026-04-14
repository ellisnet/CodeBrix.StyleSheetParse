using System.IO;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS list selector.</summary>
public sealed class ListSelector : Selectors, ISelector
{
    /// <summary>Gets or sets the is invalid.</summary>
    public bool IsInvalid { get; internal set; }

    /// <summary>Performs the to css operation.</summary>
    public override void ToCss(TextWriter writer, IStyleFormatter formatter)
    {
        if (_selectors.Count <= 0) return;
        writer.Write(_selectors[0].Text);

        for (var i = 1; i < _selectors.Count; i++)
        {
            writer.Write(Symbols.Comma);
            writer.Write(_selectors[i].Text);
        }
    }
}
