================================================================================
AGENT-README: CodeBrix.StyleSheetParse
A Comprehensive Guide for AI Coding Agents
================================================================================

OVERVIEW
--------
CodeBrix.StyleSheetParse is a fully managed, cross-platform CSS stylesheet
parsing library for .NET 10.0+. It parses CSS text into a strongly-typed
object model that can be queried, manipulated, and serialized back to CSS.

CodeBrix.StyleSheetParse has no dependencies other than .NET.

CodeBrix.StyleSheetParse is a fork of the ExCSS library (v4.3.1). All
namespaces use "CodeBrix.StyleSheetParse" instead of "ExCSS". Do NOT use
ExCSS namespaces.


INSTALLATION
------------
NuGet Package: CodeBrix.StyleSheetParse.MitLicenseForever

    dotnet add package CodeBrix.StyleSheetParse.MitLicenseForever

IMPORTANT: The NuGet package name is CodeBrix.StyleSheetParse.MitLicenseForever
(NOT CodeBrix.StyleSheetParse). The namespace is CodeBrix.StyleSheetParse.

Requirements: .NET 10.0 or higher
License: MIT License


KEY NAMESPACE
-------------
    using CodeBrix.StyleSheetParse;

All public types are in the CodeBrix.StyleSheetParse namespace.


================================================================================

CORE API REFERENCE
==================

PARSING CSS
-----------
The StylesheetParser class is the main entry point. All parsing begins here.

Constructor:
    var parser = new StylesheetParser(
        includeUnknownRules: false,
        includeUnknownDeclarations: false,
        tolerateInvalidSelectors: false,
        tolerateInvalidValues: false,
        tolerateInvalidConstraints: false,
        preserveComments: false,
        preserveDuplicateProperties: false
    );

All parameters are optional and default to false. Pass true to enable
lenient/tolerant parsing behavior.

Parsing methods:
    Stylesheet Parse(string content)
    Stylesheet Parse(Stream content)
    Task<Stylesheet> ParseAsync(string content)
    Task<Stylesheet> ParseAsync(string content, CancellationToken cancelToken)
    Task<Stylesheet> ParseAsync(Stream content)
    Task<Stylesheet> ParseAsync(Stream content, CancellationToken cancelToken)

Selector parsing:
    ISelector ParseSelector(string selectorText)

Example:
    var parser = new StylesheetParser();
    var stylesheet = parser.Parse("h1 { color: red; } p { margin: 10px; }");


STYLESHEET MODEL
-----------------
The Stylesheet class is the root of the parsed CSS document model.

Properties for accessing rules by type:
    stylesheet.CharacterSetRules    // IEnumerable<ICharsetRule>
    stylesheet.FontfaceSetRules     // IEnumerable<IFontFaceRule>
    stylesheet.MediaRules           // IEnumerable<IMediaRule>
    stylesheet.ContainerRules       // IEnumerable<IContainerRule>
    stylesheet.ImportRules          // IEnumerable<IImportRule>
    stylesheet.NamespaceRules       // IEnumerable<INamespaceRule>
    stylesheet.PageRules            // IEnumerable<IPageRule>
    stylesheet.StyleRules           // IEnumerable<IStyleRule>

Modification methods:
    IRule Add(RuleType ruleType)            // Add new rule
    void RemoveAt(int index)                // Remove rule at index
    int Insert(string ruleText, int index)  // Insert parsed rule at index

Serialization:
    void ToCss(TextWriter writer, IStyleFormatter formatter)


RULE TYPES
----------
All rules implement the IRule interface:

    RuleType Type       // The rule type
    string Text         // Get/set rule as CSS text
    IRule Parent        // Parent rule (for nested rules)
    Stylesheet Owner    // Owning stylesheet

RuleType enum values:
    Unknown, Style, Charset, Import, Media, FontFace, Page, Keyframes,
    Keyframe, MarginBox, Namespace, CounterStyle, Supports, Document,
    FontFeatureValues, Viewport, RegionStyle, Container


STYLE RULES (IStyleRule)
-------------------------
Represent standard CSS rules: selector { declarations }

    string SelectorText           // Selector as text
    ISelector Selector            // Parsed selector object
    StyleDeclaration Style        // Style declarations

