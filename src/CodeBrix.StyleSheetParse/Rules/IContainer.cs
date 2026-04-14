namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS containerrule node.</summary>
public interface IContainerRule : IConditionRule
{
    /// <summary>The name member.</summary>
    string Name { get; set; }
    /// <summary>The media member.</summary>
    MediaList Media { get; }
  }
