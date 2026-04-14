using static CodeBrix.StyleSheetParse.Converters;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

internal sealed class ColorMediaFeature : MediaFeature
{
    public ColorMediaFeature(string name) : base(name)
    {
    }

    internal override IValueConverter Converter =>
        IsMinimum || IsMaximum
            ? PositiveIntegerConverter
            : PositiveIntegerConverter.Option(1);
}
