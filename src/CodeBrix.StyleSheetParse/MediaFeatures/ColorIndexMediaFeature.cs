using static CodeBrix.StyleSheetParse.Converters;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

internal sealed class ColorIndexMediaFeature : MediaFeature
{
    public ColorIndexMediaFeature(string name) : base(name)
    {
    }

    internal override IValueConverter Converter =>
        IsMinimum || IsMaximum
            ? NaturalIntegerConverter
            : NaturalIntegerConverter.Option(1);
}
