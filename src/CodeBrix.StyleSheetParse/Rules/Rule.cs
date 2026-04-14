namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS rule.</summary>
public abstract class Rule : StylesheetNode, IRule
{
    private IRule _parentRule;

    internal Rule(RuleType type, StylesheetParser parser)
    {
        Type = type;
        Parser = parser;
    }

    internal StylesheetParser Parser { get; }
    /// <summary>Gets or sets the owner.</summary>
    public Stylesheet Owner { get; internal set; }
    /// <summary>Gets the type.</summary>
    public RuleType Type { get; }

    /// <summary>The text member.</summary>
    public string Text
    {
        get => this.ToCss();
        set
        {
            var rule = Parser.ParseRule(value);

            if (rule == null) throw new ParseException("Unable to parse rule");
            if (rule.Type != Type) throw new ParseException("Invalid rule type");
            ReplaceWith(rule);
        }
    }

    /// <summary>The parent member.</summary>
    public IRule Parent
    {
        get => _parentRule;
        internal set
        {
            _parentRule = value;

            if (value != null) Owner = _parentRule.Owner;
        }
    }

    /// <summary>The replace with member.</summary>
    protected virtual void ReplaceWith(IRule rule)
    {
        ReplaceAll(rule);
    }

    /// <summary>The replace single member.</summary>
    protected void ReplaceSingle(IStylesheetNode oldNode, IStylesheetNode newNode)
    {
        if (oldNode != null)
        {
            if (newNode != null)
            {
                ReplaceChild(oldNode, newNode);
            }
            else
            {
                RemoveChild(oldNode);
            }
        }
        else if (newNode != null)
        {
            AppendChild(newNode);
        }
    }
}
