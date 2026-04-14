namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS tokenizer error.</summary>
public class TokenizerError
{
    private readonly ParseError _code;

    /// <summary>Initializes a new instance of the <see cref="TokenizerError"/> class.</summary>
    public TokenizerError(ParseError code, TextPosition position)
    {
        _code = code;
        Position = position;
    }

    /// <summary>Gets the position.</summary>
    public TextPosition Position { get; }
    /// <summary>Gets the code.</summary>
    public int Code => _code.GetCode();
    /// <summary>Gets the message.</summary>
    public string Message => "An unknown error occurred.";
}
