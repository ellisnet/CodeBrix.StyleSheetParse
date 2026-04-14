using System;

// ReSharper disable UnusedMember.Global

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS angle value.</summary>
public struct Angle : IEquatable<Angle>, IComparable<Angle>, IFormattable
{
    /// <summary>Gets the zero value.</summary>
    public static readonly Angle Zero = new(0f, Unit.Rad);
    /// <summary>Gets the half quarter value.</summary>
    public static readonly Angle HalfQuarter = new(45f, Unit.Deg);
    /// <summary>Gets the quarter value.</summary>
    public static readonly Angle Quarter = new(90f, Unit.Deg);
    /// <summary>Gets the triple half quarter value.</summary>
    public static readonly Angle TripleHalfQuarter = new(135f, Unit.Deg);
    /// <summary>Gets the half value.</summary>
    public static readonly Angle Half = new(180f, Unit.Deg);

    /// <summary>Initializes a new instance of the <see cref="Angle"/> class.</summary>
    public Angle(float value, Unit unit)
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
            switch (Type)
            {
                case Unit.Deg:
                    return UnitNames.Deg;

                case Unit.Grad:
                    return UnitNames.Grad;

                case Unit.Turn:
                    return UnitNames.Turn;

                case Unit.Rad:
                    return UnitNames.Rad;

                default:
                    return string.Empty;
            }
        }
    }

    /// <summary>
    ///     Compares the magnitude of two angles.
    /// </summary>
    public static bool operator >=(Angle a, Angle b)
    {
        var result = a.CompareTo(b);
        return result is 0 or 1;
    }

    /// <summary>
    ///     Compares the magnitude of two angles.
    /// </summary>
    public static bool operator >(Angle a, Angle b)
    {
        return a.CompareTo(b) == 1;
    }

    /// <summary>
    ///     Compares the magnitude of two angles.
    /// </summary>
    public static bool operator <=(Angle a, Angle b)
    {
        var result = a.CompareTo(b);
        return result is 0 or -1;
    }

    /// <summary>
    ///     Compares the magnitude of two angles.
    /// </summary>
    public static bool operator <(Angle a, Angle b)
    {
        return a.CompareTo(b) == -1;
    }

    /// <summary>
    ///     Compares the current angle against the given one.
    /// </summary>
    /// <param name="other">The angle to compare to.</param>
    /// <returns>The result of the comparison.</returns>
    public int CompareTo(Angle other)
    {
        return ToRadian().CompareTo(other.ToRadian());
    }

    /// <summary>Performs the try parse operation.</summary>
    public static bool TryParse(string s, out Angle result)
    {
        var unit = GetUnit(s.StylesheetUnit(out var value));

        if (unit != Unit.None)
        {
            result = new Angle(value, unit);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>Performs the get unit operation.</summary>
    public static Unit GetUnit(string s)
    {
        return s switch
        {
            "deg" => Unit.Deg,
            "grad" => Unit.Grad,
            "turn" => Unit.Turn,
            "rad" => Unit.Rad,
            _ => Unit.None
        };
    }

    /// <summary>Performs the to radian operation.</summary>
    public float ToRadian()
    {
        return Type switch
        {
            Unit.Deg => (float) (Math.PI / 180.0 * Value),
            Unit.Grad => (float) (Math.PI / 200.0 * Value),
            Unit.Turn => (float) (2.0 * Math.PI * Value),
            _ => Value
        };
    }

    /// <summary>Performs the to turns operation.</summary>
    public float ToTurns()
    {
        return Type switch
        {
            Unit.Deg => (float) (Value / 360.0),
            Unit.Grad => (float) (Value / 400.0),
            Unit.Rad => (float) (Value / (2.0 * Math.PI)),
            _ => Value
        };
    }

    /// <summary>Performs the equals operation.</summary>
    public bool Equals(Angle other)
    {
        return ToRadian() == other.ToRadian();
    }

    /// <summary>
    ///     An enumeration of angle representations.
    /// </summary>
    public enum Unit : byte
    {
        /// <summary>The none value.</summary>
        None,
        /// <summary>The deg value.</summary>
        Deg,
        /// <summary>The rad value.</summary>
        Rad,
        /// <summary>The grad value.</summary>
        Grad,
        /// <summary>The turn value.</summary>
        Turn
    }

    /// <summary>
    ///     Checks for equality of two angles.
    /// </summary>
    public static bool operator ==(Angle a, Angle b)
    {
        return a.Equals(b);
    }

    /// <summary>
    ///     Checks for inequality of two angles.
    /// </summary>
    public static bool operator !=(Angle a, Angle b)
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
        var other = obj as Angle?;

        return other != null && Equals(other.Value);
    }

    /// <summary>
    ///     Returns a hash code that defines the current angle.
    /// </summary>
    /// <returns>The integer value of the hashcode.</returns>
    public override int GetHashCode()
    {
        return (int) Value;
    }

    /// <summary>Performs the to string operation.</summary>
    public override string ToString()
    {
        return string.Concat(Value.ToString(), UnitString);
    }

    /// <summary>Performs the to string operation.</summary>
    public string ToString(string format, IFormatProvider formatProvider)
    {
        return string.Concat(Value.ToString(format, formatProvider), UnitString);
    }
}
