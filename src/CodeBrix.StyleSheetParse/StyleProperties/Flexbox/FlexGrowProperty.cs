namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

internal sealed class FlexGrowProperty : Property
{
    private static readonly IValueConverter StyleConverter = Converters.FlexGrowShrinkConverter;

    internal FlexGrowProperty()
        : base(PropertyNames.FlexGrow)
    { }

    internal override IValueConverter Converter => StyleConverter;
}
