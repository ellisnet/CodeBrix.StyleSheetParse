namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS pseudo element selector.</summary>
public sealed class PseudoElementSelector : SelectorBase
{
    private PseudoElementSelector(string name) : base(Priority.OneTag, $"{PseudoElementNames.Separator}{name}")
    {
        Name = name;
    }

    /// <summary>Gets the name.</summary>
    public string Name { get; }

    /// <summary>Performs the create operation.</summary>
    public static ISelector Create(string name)
    {
        return new PseudoElementSelector(name);
    }
}
