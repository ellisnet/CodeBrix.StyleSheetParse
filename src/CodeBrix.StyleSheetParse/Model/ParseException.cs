using System;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS parse exception.</summary>
public class ParseException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="ParseException"/> class.</summary>
    public ParseException(string message) : base(message)
    {
    }
}
