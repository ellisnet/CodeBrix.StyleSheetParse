using static CodeBrix.StyleSheetParse.Converters;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

internal sealed class BorderBottomProperty : ShorthandProperty
{
    private static readonly IValueConverter StyleConverter = WithAny(
        LineWidthConverter.Option().For(PropertyNames.BorderBottomWidth),
        LineStyleConverter.Option().For(PropertyNames.BorderBottomStyle),
        CurrentColorConverter.Option().For(PropertyNames.BorderBottomColor)
    ).OrDefault();

    internal BorderBottomProperty()
        : base(PropertyNames.BorderBottom, PropertyFlags.Animatable)
    {
    }

    internal override IValueConverter Converter => StyleConverter;
}