Example:
    foreach (var rule in stylesheet.StyleRules)
    {
        Console.WriteLine($"Selector: {rule.SelectorText}");
        Console.WriteLine($"Color: {rule.Style.Color}");
        Console.WriteLine($"Font size: {rule.Style.FontSize}");
    }


STYLE DECLARATIONS (StyleDeclaration)
--------------------------------------
Represents CSS property declarations (the content inside { }).

Properties:
    string CssText                      // Get/set all declarations as CSS text
    IEnumerable<Property> Declarations  // All declared properties
    int Length                          // Number of properties
    string this[int index]              // Get property name by index
    string this[string name]            // Get property value by name

Methods:
    void SetProperty(string name, string value, string priority = null)
    void SetPropertyValue(string name, string value)
    void SetPropertyPriority(string name, string priority)
    string GetPropertyValue(string name)
    string GetPropertyPriority(string name)
    string RemoveProperty(string name)
    void Update(string value)

Event:
    event Action<string> Changed

100+ typed CSS properties are available directly:
    rule.Style.Color
    rule.Style.BackgroundColor
    rule.Style.FontSize
    rule.Style.Margin
    rule.Style.Padding
    rule.Style.Display
    rule.Style.Position
    rule.Style.AlignContent
    rule.Style.Animation
    ... and many more


SELECTORS (ISelector)
----------------------
Parsed CSS selectors with specificity calculation.

    string Text                // Selector as text
    Priority Specificity       // CSS specificity (a, b, c, d)

Priority struct:
    byte Inlines    // (a) inline style
    byte Ids        // (b) ID selectors
    byte Classes    // (c) class, attribute, pseudo-class selectors
    byte Tags       // (d) element, pseudo-element selectors

Static values:
    Priority.Zero, Priority.OneTag, Priority.OneClass,
    Priority.OneId, Priority.Inline

Operators: +, ==, !=, <, >, <=, >=

Example:
    var selector = parser.ParseSelector("div.highlight > span#title");
    var specificity = selector.Specificity;
    // Ids: 1, Classes: 1, Tags: 2


MEDIA RULES (IMediaRule)
-------------------------
@media query rules.

    MediaList Media             // Media query conditions
    string ConditionText        // Media condition as text
    IRuleList Rules             // Rules inside media query

Example:
    foreach (var mediaRule in stylesheet.MediaRules)
    {
        Console.WriteLine($"Media: {mediaRule.ConditionText}");
        foreach (IRule rule in mediaRule.Rules)
        {
            // Process rules inside media query
        }
    }


MEDIA LIST (MediaList)
-----------------------
Collection of media query conditions.

    string MediaText            // Get/set all media queries as text
    IEnumerable<Medium> Media   // Individual media conditions
    int Length                  // Number of media conditions
    string this[int index]      // Get media at index as CSS text
    void Add(string newMedium)
    void Remove(string oldMedium)


CONTAINER RULES (IContainerRule)
---------------------------------
@container query rules.

    string Name                 // Container name (optional)
    string ConditionText        // Container condition
    MediaList Media             // Media conditions
    IRuleList Rules             // Rules inside container


KEYFRAMES RULES (IKeyframesRule)
---------------------------------
@keyframes animation rules.

    string Name                 // Animation name
    IRuleList Rules             // Keyframe rules
    void Add(string rule)       // Add keyframe
    void Remove(string key)     // Remove keyframe by key
    IKeyframeRule Find(string key)


KEYFRAME RULE (IKeyframeRule)
------------------------------
Individual keyframe in @keyframes.

    string KeyText              // Key value ("0%", "100%", "from", "to")
    StyleDeclaration Style      // Keyframe styles
    KeyframeSelector Key        // Parsed key selector


FONT FACE RULES (IFontFaceRule)
--------------------------------
@font-face rules.

    string Family               // font-family
    string Source                // src
    string Style                // font-style
    string Weight               // font-weight
    string Stretch              // font-stretch
    string Range                // unicode-range
    string Variant              // font-variant
    string Features             // font-feature-settings


IMPORT RULES (IImportRule)
---------------------------
@import rules.

    string Href                 // URL to import
    MediaList Media             // Media restrictions


PAGE RULES (IPageRule)
-----------------------
@page rules.

    string SelectorText         // Page selector
    StyleDeclaration Style      // Page style declarations


NAMESPACE RULES (INamespaceRule)
---------------------------------
@namespace rules.

    string NamespaceUri         // Namespace URI
    string Prefix               // Namespace prefix


