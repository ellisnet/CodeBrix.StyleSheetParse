using System.Collections.Generic;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS stylesheetnode node.</summary>
public interface IStylesheetNode : IStyleFormattable
{
    /// <summary>The children member.</summary>
    IEnumerable<IStylesheetNode> Children { get; }
    /// <summary>The stylesheet text member.</summary>
    StylesheetText StylesheetText { get; }
}
