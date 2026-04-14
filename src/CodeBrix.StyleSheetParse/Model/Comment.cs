using System.IO;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

internal sealed class Comment : StylesheetNode
{
    public Comment(string data)
    {
        Data = data;
    }

    public string Data { get; }

    public override void ToCss(TextWriter writer, IStyleFormatter formatter)
    {
        writer.Write(formatter.Comment(Data));
    }
}
