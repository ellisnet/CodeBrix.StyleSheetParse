namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

internal sealed class AlignContentProperty : Property
{
    private static readonly IValueConverter StyleConverter = Converters.AlignContentConverter;

    internal AlignContentProperty()
        : base(PropertyNames.AlignContent)
    { }

    internal override IValueConverter Converter => StyleConverter;
}
