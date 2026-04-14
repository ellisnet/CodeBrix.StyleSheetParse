using System.Collections.Generic;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS gradient node.</summary>
public interface IGradient : IImageSource
{
    /// <summary>The stops member.</summary>
    IEnumerable<GradientStop> Stops { get; }
    /// <summary>The is repeating member.</summary>
    bool IsRepeating { get; }
}
