using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS complex selector.</summary>
public sealed class ComplexSelector : StylesheetNode, ISelector, IEnumerable<CombinatorSelector>
{
    private readonly List<CombinatorSelector> _selectors;

    /// <summary>Initializes a new instance of the <see cref="ComplexSelector"/> class.</summary>
    public ComplexSelector()
    {
        _selectors = new List<CombinatorSelector>();
    }

    /// <summary>Gets the text.</summary>
    public string Text => this.ToCss();
    /// <summary>Gets the length.</summary>
    public int Length => _selectors.Count;
    /// <summary>Gets or sets the is ready.</summary>
    public bool IsReady { get; private set; }

    /// <summary>The specificity member.</summary>
    public Priority Specificity
    {
        get
        {
            var sum = new Priority();
            var n = _selectors.Count;

            for (var i = 0; i < n; i++) sum += _selectors[i].Selector.Specificity;

            return sum;
        }
    }

    /// <summary>Performs the to css operation.</summary>
    public override void ToCss(TextWriter writer, IStyleFormatter formatter)
    {
        if (_selectors.Count <= 0) return;

        var n = _selectors.Count - 1;

        for (var i = 0; i < n; i++)
        {
            writer.Write(_selectors[i].Selector.Text);
            writer.Write(_selectors[i].Delimiter);
        }

        writer.Write(_selectors[n].Selector.Text);
    }

    /// <summary>Performs the conclude selector operation.</summary>
    public void ConcludeSelector(ISelector selector)
    {
        if (IsReady) return;

        _selectors.Add(new CombinatorSelector
        {
            Selector = selector,
            Delimiter = null
        });
        IsReady = true;
    }

    /// <summary>Performs the append selector operation.</summary>
    public void AppendSelector(ISelector selector, Combinator combinator)
    {
        if (!IsReady)
            _selectors.Add(new CombinatorSelector
            {
                Selector = combinator.Change(selector),
                Delimiter = combinator.Delimiter,
            });
    }

    /// <summary>Performs the get enumerator operation.</summary>
    public IEnumerator<CombinatorSelector> GetEnumerator()
    {
        return _selectors.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
