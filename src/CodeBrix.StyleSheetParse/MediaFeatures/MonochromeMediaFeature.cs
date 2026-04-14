using static CodeBrix.StyleSheetParse.Converters;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

internal sealed class MonochromeMediaFeature : MediaFeature
{
    public MonochromeMediaFeature(string name) : base(name)
    {
    }

    internal override IValueConverter Converter =>
        IsMinimum || IsMaximum ? NaturalIntegerConverter : NaturalIntegerConverter.Option(1);
}
