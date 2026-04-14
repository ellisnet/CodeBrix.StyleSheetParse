namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS importrule node.</summary>
public interface IImportRule : IRule
{
    /// <summary>The href member.</summary>
    string Href { get; set; }
    /// <summary>The media member.</summary>
    MediaList Media { get; }
}
