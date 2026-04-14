namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

internal sealed class ColumnWidthProperty : Property
{
    private static readonly IValueConverter
        StyleConverter = Converters.AutoLengthConverter.OrDefault(Keywords.Auto);

    internal ColumnWidthProperty()
        : base(PropertyNames.ColumnWidth, PropertyFlags.Animatable)
    {
    }

    internal override IValueConverter Converter => StyleConverter;
}
