using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS selectors.</summary>
public abstract class Selectors : StylesheetNode, IEnumerable<ISelector>
{
    /// <summary>The _selectors member.</summary>
    protected readonly List<ISelector> _selectors;

    /// <summary>The selectors member.</summary>
    protected Selectors()
    {
        _selectors = new List<ISelector>();
    }

    /// <summary>The specificity member.</summary>
    public Priority Specificity
    {
        get
        {
            var sum = new Priority();

            return _selectors.Aggregate(sum, (current, t) => current + t.Specificity);
        }
    }

    /// <summary>Gets the text.</summary>
    public string Text => this.ToCss();
    /// <summary>Gets the length.</summary>
    public int Length => _selectors.Count;

    /// <summary>The this member.</summary>
    public ISelector this[int index]
    {
        get => _selectors[index];
        set => _selectors[index] = value;
    }

    /// <summary>Performs the add operation.</summary>
    public void Add(ISelector selector)
    {
        _selectors.Add(selector);
    }

    /// <summary>Performs the remove operation.</summary>
    public void Remove(ISelector selector)
    {
        _selectors.Remove(selector);
    }

    /// <summary>Performs the get enumerator operation.</summary>
    public IEnumerator<ISelector> GetEnumerator()
    {
        return _selectors.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
