using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS stylesheet node.</summary>
public abstract class StylesheetNode : IStylesheetNode
{
    private readonly List<IStylesheetNode> _children = new();

    /// <summary>The replace all member.</summary>
    protected void ReplaceAll(IStylesheetNode node)
    {
        Clear();
        StylesheetText = node.StylesheetText;
        foreach (var child in node.Children) AppendChild(child);
    }

    /// <summary>Gets or sets the stylesheet text.</summary>
    public StylesheetText StylesheetText { get; internal set; }

    /// <summary>Gets the children.</summary>
    public IEnumerable<IStylesheetNode> Children => _children.AsEnumerable();

    /// <summary>Performs the to css operation.</summary>
    public abstract void ToCss(TextWriter writer, IStyleFormatter formatter);

    /// <summary>Performs the append child operation.</summary>
    public void AppendChild(IStylesheetNode child)
    {
        Setup(child);
        _children.Add(child);
    }

    /// <summary>Performs the replace child operation.</summary>
    public void ReplaceChild(IStylesheetNode oldChild, IStylesheetNode newChild)
    {
        for (var i = 0; i < _children.Count; i++)
        {   if (ReferenceEquals(oldChild, _children[i]))
            {
                Teardown(oldChild);
                Setup(newChild);
                _children[i] = newChild;
                return;
            }
        }
    }

    /// <summary>Performs the insert before operation.</summary>
    public void InsertBefore(IStylesheetNode referenceChild, IStylesheetNode child)
    {
        if (referenceChild != null)
        {
            var index = _children.IndexOf(referenceChild);
            InsertChild(index, child);
        }
        else
        {
            AppendChild(child);
        }
    }

    /// <summary>Performs the insert child operation.</summary>
    public void InsertChild(int index, IStylesheetNode child)
    {
        Setup(child);
        _children.Insert(index, child);
    }

    /// <summary>Performs the remove child operation.</summary>
    public void RemoveChild(IStylesheetNode child)
    {
        Teardown(child);
        _children.Remove(child);
    }

    /// <summary>Performs the clear operation.</summary>
    public void Clear()
    {
        for (var i = _children.Count - 1; i >= 0; i--)
        {
            var child = _children[i];
            RemoveChild(child);
        }
    }

    private void Setup(IStylesheetNode child)
    {
        if (!(child is Rule rule)) return;
        rule.Owner = this as Stylesheet;
        rule.Parent = this as IRule;
    }

    private static void Teardown(IStylesheetNode child)
    {
        if (!(child is Rule rule)) return;
        rule.Parent = null;
        rule.Owner = null;
    }
}
