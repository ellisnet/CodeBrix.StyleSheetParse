using System;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS steps timing function.</summary>
public sealed class StepsTimingFunction : ITimingFunction
{
    /// <summary>Initializes a new instance of the <see cref="StepsTimingFunction"/> class.</summary>
    public StepsTimingFunction(int intervals, bool start = false)
    {
        Intervals = Math.Max(1, intervals);
        IsStart = start;
    }

    /// <summary>Gets the intervals.</summary>
    public int Intervals { get; }
    /// <summary>Gets the is start.</summary>
    public bool IsStart { get; }
}