CHARSET RULES (ICharsetRule)
-----------------------------
@charset rules.

    string CharacterSet         // Character encoding


GROUPING RULES (IGroupingRule)
-------------------------------
Base for rules containing other rules (@media, @supports, etc.).

    IRuleList Rules             // Child rules
    IRule AddNewRule(RuleType ruleType)
    int Insert(string rule, int index)
    void RemoveAt(int index)


================================================================================

CSS SERIALIZATION
==================

All stylesheet nodes implement IStyleFormattable:

    void ToCss(TextWriter writer, IStyleFormatter formatter)

Extension methods (FormatExtensions):
    string ToCss(this IStyleFormattable style)
    string ToCss(this IStyleFormattable style, IStyleFormatter formatter)
    void ToCss(this IStyleFormattable style, TextWriter writer)

Default formatter:
    CompressedStyleFormatter.Instance

Example - serialize a stylesheet back to CSS:
    var parser = new StylesheetParser();
    var stylesheet = parser.Parse(cssText);

    // Modify some properties
    var firstRule = stylesheet.StyleRules.First();
    firstRule.Style.SetProperty("color", "blue");

    // Serialize back to CSS
    string output = stylesheet.ToCss();

Example - write to a file:
    using var writer = new StreamWriter("output.css");
    stylesheet.ToCss(writer, CompressedStyleFormatter.Instance);

IStyleFormatter interface methods:
    string Sheet(IEnumerable<IStyleFormattable> rules)
    string Block(IEnumerable<IStyleFormattable> rules)
    string Declaration(string name, string value, bool important)
    string Declarations(IEnumerable<string> declarations)
    string Medium(bool exclusive, bool inverse, string type,
                  IEnumerable<IStyleFormattable> constraints)
    string Constraint(string name, string value, string constraintDelimiter)
    string Rule(string name, string value)
    string Rule(string name, string prelude, string rules)
    string Style(string selector, IStyleFormattable rules)
    string Comment(string data)


================================================================================

CONSTANTS AND REFERENCE CLASSES
================================

PropertyNames - constants for all CSS property names:
    PropertyNames.Color, PropertyNames.FontSize, PropertyNames.Margin,
    PropertyNames.Padding, PropertyNames.Display, PropertyNames.Position,
    PropertyNames.AlignContent, PropertyNames.Animation,
    PropertyNames.BackgroundColor, PropertyNames.BorderWidth, ...

RuleNames - constants for @-rule names:
    RuleNames.Media, RuleNames.Import, RuleNames.FontFace,
    RuleNames.Keyframes, RuleNames.Supports, RuleNames.Container,
    RuleNames.Page, RuleNames.Namespace, RuleNames.Charset, ...

Keywords - constants for CSS keywords:
    Keywords.Important, Keywords.Inherit, Keywords.Initial,
    Keywords.Unset, Keywords.Auto, Keywords.None, ...

FunctionNames - constants for CSS function names:
    FunctionNames.Url, FunctionNames.Calc, FunctionNames.Rgb,
    FunctionNames.Rgba, FunctionNames.Hsl, FunctionNames.Hsla, ...

PseudoClassNames - constants for pseudo-class names:
    PseudoClassNames.Active, PseudoClassNames.Focus,
    PseudoClassNames.Hover, PseudoClassNames.FirstChild,
    PseudoClassNames.LastChild, PseudoClassNames.NthChild, ...

PseudoElementNames - constants for pseudo-element names:
    PseudoElementNames.Before, PseudoElementNames.After,
    PseudoElementNames.FirstLine, PseudoElementNames.FirstLetter, ...

FeatureNames - constants for media feature names:
    FeatureNames.MinWidth, FeatureNames.MaxWidth,
    FeatureNames.Orientation, FeatureNames.Resolution, ...

Colors - named color constants and conversions


================================================================================

COMPLETE EXAMPLES
==================

