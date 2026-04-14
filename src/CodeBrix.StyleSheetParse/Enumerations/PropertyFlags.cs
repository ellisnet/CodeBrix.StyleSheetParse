using System;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

[Flags]
internal enum PropertyFlags : byte
{
    None = 0,
    Inherited = 1,
    Hashless = 2,
    Unitless = 4,
    Animatable = 8,
    Shorthand = 16
}
