using System;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS text position value.</summary>
public struct TextPosition : IEquatable<TextPosition>, IComparable<TextPosition>
{
    /// <summary>Gets the empty value.</summary>
    public static readonly TextPosition Empty = new();

    private readonly ushort _line;
    private readonly ushort _column;

    /// <summary>Initializes a new instance of the <see cref="TextPosition"/> class.</summary>
    public TextPosition(ushort line, ushort column, int position)
    {
        _line = line;
        _column = column;
        Position = position;
    }

    /// <summary>Gets the line.</summary>
    public int Line => _line;
    /// <summary>Gets the column.</summary>
    public int Column => _column;
    /// <summary>Gets the position.</summary>
    public int Position { get; }

    /// <summary>Performs the shift operation.</summary>
    public TextPosition Shift(int columns)
    {
        return new(_line, (ushort) (_column + columns), Position + columns);
    }

    /// <summary>Performs the after operation.</summary>
    public TextPosition After(char chr)
    {
        var line = _line;
        var column = _column;

        if (chr != Symbols.LineFeed) return new TextPosition(line, ++column, Position + 1);

        ++line;
        column = 0;

        return new TextPosition(line, ++column, Position + 1);
    }

    /// <summary>Performs the after operation.</summary>
    public TextPosition After(string str)
    {
        var line = _line;
        var column = _column;

        foreach (var chr in str)
        {
            if (chr == Symbols.LineFeed)
            {
                ++line;
                column = 0;
            }

            ++column;
        }

        return new TextPosition(line, column, Position + str.Length);
    }

    /// <summary>Performs the to string operation.</summary>
    public override string ToString()
    {
        return $"Line {_line}, Column {_column}, Position {Position}";
    }

    /// <summary>Gets the int.</summary>
    public override int GetHashCode()
    {
        return Position ^ ((_line | _column) + _line);
    }

    /// <summary>Performs the equals operation.</summary>
    public override bool Equals(object obj)
    {
        return obj is TextPosition other && Equals(other);
    }

    /// <summary>Performs the equals operation.</summary>
    public bool Equals(TextPosition other)
    {
        return Position == other.Position &&
               _column == other._column &&
               _line == other._line;
    }

    /// <summary>The operator member.</summary>
    public static bool operator >(TextPosition a, TextPosition b)
    {
        return a.Position > b.Position;
    }

    /// <summary>The operator member.</summary>
    public static bool operator <(TextPosition a, TextPosition b)
    {
        return a.Position < b.Position;
    }

    /// <summary>Performs the compare to operation.</summary>
    public int CompareTo(TextPosition other)
    {
        return Equals(other) ? 0 : this > other ? 1 : -1;
    }
}
