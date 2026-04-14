using System;

// ReSharper disable UnusedMember.Global

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS number value.</summary>
public struct Number : IEquatable<Number>, IComparable<Number>, IFormattable
{
    /// <summary>
    ///     Gets a zero value.
    /// </summary>
    public static readonly Number Zero = new(0f, Unit.Integer);

    /// <summary>
    ///     Gets the positive infinite value.
    /// </summary>
    public static readonly Number Infinite = new(float.PositiveInfinity, Unit.Float);

    /// <summary>
    ///     Gets the neutral element.
    /// </summary>
    public static readonly Number One = new(1f, Unit.Integer);

    private readonly Unit _unit;

    /// <summary>Initializes a new instance of the <see cref="Number"/> class.</summary>
    public Number(float value, Unit unit)
    {
        Value = value;
        _unit = unit;
    }

    /// <summary>Gets the value.</summary>
    public float Value { get; }
    /// <summary>Gets the is integer.</summary>
    public bool IsInteger => _unit == Unit.Integer;

    /// <summary>The operator member.</summary>
    public static bool operator >=(Number a, Number b)
    {
        return a.Value >= b.Value;
    }

    /// <summary>The operator member.</summary>
    public static bool operator >(Number a, Number b)
    {
        return a.Value > b.Value;
    }

    /// <summary>The operator member.</summary>
    public static bool operator <=(Number a, Number b)
    {
        return a.Value <= b.Value;
    }

    /// <summary>The operator member.</summary>
    public static bool operator <(Number a, Number b)
    {
        return a.Value < b.Value;
    }

    /// <summary>Performs the compare to operation.</summary>
    public int CompareTo(Number other)
    {
        return Value.CompareTo(other.Value);
    }

    /// <summary>Performs the equals operation.</summary>
    public bool Equals(Number other)
    {
        return Value == other.Value && _unit == other._unit;
    }

    /// <summary>Specifies the unit options.</summary>
    public enum Unit : byte
    {
        /// <summary>The integer value.</summary>
        Integer,
        /// <summary>The float value.</summary>
        Float,
        /// <summary>The percent value.</summary>
        Percent
    }

    /// <summary>The operator member.</summary>
    public static bool operator ==(Number a, Number b)
    {
        return a.Value == b.Value;
    }

    /// <summary>The operator member.</summary>
    public static bool operator !=(Number a, Number b)
    {
        return a.Value != b.Value;
    }

    /// <summary>Performs the equals operation.</summary>
    public override bool Equals(object obj)
    {
        return obj is Number other && Equals(other);
    }

    /// <summary>Gets the int.</summary>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    /// <summary>Performs the to string operation.</summary>
    public override string ToString()
    {
        return Value.ToString() + (_unit == Unit.Percent ? "%" : string.Empty);
    }

    /// <summary>Performs the to string operation.</summary>
    public string ToString(string format, IFormatProvider formatProvider)
    {
        return Value.ToString(format, formatProvider) + (_unit == Unit.Percent ? "%" : string.Empty);
    }
}
