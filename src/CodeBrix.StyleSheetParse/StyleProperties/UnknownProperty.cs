namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

internal sealed class UnknownProperty : Property
{
    internal UnknownProperty(string name)
        : base(name)
    {
    }

    internal override IValueConverter Converter => Converters.Any;
}
