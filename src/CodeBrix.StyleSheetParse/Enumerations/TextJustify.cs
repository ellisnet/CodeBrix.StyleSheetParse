namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Specifies the text justify options.</summary>
public enum TextJustify : byte
{
    /// <summary>The auto value.</summary>
    Auto,
    /// <summary>The inter word value.</summary>
    InterWord,
    /// <summary>The inter ideograph value.</summary>
    InterIdeograph,
    /// <summary>The inter cluster value.</summary>
    InterCluster,
    /// <summary>The distribute value.</summary>
    Distribute,
    /// <summary>The distribute all lines value.</summary>
    DistributeAllLines,
    /// <summary>The distribute center last value.</summary>
    DistributeCenterLast,
    /// <summary>The kashida value.</summary>
    Kashida,
    /// <summary>The newspaper value.</summary>
    Newspaper
}
