using System;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS percent value.</summary>
public struct Percent : IEquatable<Percent>, IComparable<Percent>, IFormattable
{
    /// <summary>
    ///     Gets a zero percent value.
    /// </summary>
    public static readonly Percent Zero = new(0f);

    /// <summary>
    ///     Gets a fifty percent value.
    /// </summary>
    public static readonly Percent Fifty = new(50f);

    /// <summary>
    ///     Gets a hundred percent value.
    /// </summary>
    public static readonly Percent Hundred = new(100f);

    /// <summary>Initializes a new instance of the <see cref="Percent"/> class.</summary>
    public Percent(float value)
    {
        Value = value;
    }

    /// <summary>Gets the normalized value.</summary>
    public float NormalizedValue => Value * 0.01f;
    /// <summary>Gets the value.</summary>
    public float Value { get; }

    /// <summary>
    ///     Compares the magnitude of two percents.
    /// </summary>
    public static bool operator >=(Percent a, Percent b)
    {
        return a.Value >= b.Value;
    }

    /// <summary>
    ///     Compares the magnitude of two percents.
    /// </summary>
    public static bool operator >(Percent a, Percent b)
    {
        return a.Value > b.Value;
    }

    /// <summary>
    ///     Compares the magnitude of two percents.
    /// </summary>
    public static bool operator <=(Percent a, Percent b)
    {
        return a.Value <= b.Value;
    }

    /// <summary>
    ///     Compares the magnitude of two percents.
    /// </summary>
    public static bool operator <(Percent a, Percent b)
    {
        return a.Value < b.Value;
    }

    /// <summary>
    ///     Compares the current percentage against the given one.
    /// </summary>
    /// <param name="other">The percentage to compare to.</param>
    /// <returns>The result of the comparison.</returns>
    public int CompareTo(Percent other)
    {
        return Value.CompareTo(other.Value);
    }

    /// <summary>
    ///     Checks if the given percent value is equal to the current one.
    /// </summary>
    /// <param name="other">The other percent value.</param>
    /// <returns>True if both have the same value.</returns>
    public bool Equals(Percent other)
    {
        return Value == other.Value;
    }

    /// <summary>
    ///     Checks for equality of two percents.
    /// </summary>
    public static bool operator ==(Percent a, Percent b)
    {
        return a.Equals(b);
    }

    /// <summary>
    ///     Checks for inequality of two percents.
    /// </summary>
    public static bool operator !=(Percent a, Percent b)
    {
        return !a.Equals(b);
    }

    /// <summary>
    ///     Tests if another object is equal to this object.
    /// </summary>
    /// <param name="obj">The object to test with.</param>
    /// <returns>True if the two objects are equal, otherwise false.</returns>
    public override bool Equals(object obj)
    {
        if (obj is Percent other) return Equals(other);

        return false;
    }

    /// <summary>Gets the int.</summary>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    /// <summary>Performs the to string operation.</summary>
    public override string ToString()
    {
        return Value + "%";
    }

    /// <summary>Performs the to string operation.</summary>
    public string ToString(string format, IFormatProvider formatProvider)
    {
        return Value.ToString(format, formatProvider) + "%";
    }
}
