using static CodeBrix.StyleSheetParse.Converters;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

internal sealed class ListStyleProperty : ShorthandProperty
{
    private static readonly IValueConverter StyleConverter = WithAny(
        ListStyleConverter.Option().For(PropertyNames.ListStyleType),
        ListPositionConverter.Option().For(PropertyNames.ListStylePosition),
        OptionalImageSourceConverter.Option().For(PropertyNames.ListStyleImage)).OrDefault();

    internal ListStyleProperty()
        : base(PropertyNames.ListStyle, PropertyFlags.Inherited)
    {
    }

    internal override IValueConverter Converter => StyleConverter;
}
