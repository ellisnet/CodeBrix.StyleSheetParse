namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS attr selector base.</summary>
public abstract class AttrSelectorBase : SelectorBase, IAttrSelector
{
    /// <summary>The attr selector base member.</summary>
    protected AttrSelectorBase(string attribute, string value, string text) : base(Priority.OneClass, text)
    {
        Attribute = attribute;
        Value = value;
    }
    /// <summary>Gets the attribute.</summary>
    public string Attribute { get; }
    /// <summary>Gets the value.</summary>
    public string Value { get; }
}
