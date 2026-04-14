using System.Collections.Generic;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS rulelist node.</summary>
public interface IRuleList : IEnumerable<IRule>
{
    /// <summary>The this member.</summary>
    IRule this[int index] { get; }
    /// <summary>The length member.</summary>
    int Length { get; }
}
