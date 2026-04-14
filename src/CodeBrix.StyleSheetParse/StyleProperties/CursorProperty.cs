using static CodeBrix.StyleSheetParse.Converters;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

internal sealed class CursorProperty : Property
{
    private static readonly IValueConverter StyleConverter = ImageSourceConverter.Or(
        WithOrder(
            ImageSourceConverter.Required(),
            NumberConverter.Required(),
            NumberConverter.Required())).RequiresEnd(
        Map.Cursors.ToConverter()).OrDefault(SystemCursor.Auto);

    internal CursorProperty()
        : base(PropertyNames.Cursor, PropertyFlags.Inherited)
    {
    }

    internal override IValueConverter Converter => StyleConverter;
}
