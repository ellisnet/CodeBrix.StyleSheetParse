using static CodeBrix.StyleSheetParse.Converters;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

internal sealed class CounterResetProperty : Property
{
    private static readonly IValueConverter StyleConverter = Continuous(
        WithOrder(IdentifierConverter.Required(), IntegerConverter.Option(0))).OrDefault();

    internal CounterResetProperty()
        : base(PropertyNames.CounterReset)
    {
    }

    internal override IValueConverter Converter => StyleConverter;
}
