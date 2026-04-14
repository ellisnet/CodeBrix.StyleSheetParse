using static CodeBrix.StyleSheetParse.Converters;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

internal sealed class VerticalAlignProperty : Property
{
    private static readonly IValueConverter StyleConverter = LengthOrPercentConverter.Or(
        VerticalAlignmentConverter).OrDefault(VerticalAlignment.Baseline);

    internal VerticalAlignProperty()
        : base(PropertyNames.VerticalAlign, PropertyFlags.Animatable)
    {
    }

    internal override IValueConverter Converter => StyleConverter;
}
