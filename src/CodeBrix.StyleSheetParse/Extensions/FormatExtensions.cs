using System.IO;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>The format extensions member.</summary>
public static class FormatExtensions
{
    /// <summary>Performs the to css operation.</summary>
    public static string ToCss(this IStyleFormattable style)
    {
        return style.ToCss(CompressedStyleFormatter.Instance);
    }

    /// <summary>Performs the to css operation.</summary>
    public static string ToCss(this IStyleFormattable style, IStyleFormatter formatter)
    {
        var sb = Pool.NewStringBuilder();
        using (var writer = new StringWriter(sb))
        {
            style.ToCss(writer, formatter);
        }

        return sb.ToPool();
    }

    /// <summary>Performs the to css operation.</summary>
    public static void ToCss(this IStyleFormattable style, TextWriter writer)
    {
        style.ToCss(writer, CompressedStyleFormatter.Instance);
    }
}
