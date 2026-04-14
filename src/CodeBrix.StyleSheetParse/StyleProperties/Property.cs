using System.IO;

// ReSharper disable UnusedMember.Global

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS property.</summary>
public abstract class Property : StylesheetNode, IProperty
{
    private readonly PropertyFlags _flags;

    internal Property(string name, PropertyFlags flags = PropertyFlags.None)
    {
        Name = name;
        _flags = flags;
    }

    /// <summary>Performs the to css operation.</summary>
    public override void ToCss(TextWriter writer, IStyleFormatter formatter)
    {
        writer.Write(formatter.Declaration(Name, Value, IsImportant));
    }

    internal bool TrySetValue(TokenValue newTokenValue)
    {
        var value = Converter.Convert(newTokenValue ?? TokenValue.Initial);

        if (value == null) return false;
        DeclaredValue = value;
        return true;
    }

    /// <summary>Gets the value.</summary>
    public string Value => DeclaredValue != null ? DeclaredValue.CssText : Keywords.Initial;

    /// <summary>Gets the original.</summary>
    public string Original => DeclaredValue != null ? DeclaredValue.Original.Text : Keywords.Initial;

    /// <summary>Gets the is inherited.</summary>
    public bool IsInherited => (_flags & PropertyFlags.Inherited) == PropertyFlags.Inherited && IsInitial ||
                               DeclaredValue != null && DeclaredValue.CssText.Is(Keywords.Inherit);

    /// <summary>Gets the is animatable.</summary>
    public bool IsAnimatable => (_flags & PropertyFlags.Animatable) == PropertyFlags.Animatable;

    /// <summary>Gets the is initial.</summary>
    public bool IsInitial => DeclaredValue == null || DeclaredValue.CssText.Is(Keywords.Initial);

    internal bool HasValue => DeclaredValue != null;

    internal bool CanBeHashless => (_flags & PropertyFlags.Hashless) == PropertyFlags.Hashless;

    internal bool CanBeUnitless => (_flags & PropertyFlags.Unitless) == PropertyFlags.Unitless;

    /// <summary>Gets the can be inherited.</summary>
    public bool CanBeInherited => (_flags & PropertyFlags.Inherited) == PropertyFlags.Inherited;

    internal bool IsShorthand => (_flags & PropertyFlags.Shorthand) == PropertyFlags.Shorthand;

    /// <summary>Gets the name.</summary>
    public string Name { get; }

    /// <summary>Gets or sets the is important.</summary>
    public bool IsImportant { get; set; }

    /// <summary>Gets the css text.</summary>
    public string CssText => this.ToCss();

    internal abstract IValueConverter Converter { get; }

    internal IPropertyValue DeclaredValue { get; set; }
}
