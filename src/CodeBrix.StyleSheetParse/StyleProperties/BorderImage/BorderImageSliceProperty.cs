using static CodeBrix.StyleSheetParse.Converters;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

internal sealed class BorderImageSliceProperty : Property
{
    internal static readonly IValueConverter TheConverter = WithAny(
        BorderSliceConverter.Option(new Length(100f, Length.Unit.Percent)),
        BorderSliceConverter.Option(),
        BorderSliceConverter.Option(),
        BorderSliceConverter.Option(),
        Assign(Keywords.Fill, true).Option(false));

    private static readonly IValueConverter StyleConverter = TheConverter.OrDefault(Length.Full);

    internal BorderImageSliceProperty()
        : base(PropertyNames.BorderImageSlice)
    {
    }

    internal override IValueConverter Converter => StyleConverter;
}
