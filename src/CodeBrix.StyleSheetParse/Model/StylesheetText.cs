using System;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS stylesheet text.</summary>
public class StylesheetText
{
    private readonly TextSource _source;

    internal StylesheetText(TextRange range, TextSource source)
    {
        Range = range;
        _source = source;
    }

    /// <summary>Gets the range.</summary>
    public TextRange Range { get; }

    /// <summary>The text member.</summary>
    public string Text
    {
        get
        {
            var start = Math.Max(Range.Start.Position - 1, 0);
            var length = Range.End.Position + 1 - Range.Start.Position;
            var text = _source.Text;

            if (start + length > text.Length) length = text.Length - start;

            return text.Substring(start, length);
        }
    }
}
