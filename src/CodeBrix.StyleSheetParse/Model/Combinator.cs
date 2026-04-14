namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS combinator.</summary>
public abstract class Combinator
{
    /// <summary>Gets the child value.</summary>
    public static readonly Combinator Child = new ChildCombinator();
    /// <summary>Gets the deep value.</summary>
    public static readonly Combinator Deep = new DeepCombinator();
    /// <summary>Gets the descendent value.</summary>
    public static readonly Combinator Descendent = new DescendentCombinator();
    /// <summary>Gets the adjacent sibling value.</summary>
    public static readonly Combinator AdjacentSibling = new AdjacentSiblingCombinator();
    /// <summary>Gets the sibling value.</summary>
    public static readonly Combinator Sibling = new SiblingCombinator();
    /// <summary>Gets the namespace value.</summary>
    public static readonly Combinator Namespace = new NamespaceCombinator();
    /// <summary>Gets the column value.</summary>
    public static readonly Combinator Column = new ColumnCombinator();
    /// <summary>Gets or sets the delimiter.</summary>
    public string Delimiter { get; protected set; }

    /// <summary>Performs the change operation.</summary>
    public virtual ISelector Change(ISelector selector)
    {
        return selector;
    }

    private sealed class ChildCombinator : Combinator
    {
        public ChildCombinator()
        {
            Delimiter = Combinators.Child;
        }
    }

    private sealed class DeepCombinator : Combinator
    {
        public DeepCombinator()
        {
            Delimiter = Combinators.Deep;
        }
    }

    private sealed class DescendentCombinator : Combinator
    {
        public DescendentCombinator()
        {
            Delimiter = Combinators.Descendent;
        }
    }

    private sealed class AdjacentSiblingCombinator : Combinator
    {
        public AdjacentSiblingCombinator()
        {
            Delimiter = Combinators.Adjacent;
        }
    }

    private sealed class SiblingCombinator : Combinator
    {
        public SiblingCombinator()
        {
            Delimiter = Combinators.Sibling;
        }
    }

    private sealed class NamespaceCombinator : Combinator
    {
        public NamespaceCombinator()
        {
            Delimiter = Combinators.Pipe;
        }

        public override ISelector Change(ISelector selector)
        {
            return NamespaceSelector.Create(selector.Text);
        }
    }

    private sealed class ColumnCombinator : Combinator
    {
        public ColumnCombinator()
        {
            Delimiter = Combinators.Column;
        }
    }
}
