namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS attrselector node.</summary>
public interface IAttrSelector : ISelector
{
    /// <summary>The attribute member.</summary>
    string Attribute { get;  }
    /// <summary>The value member.</summary>
    string Value { get; }
}
