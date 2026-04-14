using System;
using System.IO;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS media feature.</summary>
public abstract class MediaFeature : StylesheetNode, IMediaFeature
{
    private TokenValue _tokenValue;
    private TokenType _constraintDelimiter;

    internal MediaFeature(string name)
    {
        Name = name;
        IsMinimum = name.StartsWith("min-");
        IsMaximum = name.StartsWith("max-");
    }

    internal abstract IValueConverter Converter { get; }

    /// <summary>Gets the is minimum.</summary>
    public bool IsMinimum { get; }

    /// <summary>Gets the is maximum.</summary>
    public bool IsMaximum { get; }

    /// <summary>Gets the name.</summary>
    public string Name { get; }

    /// <summary>Gets the value.</summary>
    public string Value => HasValue ? _tokenValue.Text : string.Empty;

    /// <summary>Gets the has value.</summary>
    public bool HasValue => _tokenValue is {Count: > 0};

    /// <summary>Performs the to css operation.</summary>
    public override void ToCss(TextWriter writer, IStyleFormatter formatter)
    {
        var constraintDelimiter = GetConstraintDelimiter();
        var value = HasValue ? Value : null;
        writer.Write(formatter.Constraint(Name, value, GetConstraintDelimiter()));
    }

    private string GetConstraintDelimiter()
    {
        if (_constraintDelimiter == TokenType.Colon)
            return ": ";
        if (_constraintDelimiter == TokenType.GreaterThan)
            return " > ";
        if (_constraintDelimiter == TokenType.LessThan)
            return " < ";
        if (_constraintDelimiter == TokenType.Equal)
            return " = ";
        if (_constraintDelimiter == TokenType.GreaterThanOrEqual)
            return " >= ";
        if (_constraintDelimiter == TokenType.LessThanOrEqual)
            return " <= ";
        return ": ";
    }

    internal bool TrySetValue(TokenValue tokenValue, TokenType constraintDelimiter)
    {
        bool result;

        if (tokenValue == null)
            result = !IsMinimum && !IsMaximum && Converter.ConvertDefault() != null;
        else
            result = Converter.Convert(tokenValue) != null;

        if (result) _tokenValue = tokenValue;

        _constraintDelimiter = constraintDelimiter;

        return result;
    }
}
