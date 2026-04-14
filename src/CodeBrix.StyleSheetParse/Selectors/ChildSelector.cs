using System.IO;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS child selector.</summary>
public abstract class ChildSelector : StylesheetNode, ISelector
{
    private readonly string _name;
    /// <summary>Gets or sets the step.</summary>
    public int Step { get; private set; }
    /// <summary>Gets or sets the offset.</summary>
    public int Offset { get; private set; }
    /// <summary>The kind member.</summary>
    protected ISelector Kind;

    /// <summary>The child selector member.</summary>
    protected ChildSelector(string name)
    {
        _name = name;
    }

    /// <summary>Performs the to css operation.</summary>
    public override void ToCss(TextWriter writer, IStyleFormatter formatter)
    {
        var a = Step.ToString();

        var b = Offset switch
        {
            > 0 => "+" + Offset,
            < 0 => Offset.ToString(),
            _ => string.Empty
        };

        writer.Write(":{0}({1}n{2})", _name, a, b);
    }

    /// <summary>Gets the specificity.</summary>
    public Priority Specificity => Priority.OneClass;
    /// <summary>Gets the text.</summary>
    public string Text => this.ToCss();

    internal ChildSelector With(int step, int offset, ISelector kind)
    {
        Step = step;
        Offset = offset;
        Kind = kind;
        return this;
    }
}
