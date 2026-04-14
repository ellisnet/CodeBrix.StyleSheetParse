namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS pseudo class selector.</summary>
public sealed class PseudoClassSelector : SelectorBase
{
    private PseudoClassSelector(string name) : base(Priority.OneClass, $"{PseudoClassNames.Separator}{name}")
    {
        Class = name;
    }

    /// <summary>Gets the class.</summary>
    public string Class { get; }

    /// <summary>Performs the create operation.</summary>
    public static ISelector Create(string name)
    {
        return new PseudoClassSelector(name);
    }
}
