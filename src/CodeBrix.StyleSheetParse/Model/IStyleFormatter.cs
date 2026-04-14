using System.Collections.Generic;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS styleformatter node.</summary>
public interface IStyleFormatter
{
    /// <summary>The sheet member.</summary>
    string Sheet(IEnumerable<IStyleFormattable> rules);
    /// <summary>The block member.</summary>
    string Block(IEnumerable<IStyleFormattable> rules);
    /// <summary>The declaration member.</summary>
    string Declaration(string name, string value, bool important);
    /// <summary>The declarations member.</summary>
    string Declarations(IEnumerable<string> declarations);
    /// <summary>The medium member.</summary>
    string Medium(bool exclusive, bool inverse, string type, IEnumerable<IStyleFormattable> constraints);
    /// <summary>The constraint member.</summary>
    string Constraint(string name, string value, string constraintDelimiter);
    /// <summary>The rule member.</summary>
    string Rule(string name, string value);
    /// <summary>The rule member.</summary>
    string Rule(string name, string prelude, string rules);
    /// <summary>The style member.</summary>
    string Style(string selector, IStyleFormattable rules);
    /// <summary>The comment member.</summary>
    string Comment(string data);
}
