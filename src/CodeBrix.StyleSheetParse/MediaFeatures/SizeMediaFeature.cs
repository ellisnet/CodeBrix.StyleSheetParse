namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

internal sealed class SizeMediaFeature : MediaFeature
{
    public SizeMediaFeature(string name) : base(name)
    {
    }

    internal override IValueConverter Converter => Converters.LengthConverter;
}
