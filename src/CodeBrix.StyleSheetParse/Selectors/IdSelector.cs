namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS id selector.</summary>
public sealed class IdSelector : SelectorBase
{
    private IdSelector(string name) : base(Priority.OneId, $"#{name}")
    {
        Id = name;
    }

    /// <summary>Gets the id.</summary>
    public string Id { get; }
    
    /// <summary>Performs the create operation.</summary>
    public static IdSelector Create(string name)
    {
        return new IdSelector(name);
    }
}
