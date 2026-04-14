using static CodeBrix.StyleSheetParse.Converters;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

internal sealed class CounterIncrementProperty : Property
{
    private static readonly IValueConverter StyleConverter = Continuous(
        WithOrder(IdentifierConverter.Required(), IntegerConverter.Option(1))).OrDefault();

    internal CounterIncrementProperty()
        : base(PropertyNames.CounterIncrement)
    {
    }

    internal override IValueConverter Converter => StyleConverter;
}