Example 1: Parse and Inspect a Stylesheet
------------------------------------------
    using CodeBrix.StyleSheetParse;

    var parser = new StylesheetParser();
    var stylesheet = parser.Parse(@"
        h1 { color: red; font-size: 24px; }
        .highlight { background-color: yellow; }
        @media screen and (max-width: 768px) {
            h1 { font-size: 18px; }
        }
    ");

    // Access style rules
    foreach (var rule in stylesheet.StyleRules)
    {
        Console.WriteLine($"Selector: {rule.SelectorText}");
        foreach (var property in rule.Style.Declarations)
        {
            Console.WriteLine($"  {property.Name}: {property.Value}");
        }
    }

    // Access media rules
    foreach (var media in stylesheet.MediaRules)
    {
        Console.WriteLine($"Media: {media.ConditionText}");
    }


Example 2: Modify Properties and Serialize
--------------------------------------------
    using CodeBrix.StyleSheetParse;

    var parser = new StylesheetParser();
    var stylesheet = parser.Parse("h1 { color: red; } p { margin: 10px; }");

    // Modify a property
    var firstRule = stylesheet.StyleRules.First();
    firstRule.Style.SetProperty("color", "blue");
    firstRule.Style.SetProperty("font-weight", "bold");

    // Add a new property with !important
    firstRule.Style.SetProperty("text-align", "center", "important");

    // Serialize back to CSS
    string css = stylesheet.ToCss();
    Console.WriteLine(css);


Example 3: Parse Selectors and Calculate Specificity
------------------------------------------------------
    using CodeBrix.StyleSheetParse;

    var parser = new StylesheetParser();

    var selectors = new[]
    {
        "h1",
        ".class",
        "#id",
        "div.class > span#id",
        "ul li a:hover"
    };

    foreach (var selectorText in selectors)
    {
        var selector = parser.ParseSelector(selectorText);
        var s = selector.Specificity;
        Console.WriteLine($"{selectorText} => ({s.Inlines},{s.Ids},{s.Classes},{s.Tags})");
    }


Example 4: Async Parsing from a Stream
----------------------------------------
    using CodeBrix.StyleSheetParse;

    await using var stream = File.OpenRead("styles.css");
    var parser = new StylesheetParser();

    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
    var stylesheet = await parser.ParseAsync(stream, cts.Token);

    Console.WriteLine($"Parsed {stylesheet.StyleRules.Count()} style rules");
    Console.WriteLine($"Parsed {stylesheet.MediaRules.Count()} media rules");
    Console.WriteLine($"Parsed {stylesheet.FontfaceSetRules.Count()} font-face rules");


Example 5: Tolerant Parsing with Comments
-------------------------------------------
    using CodeBrix.StyleSheetParse;

    var parser = new StylesheetParser(
        includeUnknownRules: true,
        includeUnknownDeclarations: true,
        tolerateInvalidSelectors: true,
        preserveComments: true
    );

    var stylesheet = parser.Parse(@"
        /* Main styles */
        h1 { color: red; }
        @unknown-rule { content: test; }
    ");

    string css = stylesheet.ToCss();


Example 6: Working with @keyframes
------------------------------------
    using CodeBrix.StyleSheetParse;
    using System.Linq;

    var parser = new StylesheetParser();
    var stylesheet = parser.Parse(@"
        @keyframes fadeIn {
            from { opacity: 0; }
            to { opacity: 1; }
        }
        .animated { animation: fadeIn 1s ease-in; }
    ");

    // Access keyframes rules via the stylesheet's children
    foreach (IRule rule in stylesheet.Children.OfType<IRule>())
    {
        if (rule is IKeyframesRule keyframes)
        {
            Console.WriteLine($"Animation: {keyframes.Name}");
            foreach (IRule kfRule in keyframes.Rules)
            {
                if (kfRule is IKeyframeRule keyframe)
                {
                    Console.WriteLine($"  {keyframe.KeyText}: {keyframe.Style.CssText}");
                }
            }
        }
    }


Example 7: Working with @font-face
------------------------------------
    using CodeBrix.StyleSheetParse;

    var parser = new StylesheetParser();
    var stylesheet = parser.Parse(@"
        @font-face {
            font-family: 'MyFont';
            src: url('myfont.woff2') format('woff2');
            font-weight: normal;
            font-style: normal;
        }
    ");

    foreach (var fontFace in stylesheet.FontfaceSetRules)
    {
        Console.WriteLine($"Font family: {fontFace.Family}");
        Console.WriteLine($"Source: {fontFace.Source}");
        Console.WriteLine($"Weight: {fontFace.Weight}");
    }


Example 8: Working with @container Queries
--------------------------------------------
    using CodeBrix.StyleSheetParse;

    var parser = new StylesheetParser();
    var stylesheet = parser.Parse(@"
        @container sidebar (min-width: 700px) {
            .card { font-size: 2em; }
        }
    ");

    foreach (var container in stylesheet.ContainerRules)
    {
        Console.WriteLine($"Container: {container.Name}");
        Console.WriteLine($"Condition: {container.ConditionText}");
    }


================================================================================

PERFORMANCE TIPS
=================

1. Reuse StylesheetParser instances. The parser is stateless between parse
   calls and safe to reuse.

2. Use ParseAsync with CancellationToken for large stylesheets to allow
   timeout/cancellation.

3. Use Stream-based parsing for large CSS files instead of loading the
   entire string into memory first.

4. Only enable tolerance options you need. Each tolerance option adds
   processing overhead.

5. Only enable preserveComments if you need comment data. Comments add
   nodes to the tree.

6. Use ToCss with a TextWriter for large output instead of building
   strings in memory.

7. Access typed rule collections (StyleRules, MediaRules, etc.) instead of
   iterating all children and type-checking.

8. Use the string indexer on StyleDeclaration (rule.Style["color"]) for
   quick property value lookups by name.


================================================================================

COMMON PITFALLS TO AVOID
==========================

1. DO NOT confuse the NuGet package name with the namespace.
   - Package: CodeBrix.StyleSheetParse.MitLicenseForever
     Namespace: CodeBrix.StyleSheetParse

2. DO NOT use ExCSS namespaces. Even though this is a fork of ExCSS, all
   namespaces are CodeBrix.StyleSheetParse.

3. DO NOT target .NET versions below 10.0.

4. DO NOT forget that Parse() returns a Stylesheet, not a list of rules.
   Access rules through the Stylesheet's typed properties (StyleRules,
   MediaRules, etc.).

5. DO NOT assume invalid CSS will throw exceptions. By default, invalid
   rules/selectors/values are silently dropped. Use the tolerance
   parameters to control this behavior.

6. DO NOT confuse Property.Value with Property.Original. Value is the
   parsed/normalized value; Original is the raw text from the source.

7. DO NOT forget that specificity comparison uses operator overloads.
   Compare Priority values directly with <, >, ==, etc.

8. DO NOT forget to check IsImportant on properties when reading
   declarations. The !important flag is separate from the value.

9. DO NOT use SetPropertyPriority with the value "!important" (with the
   exclamation mark). Use "important" without the exclamation mark, or
   pass the priority through SetProperty's third parameter.


================================================================================

WHAT THIS LIBRARY DOES NOT DO
===============================

Do NOT attempt to use this library for the following:

  - Rendering CSS or applying styles to HTML elements (this is a parser
    and object model only - it does not render anything)
  - Validating CSS against a specific CSS specification version
  - Minifying or prettifying CSS (it serializes using CompressedStyleFormatter;
    custom formatters can be implemented via IStyleFormatter)
  - Resolving @import rules (the import URL is parsed but not fetched)
  - Evaluating media queries against a device context
  - Sass/SCSS/Less preprocessing
  - CSS module scoping or transformation
  - PostCSS-style plugin processing

CodeBrix.StyleSheetParse IS for: parsing CSS text into a structured object
model, querying and manipulating that model, calculating selector specificity,
and serializing the model back to CSS text.


================================================================================

QUICK REFERENCE CARD
=====================

--- Install ---
dotnet add package CodeBrix.StyleSheetParse.MitLicenseForever

--- Namespace ---
using CodeBrix.StyleSheetParse;

--- Parse ---
Parse CSS:          var sheet = new StylesheetParser().Parse(cssText);
Parse stream:       var sheet = parser.Parse(stream);
Parse async:        var sheet = await parser.ParseAsync(cssText);
Parse async cancel: var sheet = await parser.ParseAsync(cssText, token);
Parse selector:     var sel = parser.ParseSelector("div > p.class");

--- Access Rules ---
Style rules:        sheet.StyleRules
Media rules:        sheet.MediaRules
Container rules:    sheet.ContainerRules
Font-face rules:    sheet.FontfaceSetRules
Import rules:       sheet.ImportRules
Namespace rules:    sheet.NamespaceRules
Page rules:         sheet.PageRules
Charset rules:      sheet.CharacterSetRules

--- Style Rule ---
Selector text:      rule.SelectorText
Parsed selector:    rule.Selector
Specificity:        rule.Selector.Specificity
Declarations:       rule.Style

--- Style Declarations ---
Get value:          rule.Style["color"] or rule.Style.GetPropertyValue("color")
Get priority:       rule.Style.GetPropertyPriority("color")
Set property:       rule.Style.SetProperty("color", "red")
Set with priority:  rule.Style.SetProperty("color", "red", "important")
Remove property:    rule.Style.RemoveProperty("color")
All declarations:   rule.Style.Declarations
Count:              rule.Style.Length
Typed access:       rule.Style.Color, rule.Style.FontSize, ...

--- Media Rules ---
Condition text:     mediaRule.ConditionText
Nested rules:       mediaRule.Rules
Media list:         mediaRule.Media

--- Serialize ---
To string:          sheet.ToCss()
With formatter:     sheet.ToCss(CompressedStyleFormatter.Instance)
To writer:          sheet.ToCss(writer)

--- Parser Options (all default false) ---
includeUnknownRules, includeUnknownDeclarations, tolerateInvalidSelectors,
tolerateInvalidValues, tolerateInvalidConstraints, preserveComments,
preserveDuplicateProperties

Target: .NET 10.0+
License: MIT


================================================================================

DEEPER LEARNING: TEST FILE CROSS-REFERENCES
=============================================

The CodeBrix.StyleSheetParse.Tests project contains working examples:

    https://github.com/ellisnet/CodeBrix.StyleSheetParse
    Path: tests/CodeBrix.StyleSheetParse.Tests/

Feature-to-test-file mapping:

  CSS selector parsing (attribute, class, general selectors):
    -> tests/CodeBrix.StyleSheetParse.Tests/AttrSelectorTests.cs
    -> tests/CodeBrix.StyleSheetParse.Tests/ClassSelectorTests.cs
    -> tests/CodeBrix.StyleSheetParse.Tests/SelectorsTests.cs

  CSS color values:
    -> tests/CodeBrix.StyleSheetParse.Tests/Color.cs

  CSS construction functions (calc, var, etc.):
    -> tests/CodeBrix.StyleSheetParse.Tests/ConstructionFunctions.cs

  @container queries:
    -> tests/CodeBrix.StyleSheetParse.Tests/Container.cs

  @font-face rules:
    -> tests/CodeBrix.StyleSheetParse.Tests/FontFace.cs

  Font properties:
    -> tests/CodeBrix.StyleSheetParse.Tests/FontProperty.cs

  CSS gradient functions:
    -> tests/CodeBrix.StyleSheetParse.Tests/Gradient.cs

  @import rules:
    -> tests/CodeBrix.StyleSheetParse.Tests/ImportRule.cs

  @keyframes rules:
    -> tests/CodeBrix.StyleSheetParse.Tests/KeyframeRule.cs

  List properties (list-style, etc.):
    -> tests/CodeBrix.StyleSheetParse.Tests/ListProperty.cs

  Media features and media lists:
    -> tests/CodeBrix.StyleSheetParse.Tests/MediaFeatures.cs
    -> tests/CodeBrix.StyleSheetParse.Tests/MediaList.cs

  Object sizing properties:
    -> tests/CodeBrix.StyleSheetParse.Tests/ObjectSizing.cs

  CSS properties (general):
    -> tests/CodeBrix.StyleSheetParse.Tests/Property.cs

  Flexbox properties:
    -> tests/CodeBrix.StyleSheetParse.Tests/Flexbox.cs

  Real-world CSS parsing (e.g., bootstrap.css):
    -> tests/CodeBrix.StyleSheetParse.Tests/RealWorld.cs

  Stylesheet parsing and manipulation:
    -> tests/CodeBrix.StyleSheetParse.Tests/Sheet.cs

  @supports rules:
    -> tests/CodeBrix.StyleSheetParse.Tests/Supports.cs

  CSS tokenization:
    -> tests/CodeBrix.StyleSheetParse.Tests/Tokenization.cs

  @document function:
    -> tests/CodeBrix.StyleSheetParse.Tests/DocumentFunction.cs

HOW TO USE: Fetch the raw file content from GitHub using a URL like:
    https://raw.githubusercontent.com/ellisnet/CodeBrix.StyleSheetParse/main/{path}
For example:
    https://raw.githubusercontent.com/ellisnet/CodeBrix.StyleSheetParse/main/tests/CodeBrix.StyleSheetParse.Tests/Sheet.cs


================================================================================

END OF AGENT-README
