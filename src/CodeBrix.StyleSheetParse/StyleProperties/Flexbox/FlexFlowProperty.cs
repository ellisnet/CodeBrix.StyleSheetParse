namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

internal sealed class FlexFlowProperty : ShorthandProperty
{
    private static readonly IValueConverter StyleConverter = Converters.FlexFlowConverter;

    internal FlexFlowProperty()
        : base(PropertyNames.FlexFlow)
    { }

    internal override IValueConverter Converter => StyleConverter;
}
