namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

internal sealed class WidthMediaFeature : MediaFeature
{
    public WidthMediaFeature(string name) : base(name)
    {
    }

    internal override IValueConverter Converter => Converters.LengthConverter;
}
