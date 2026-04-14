using System.Collections.Generic;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS properties node.</summary>
public interface IProperties : IEnumerable<IProperty>
{
    /// <summary>The this member.</summary>
    string this[string propertyName] { get; }
    /// <summary>The length member.</summary>
    int Length { get; }
    /// <summary>The get property value member.</summary>
    string GetPropertyValue(string propertyName);
    /// <summary>The get property priority member.</summary>
    string GetPropertyPriority(string propertyName);
    /// <summary>The set property member.</summary>
    void SetProperty(string propertyName, string propertyValue, string priority = null);
    /// <summary>The remove property member.</summary>
    string RemoveProperty(string propertyName);
}
