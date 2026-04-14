using static CodeBrix.StyleSheetParse.Converters;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

internal sealed class TransitionProperty : ShorthandProperty
{
    internal static readonly IValueConverter ListConverter = WithAny(
        AnimatableConverter.Option().For(PropertyNames.TransitionProperty),
        TimeConverter.Option().For(PropertyNames.TransitionDuration),
        TransitionConverter.Option().For(PropertyNames.TransitionTimingFunction),
        TimeConverter.Option().For(PropertyNames.TransitionDelay)).FromList().OrDefault();

    internal TransitionProperty()
        : base(PropertyNames.Transition)
    {
    }

    internal override IValueConverter Converter => ListConverter;
}
