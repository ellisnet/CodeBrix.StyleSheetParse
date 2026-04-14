using System;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS text range value.</summary>
public struct TextRange : IEquatable<TextRange>, IComparable<TextRange>
{
    /// <summary>Initializes a new instance of the <see cref="TextRange"/> class.</summary>
    public TextRange(TextPosition start, TextPosition end)
    {
        Start = start;
        End = end;
    }

    /// <summary>Gets the start.</summary>
    public TextPosition Start { get; }
    /// <summary>Gets the end.</summary>
    public TextPosition End { get; }

    /// <summary>Performs the to string operation.</summary>
    public override string ToString()
    {
        return $"({Start}) -- ({End})";
    }

    /// <summary>Gets the int.</summary>
    public override int GetHashCode()
    {
        return End.GetHashCode() ^ Start.GetHashCode();
    }

    /// <summary>Performs the equals operation.</summary>
    public override bool Equals(object obj)
    {
        return obj is TextRange other && Equals(other);
    }

    /// <summary>Performs the equals operation.</summary>
    public bool Equals(TextRange other)
    {
        return Start.Equals(other.Start) && End.Equals(other.End);
    }

    /// <summary>The operator member.</summary>
    public static bool operator >(TextRange a, TextRange b)
    {
        return a.Start > b.End;
    }

    /// <summary>The operator member.</summary>
    public static bool operator <(TextRange a, TextRange b)
    {
        return a.End < b.Start;
    }

    /// <summary>Performs the compare to operation.</summary>
    public int CompareTo(TextRange other)
    {
        if (this > other) return 1;

        if (other > this) return -1;

        return 0;
    }
}
