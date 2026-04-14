namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS class selector.</summary>
public sealed class ClassSelector : SelectorBase
{
    private ClassSelector(string name) : base(Priority.OneClass, $".{name}")
    {
        Class = name;
    }

    /// <summary>Gets the class.</summary>
    public string Class { get; }

    /// <summary>Performs the create operation.</summary>
    public static ClassSelector Create(string name)
    {
        return new ClassSelector(name);
    }
}
