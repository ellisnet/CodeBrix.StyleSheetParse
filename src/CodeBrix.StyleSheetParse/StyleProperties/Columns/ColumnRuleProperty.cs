using static CodeBrix.StyleSheetParse.Converters;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

internal sealed class ColumnRuleProperty : ShorthandProperty
{
    private static readonly IValueConverter StyleConverter = WithAny(
        ColorConverter.Option().For(PropertyNames.ColumnRuleColor),
        LineWidthConverter.Option().For(PropertyNames.ColumnRuleWidth),
        LineStyleConverter.Option().For(PropertyNames.ColumnRuleStyle)).OrDefault();

    internal ColumnRuleProperty()
        : base(PropertyNames.ColumnRule, PropertyFlags.Animatable)
    {
    }

    internal override IValueConverter Converter => StyleConverter;
}
