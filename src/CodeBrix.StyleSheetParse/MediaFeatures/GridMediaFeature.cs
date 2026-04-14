namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

internal sealed class GridMediaFeature : MediaFeature
{
    public GridMediaFeature() : base(FeatureNames.Grid)
    {
    }

    internal override IValueConverter Converter => Converters.BinaryConverter;
}
