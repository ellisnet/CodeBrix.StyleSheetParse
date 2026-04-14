# CodeBrix.StyleSheetParse

A fully managed, cross-platform CSS stylesheet parsing library for .NET.
CodeBrix.StyleSheetParse has no dependencies other than .NET, and is provided as a .NET 10 library and associated `CodeBrix.StyleSheetParse.MitLicenseForever` NuGet package.

CodeBrix.StyleSheetParse supports applications and assemblies that target Microsoft .NET version 10.0 and later.
Microsoft .NET version 10.0 is a Long-Term Supported (LTS) version of .NET, and was released on Nov 11, 2025; and will be actively supported by Microsoft until Nov 14, 2028.
Please update your C#/.NET code and projects to the latest LTS version of Microsoft .NET.

CodeBrix.StyleSheetParse is a fork of the code of the open source ExCSS library - see below for licensing details.

## CodeBrix.StyleSheetParse supports:

* CSS stylesheet parsing from strings and streams
* Async parsing with cancellation support
* CSS selector parsing and specificity calculation
* Style rule, media query, and at-rule modeling
* @keyframes, @font-face, @supports, @container, @page, @import rules
* Style declaration reading and manipulation
* CSS serialization (converting parsed stylesheets back to CSS text)
* Configurable parser tolerance (unknown rules, invalid selectors, comments, etc.)
* Many more...

## Sample Code

### Parse a CSS Stylesheet

```csharp
using CodeBrix.StyleSheetParse;

var parser = new StylesheetParser();
var stylesheet = parser.Parse("h1 { color: red; font-size: 24px; }");

foreach (var rule in stylesheet.StyleRules)
{
    Console.WriteLine($"Selector: {rule.SelectorText}");
    Console.WriteLine($"Color: {rule.Style.Color}");
}
```

### Parse a CSS Selector and Check Specificity

```csharp
using CodeBrix.StyleSheetParse;

var parser = new StylesheetParser();
var selector = parser.ParseSelector("div.highlight > span#title");

Console.WriteLine($"Selector: {selector.Text}");
Console.WriteLine($"Specificity: {selector.Specificity}");
```

### Serialize a Stylesheet Back to CSS

```csharp
using CodeBrix.StyleSheetParse;

var parser = new StylesheetParser();
var stylesheet = parser.Parse("h1 { color: red; } @media screen { p { font-size: 14px; } }");

string css = stylesheet.ToCss();
Console.WriteLine(css);
```

Note that additional sample code and usage examples are available in the `CodeBrix.StyleSheetParse.Tests` project.

## License

The project is licensed under the MIT License. see: https://en.wikipedia.org/wiki/MIT_License

All code originating from ExCSS was included as allowed by the MIT License permissible open source software license - as of ExCSS version 4.3.1.
This project (CodeBrix.StyleSheetParse) complies with all provisions of the source code license of ExCSS v4.3.1 (MIT License).
