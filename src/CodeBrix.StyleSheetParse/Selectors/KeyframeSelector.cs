using System.Collections.Generic;
using System.IO;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS keyframe selector.</summary>
public sealed class KeyframeSelector : StylesheetNode
{
    private readonly List<Percent> _stops;

    /// <summary>Initializes a new instance of the <see cref="KeyframeSelector"/> class.</summary>
    public KeyframeSelector(IEnumerable<Percent> stops)
    {
        _stops = new List<Percent>(stops);
    }

    /// <summary>Performs the to css operation.</summary>
    public override void ToCss(TextWriter writer, IStyleFormatter formatter)
    {
        if (_stops.Count <= 0) return;

        writer.Write(_stops[0].ToString());
        for (var i = 1; i < _stops.Count; i++)
        {
            writer.Write(", ");
            writer.Write(_stops[i].ToString());
        }
    }

    /// <summary>Gets the stops.</summary>
    public IEnumerable<Percent> Stops => _stops;
    /// <summary>Gets the text.</summary>
    public string Text => this.ToCss();
}

/// <summary>Represents a CSS page selector.</summary>
public sealed class PageSelector : StylesheetNode, ISelector
{
    private readonly string _name;

    /// <summary>Initializes a new instance of the <see cref="PageSelector"/> class.</summary>
    public PageSelector(string name)
    {
        _name = name;
    }

    /// <summary>Initializes a new instance of the <see cref="PageSelector"/> class.</summary>
    public PageSelector() : this(string.Empty)
    {
    }

    /// <summary>Performs the to css operation.</summary>
    public override void ToCss(TextWriter writer, IStyleFormatter formatter)
    {
        var pseudo = _name == string.Empty ? "" : ":";
        writer.Write($"{pseudo}{_name}");
    }

    /// <summary>Gets the specificity.</summary>
    public Priority Specificity => Priority.Inline;
    /// <summary>Gets the text.</summary>
    public string Text => this.ToCss();
}
