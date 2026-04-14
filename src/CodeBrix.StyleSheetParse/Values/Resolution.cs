using System;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS resolution value.</summary>
public struct Resolution : IEquatable<Resolution>, IComparable<Resolution>, IFormattable
{
    /// <summary>Initializes a new instance of the <see cref="Resolution"/> class.</summary>
    public Resolution(float value, Unit unit)
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
                Unit.Dpcm => UnitNames.Dpcm,
                Unit.Dpi => UnitNames.Dpi,
                Unit.Dppx => UnitNames.Dppx,
                _ => string.Empty
            };
        }
    }

    /// <summary>Performs the try parse operation.</summary>
    public static bool TryParse(string s, out Resolution result)
    {
        var unit = GetUnit(s.StylesheetUnit(out var value));

        if (unit != Unit.None)
        {
            result = new Resolution(value, unit);
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
            "dpcm" => Unit.Dpcm,
            "dpi" => Unit.Dpi,
            "dppx" => Unit.Dppx,
            _ => Unit.None
        };
    }

    /// <summary>Performs the to dots per pixel operation.</summary>
    public float ToDotsPerPixel()
    {
        if (Type == Unit.Dpi) return Value / 96f;
        if (Type == Unit.Dpcm) return Value * 127f / (50f * 96f);

        return Value;
    }

    /// <summary>Performs the to operation.</summary>
    public float To(Unit unit)
    {
        var value = ToDotsPerPixel();

        if (unit == Unit.Dpi) return value * 96f;
        if (unit == Unit.Dpcm) return value * 50f * 96f / 127f;

        return value;
    }

    /// <summary>Performs the equals operation.</summary>
    public bool Equals(Resolution other)
    {
        return Value == other.Value && Type == other.Type;
    }

    /// <summary>Specifies the unit options.</summary>
    public enum Unit : byte
    {
        /// <summary>The none value.</summary>
        None,
        /// <summary>The dpi value.</summary>
        Dpi,
        /// <summary>The dpcm value.</summary>
        Dpcm,
        /// <summary>The dppx value.</summary>
        Dppx
    }

    /// <summary>Performs the compare to operation.</summary>
    public int CompareTo(Resolution other)
    {
        return ToDotsPerPixel().CompareTo(other.ToDotsPerPixel());
    }

    /// <summary>Performs the equals operation.</summary>
    public override bool Equals(object obj)
    {
        if (obj is Resolution other) return Equals(other);

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
        return string.Concat(Value.ToString(), UnitString);
    }

    /// <summary>Performs the to string operation.</summary>
    public string ToString(string format, IFormatProvider formatProvider)
    {
        return string.Concat(Value.ToString(format, formatProvider), UnitString);
    }
}
