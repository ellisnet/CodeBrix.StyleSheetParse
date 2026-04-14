namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS type selector.</summary>
public sealed class TypeSelector : SelectorBase
{
    private TypeSelector(string name) : base(Priority.OneTag, name)
    {
        Name = name;
    }

    /// <summary>Gets the name.</summary>
    public string Name { get; }

    /// <summary>Performs the create operation.</summary>
    public static TypeSelector Create(string name)
    {
        return new TypeSelector(name);
    }
}
