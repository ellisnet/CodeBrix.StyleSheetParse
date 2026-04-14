using System;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS time value.</summary>
public struct Time : IEquatable<Time>, IComparable<Time>, IFormattable
{
    /// <summary>Gets the zero value.</summary>
    public static readonly Time Zero = new(0f, Unit.Ms);

    /// <summary>Initializes a new instance of the <see cref="Time"/> class.</summary>
    public Time(float value, Unit unit)
    {
        Value = value;
        Type = unit;
    }

    /// <summary>Gets the value.</summary>
    public float Value { get; }
    /// <summary>Gets the type.</summary>
    public Unit Type { get; }

    /// <summary>The unit string member.</summary>
    public string UnitString
    {
        get
        {
            return Type switch
            {
                Unit.Ms => UnitNames.Ms,
                Unit.S => UnitNames.S,
                _ => string.Empty
            };
        }
    }

    /// <summary>
    ///     Compares the magnitude of two times.
    /// </summary>
    public static bool operator >=(Time a, Time b)
    {
        var result = a.CompareTo(b);
        return result == 0 || result == 1;
    }

    /// <summary>
    ///     Compares the magnitude of two times.
    /// </summary>
    public static bool operator >(Time a, Time b)
    {
        return a.CompareTo(b) == 1;
    }

    /// <summary>
    ///     Compares the magnitude of two times.
    /// </summary>
    public static bool operator <=(Time a, Time b)
    {
        var result = a.CompareTo(b);
        return result == 0 || result == -1;
    }

    /// <summary>
    ///     Compares the magnitude of two times.
    /// </summary>
    public static bool operator <(Time a, Time b)
    {
        return a.CompareTo(b) == -1;
    }

    /// <summary>Performs the compare to operation.</summary>
    public int CompareTo(Time other)
    {
        return ToMilliseconds().CompareTo(other.ToMilliseconds());
    }

   
    /// <summary>Performs the get unit operation.</summary>
    public static Unit GetUnit(string s)
    {
        switch (s)
        {
            case "s":
                return Unit.S;
            case "ms":
                return Unit.Ms;
            default:
                return Unit.None;
        }
    }

    /// <summary>Performs the to milliseconds operation.</summary>
    public float ToMilliseconds()
    {
        return Type == Unit.S ? Value * 1000f : Value;
    }

    /// <summary>Performs the equals operation.</summary>
    public bool Equals(Time other)
    {
        return ToMilliseconds() == other.ToMilliseconds();
    }

    /// <summary>Specifies the unit options.</summary>
    public enum Unit : byte
    {
        /// <summary>The none value.</summary>
        None,
        /// <summary>The ms value.</summary>
        Ms,
        /// <summary>The s value.</summary>
        S
    }

    /// <summary>
    ///     Checks for equality of two times.
    /// </summary>
    public static bool operator ==(Time a, Time b)
    {
        return a.Equals(b);
    }

    /// <summary>
    ///     Checks for inequality of two times.
    /// </summary>
    public static bool operator !=(Time a, Time b)
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
        if (obj is Time other) return Equals(other);

        return false;
    }

    /// <summary>
    ///     Returns a hash code that defines the current time.
    /// </summary>
    /// <returns>The integer value of the hashcode.</returns>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    /// <summary>
    ///     Returns a string representing the time.
    /// </summary>
    /// <returns>The unit string.</returns>
    public override string ToString()
    {
        return string.Concat(Value.ToString(), UnitString);
    }

    /// <summary>
    ///     Returns a formatted string representing the time.
    /// </summary>
    /// <param name="format">The format of the number.</param>
    /// <param name="formatProvider">The provider to use.</param>
    /// <returns>The unit string.</returns>
    public string ToString(string format, IFormatProvider formatProvider)
    {
        return string.Concat(Value.ToString(format, formatProvider), UnitString);
    }
}
