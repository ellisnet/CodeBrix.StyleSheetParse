namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS mediarule node.</summary>
public interface IMediaRule : IConditionRule
{
    /// <summary>The media member.</summary>
    MediaList Media { get; }
}
