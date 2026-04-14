using System.IO;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS styleformattable node.</summary>
public interface IStyleFormattable
{
    /// <summary>The to css member.</summary>
    void ToCss(TextWriter writer, IStyleFormatter formatter);
}
