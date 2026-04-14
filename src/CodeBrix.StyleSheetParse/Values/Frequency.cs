using System;

// ReSharper disable UnusedMember.Global

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS frequency value.</summary>
public struct Frequency : IEquatable<Frequency>, IComparable<Frequency>, IFormattable
{
    /// <summary>Initializes a new instance of the <see cref="Frequency"/> class.</summary>
    public Frequency(float value, Unit unit)
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
                case Unit.Khz:
                    return UnitNames.Khz;

                case Unit.Hz:
                    return UnitNames.Hz;

                default:
                    return string.Empty;
            }
        }
    }

    /// <summary>The operator member.</summary>
    public static bool operator >=(Frequency a, Frequency b)
    {
        var result = a.CompareTo(b);
        return result == 0 || result == 1;
    }

    /// <summary>The operator member.</summary>
    public static bool operator >(Frequency a, Frequency b)
    {
        return a.CompareTo(b) == 1;
    }

    /// <summary>The operator member.</summary>
    public static bool operator <=(Frequency a, Frequency b)
    {
        var result = a.CompareTo(b);
        return result == 0 || result == -1;
    }

    /// <summary>The operator member.</summary>
    public static bool operator <(Frequency a, Frequency b)
    {
        return a.CompareTo(b) == -1;
    }

    /// <summary>Performs the compare to operation.</summary>
    public int CompareTo(Frequency other)
    {
        return ToHertz().CompareTo(other.ToHertz());
    }

    /// <summary>Performs the try parse operation.</summary>
    public static bool TryParse(string s, out Frequency result)
    {
        var unit = GetUnit(s.StylesheetUnit(out var value));

        if (unit != Unit.None)
        {
            result = new Frequency(value, unit);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>Performs the get unit operation.</summary>
    public static Unit GetUnit(string s)
    {
        switch (s)
        {
            case "hz":
                return Unit.Hz;
            case "khz":
                return Unit.Khz;
            default:
                return Unit.None;
        }
    }

    /// <summary>Performs the to hertz operation.</summary>
    public float ToHertz()
    {
        return Type == Unit.Khz ? Value * 1000f : Value;
    }

    /// <summary>Performs the equals operation.</summary>
    public bool Equals(Frequency other)
    {
        return Value == other.Value && Type == other.Type;
    }

    /// <summary>Specifies the unit options.</summary>
    public enum Unit : byte
    {
        /// <summary>The none value.</summary>
        None,
        /// <summary>The hz value.</summary>
        Hz,
        /// <summary>The khz value.</summary>
        Khz
    }

    /// <summary>The operator member.</summary>
    public static bool operator ==(Frequency a, Frequency b)
    {
        return a.Equals(b);
    }

    /// <summary>The operator member.</summary>
    public static bool operator !=(Frequency a, Frequency b)
    {
        return !a.Equals(b);
    }

    /// <summary>Performs the equals operation.</summary>
    public override bool Equals(object obj)
    {
        var other = obj as Frequency?;

        return other != null && Equals(other.Value);
    }

    /// <summary>Gets the int.</summary>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
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
