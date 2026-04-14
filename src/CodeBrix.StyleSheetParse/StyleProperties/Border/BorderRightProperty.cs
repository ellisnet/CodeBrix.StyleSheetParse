using static CodeBrix.StyleSheetParse.Converters;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

internal sealed class BorderRightProperty : ShorthandProperty
{
    private static readonly IValueConverter StyleConverter = WithAny(
        LineWidthConverter.Option().For(PropertyNames.BorderRightWidth),
        LineStyleConverter.Option().For(PropertyNames.BorderRightStyle),
        CurrentColorConverter.Option().For(PropertyNames.BorderRightColor)
    ).OrDefault();

    internal BorderRightProperty()
        : base(PropertyNames.BorderRight, PropertyFlags.Animatable)
    {
    }

    internal override IValueConverter Converter => StyleConverter;
}
