namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS counter.</summary>
public sealed class Counter
{
    /// <summary>Initializes a new instance of the <see cref="Counter"/> class.</summary>
    public Counter(string identifier, string listStyle, string separator)
    {
        CounterIdentifier = identifier;
        ListStyle = listStyle;
        DefinedSeparator = separator;
    }

    /// <summary>Gets the counter identifier.</summary>
    public string CounterIdentifier { get; }
    /// <summary>Gets the list style.</summary>
    public string ListStyle { get; }
    /// <summary>Gets the defined separator.</summary>
    public string DefinedSeparator { get; }
}
