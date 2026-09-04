# CodeBrix.StyleSheetParse

A fully managed, cross-platform CSS stylesheet parsing library for .NET.
CodeBrix.StyleSheetParse has no dependencies other than .NET, and is provided as a .NET 10 library and associated `CodeBrix.StyleSheetParse.MitLicenseForever` NuGet package.

CodeBrix.StyleSheetParse supports applications and assemblies that target Microsoft .NET version 10.0 and later.
Microsoft .NET version 10.0 is a Long-Term Supported (LTS) version of .NET, and was released on Nov 11, 2025; and will be actively supported by Microsoft until Nov 14, 2028.
Please update your C#/.NET code and projects to the latest LTS version of Microsoft .NET.

## Installation

```
dotnet add package CodeBrix.StyleSheetParse.MitLicenseForever
```

Note that the NuGet package ID and the namespace are different - there is no package named plain `CodeBrix.StyleSheetParse`:

* NuGet package ID: `CodeBrix.StyleSheetParse.MitLicenseForever`
* Assembly and namespace: `CodeBrix.StyleSheetParse` - i.e. `using CodeBrix.StyleSheetParse;`

Everything public lives in that one namespace. The package has no NuGet dependencies and no native libraries; it depends only on the .NET base class library. XML documentation (IntelliSense) ships alongside the assembly.

## CodeBrix.StyleSheetParse supports:

* CSS stylesheet parsing from strings and streams
* Async parsing with cancellation support
* CSS selector parsing and specificity calculation
* Style rule, media query, and at-rule modeling
* @keyframes, @font-face, @supports, @container, @page, @import, @namespace, @charset, @document, @viewport rules
* Style declaration reading and manipulation
* CSS serialization (converting parsed stylesheets back to CSS text)
* Configurable parser tolerance (unknown rules, invalid selectors, comments, etc.)
* Many more...

CodeBrix.StyleSheetParse is a parser and object model, not a rendering or styling engine. It does not match selectors against a document, compute cascaded or inherited styles, validate CSS against a specification, resolve `@import` URLs or `var()` custom properties, or minify CSS. The one built-in formatter emits readable output; implement `IStyleFormatter` for anything else.

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

## Documentation

The NuGet package includes `AGENT-README.txt`, a complete API reference and usage guide written for AI coding agents - point your agent at that file when it is writing code against this library.

Additional sample code and usage examples are available in the `CodeBrix.StyleSheetParse.Tests` project:
https://github.com/ellisnet/CodeBrix.StyleSheetParse/tree/main/tests/CodeBrix.StyleSheetParse.Tests

Note that the test project has `InternalsVisibleTo` access to the library, so some of what it calls (for example `Stylesheet.Rules`, `MediaRule`, `SupportsRule`, `KeyframeRule` and `parser.ParseDeclaration`) is internal and is not available to package consumers.

## License

CodeBrix.StyleSheetParse is licensed under the MIT License - see the
[LICENSE](https://github.com/ellisnet/CodeBrix.StyleSheetParse/blob/main/LICENSE) file.

For licensing and provenance information about the open source code included in
this package, see [THIRD-PARTY-NOTICES.txt](https://github.com/ellisnet/CodeBrix.StyleSheetParse/blob/main/THIRD-PARTY-NOTICES.txt).
