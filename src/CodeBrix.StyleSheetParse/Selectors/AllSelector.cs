namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS all selector.</summary>
public sealed class AllSelector : SelectorBase
{
    /// <summary>Performs the create operation.</summary>
    public static AllSelector Create()
    {
        return new AllSelector();
    }

    private AllSelector() : base(Priority.Zero, "*")
    {
    }
}
