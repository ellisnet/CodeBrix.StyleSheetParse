using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS medium.</summary>
public sealed class Medium : StylesheetNode
{
    /// <summary>Gets the features.</summary>
    public IEnumerable<MediaFeature> Features => Children.OfType<MediaFeature>();
    /// <summary>Gets or sets the type.</summary>
    public string Type { get; internal set; }
    /// <summary>Gets or sets the is exclusive.</summary>
    public bool IsExclusive { get; internal set; }
    /// <summary>Gets or sets the is inverse.</summary>
    public bool IsInverse { get; internal set; }

    /// <summary>The constraints member.</summary>
    public string Constraints
    {
        get
        {
            var constraints = Features.Select(m => m.ToCss());
            return string.Join(" and ", constraints);
        }
    }

    /// <summary>Performs the equals operation.</summary>
    public override bool Equals(object obj)
    {
        if (obj is Medium other &&
            other.IsExclusive == IsExclusive &&
            other.IsInverse == IsInverse &&
            other.Type.Is(Type) &&
            other.Features.Count() == Features.Count())
        { 
            return other.Features.Select(feature =>
                Features.Any(m => m.Name.Is(feature.Name) && m.Value.Is(feature.Value))).All(isShared => isShared);
        }

        return false;
    }

    /// <summary>Gets the int.</summary>
    public override int GetHashCode()
    {
        // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
        return base.GetHashCode();
    }

    /// <summary>Performs the to css operation.</summary>
    public override void ToCss(TextWriter writer, IStyleFormatter formatter)
    {
        writer.Write(formatter.Medium(IsExclusive, IsInverse, Type, Features));
    }
}
