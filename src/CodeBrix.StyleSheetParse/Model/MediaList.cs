using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS media list.</summary>
public sealed class MediaList : StylesheetNode
{
    private readonly StylesheetParser _parser;

    internal MediaList(StylesheetParser parser)
    {
        _parser = parser;
    }

    /// <summary>The this member.</summary>
    public string this[int index] => Media.GetItemByIndex(index).ToCss();
    /// <summary>Gets the media.</summary>
    public IEnumerable<Medium> Media => Children.OfType<Medium>();
    /// <summary>Gets the length.</summary>
    public int Length => Media.Count();

    /// <summary>The media text member.</summary>
    public string MediaText
    {
        get => this.ToCss();
        set
        {
            Clear();

            foreach (var medium in _parser.ParseMediaList(value))
            {
                if (medium == null) throw new ParseException("Unable to parse media list element");
                AppendChild(medium);
            }
        }
    }

    /// <summary>Performs the to css operation.</summary>
    public override void ToCss(TextWriter writer, IStyleFormatter formatter)
    {
        var parts = Media.ToArray();
        if (parts.Length <= 0) return;
        parts[0].ToCss(writer, formatter);

        for (var i = 1; i < parts.Length; i++)
        {
            writer.Write(", ");
            parts[i].ToCss(writer, formatter);
        }
    }

    /// <summary>Performs the add operation.</summary>
    public void Add(string newMedium)
    {
        var medium = _parser.ParseMedium(newMedium);
        if (medium == null) throw new ParseException("Unable to parse medium");
        AppendChild(medium);
    }

    /// <summary>Performs the remove operation.</summary>
    public void Remove(string oldMedium)
    {
        var medium = _parser.ParseMedium(oldMedium);
        if (medium == null) throw new ParseException("Unable to parse medium");

        foreach (var item in Media)
        {
            if (!item.Equals(medium)) continue;

            RemoveChild(item);
            return;
        }

        throw new ParseException("Media list element not found");
    }

    /// <summary>Performs the get enumerator operation.</summary>
    public IEnumerator<Medium> GetEnumerator()
    {
        return Media.GetEnumerator();
    }
}
