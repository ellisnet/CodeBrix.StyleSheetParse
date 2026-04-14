namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

internal sealed class OutlineColorProperty : Property
{
    private static readonly IValueConverter StyleConverter =
        Converters.InvertedColorConverter.OrDefault(Color.Transparent);

    internal OutlineColorProperty()
        : base(PropertyNames.OutlineColor, PropertyFlags.Animatable)
    {
    }

    internal override IValueConverter Converter => StyleConverter;
}
