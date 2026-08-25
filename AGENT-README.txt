================================================================================
AGENT-README: CodeBrix.StyleSheetParse
A Guide for AI Coding Agents — CONSUMING the
CodeBrix.StyleSheetParse.MitLicenseForever NuGet package
================================================================================

OVERVIEW
========
CodeBrix.StyleSheetParse is a fully managed, cross-platform CSS stylesheet
parsing library. It parses CSS text into a strongly-typed object model that
can be queried, manipulated, and serialized back to CSS.

Target framework: .NET 10 or later. No dependencies other than .NET itself.
No native libraries, no OS restrictions - it runs anywhere .NET 10 runs.

Provenance: this library is a fork of the open-source ExCSS library (MIT), as
of ExCSS version 4.3.1. Every namespace was renamed from "ExCSS" to
"CodeBrix.StyleSheetParse". Do NOT use ExCSS namespaces, and do NOT assume an
API exists here merely because ExCSS documentation or your own memory of ExCSS
mentions it - several types that ExCSS exposes are internal in this fork
(they are called out explicitly below).

What it is good at:
  -> Parsing CSS from a string or a Stream, synchronously or asynchronously
  -> Walking the rule tree (@media, @supports, @container, @keyframes,
     @font-face, @import, @namespace, @charset, @page, @document, @viewport)
  -> Reading and writing style declarations by property name
  -> Parsing selectors into a selector object model with CSS specificity
  -> Serializing the model back to CSS text through a pluggable formatter

What it is NOT: a rendering engine, a style resolver, or a CSS validator.
See "WHAT THIS PACKAGE DOES NOT DO" near the end.


INSTALLATION
============
PackageId: CodeBrix.StyleSheetParse.MitLicenseForever

    dotnet add package CodeBrix.StyleSheetParse.MitLicenseForever

IMPORTANT: the package id is CodeBrix.StyleSheetParse.MitLicenseForever; the
namespace and the assembly name are CodeBrix.StyleSheetParse. There is no
package named "CodeBrix.StyleSheetParse".

NuGet dependencies: none.
License: MIT.
Requirements: a project targeting .NET 10 or later. Nothing else.


KEY NAMESPACES / USINGS
=======================
    using CodeBrix.StyleSheetParse;         // everything public lives here

Frequently needed companions in consumer code:

    using System.Linq;                      // OfType<T>(), First(), Where()
    using System.IO;                        // Stream, TextWriter, StreamWriter
    using System.Threading;                 // CancellationToken
    using System.Threading.Tasks;           // Task, await

There is exactly one public namespace. (An internal namespace
CodeBrix.StyleSheetParse.Conditions exists in the assembly, but every type in
it is internal; never write a using for it.)


================================================================================

CORE API REFERENCE
==================

PARSING CSS
-----------
StylesheetParser is the entry point. All parsing begins here.

Constructor (every parameter is optional and defaults to false):

    var parser = new StylesheetParser(
        bool includeUnknownRules        = false,
        bool includeUnknownDeclarations = false,
        bool tolerateInvalidSelectors   = false,
        bool tolerateInvalidValues      = false,
        bool tolerateInvalidConstraints = false,
        bool preserveComments           = false,
        bool preserveDuplicateProperties = false);

What each option actually does:

    includeUnknownRules         Keep at-rules the library has no model for,
                                as a rule with Type == RuleType.Unknown,
                                instead of skipping them.
    includeUnknownDeclarations  Keep declarations whose property name is not
                                recognised. This also turns OFF "strict mode"
                                on every StyleDeclaration created by this
                                parser (see StyleDeclaration.IsStrictMode),
                                which changes shorthand read-back behaviour.
    tolerateInvalidSelectors    Accept non-standard pseudo-elements and make
                                ParseSelector return an UnknownSelector
                                instead of null for unparsable input.
    tolerateInvalidValues       Two effects. A declaration whose value the
                                library cannot convert is kept with its raw
                                text instead of being dropped; and a style
                                rule whose selector failed to parse is kept
                                (with an UnknownSelector) instead of the whole
                                rule being dropped from the stylesheet.
    tolerateInvalidConstraints  Accept media-feature constraints the library
                                cannot validate.
    preserveComments            Retain comment nodes in the tree. NOTE: the
                                comment node type is internal, so consumer
                                code cannot read comment text (see pitfalls).
    preserveDuplicateProperties Keep every occurrence of a repeated property
                                in a declaration block instead of keeping only
                                the winning one.

Parsing methods:

    Stylesheet        Parse(string content)
    Stylesheet        Parse(Stream content)
    Task<Stylesheet>  ParseAsync(string content)
    Task<Stylesheet>  ParseAsync(string content, CancellationToken cancelToken)
    Task<Stylesheet>  ParseAsync(Stream content)
    Task<Stylesheet>  ParseAsync(Stream content, CancellationToken cancelToken)

Selector parsing (standalone, no stylesheet needed):

    ISelector ParseSelector(string selectorText)

Returns null when the text is not a valid selector and the parser was created
without tolerateInvalidSelectors: true. With that option it returns an
UnknownSelector instead. ALWAYS null-check the result.

Example:

    var parser = new StylesheetParser();
    var stylesheet = parser.Parse("h1 { color: red; } p { margin: 10px; }");

A StylesheetParser holds no per-parse state; one instance may be reused for
many parses and shared across threads for read-only parsing work.


THE STYLESHEET OBJECT MODEL
---------------------------
Every node in the tree implements IStylesheetNode, and most concrete nodes
derive from the abstract StylesheetNode class:

    public interface IStyleFormattable
    {
        void ToCss(TextWriter writer, IStyleFormatter formatter);
    }

    public interface IStylesheetNode : IStyleFormattable
    {
        IEnumerable<IStylesheetNode> Children { get; }
        StylesheetText StylesheetText { get; }   // source text + range
    }

    public abstract class StylesheetNode : IStylesheetNode
    {
        public StylesheetText StylesheetText { get; }   // internal setter
        public IEnumerable<IStylesheetNode> Children { get; }
        public abstract void ToCss(TextWriter writer, IStyleFormatter formatter);
        public void AppendChild(IStylesheetNode child);
        public void ReplaceChild(IStylesheetNode oldChild, IStylesheetNode newChild);
        public void InsertBefore(IStylesheetNode referenceChild, IStylesheetNode child);
        public void InsertChild(int index, IStylesheetNode child);
        public void RemoveChild(IStylesheetNode child);
        public void Clear();
    }

Children is THE general-purpose way to walk the tree. It is how you reach
every rule kind that Stylesheet does not expose as a typed collection.

Stylesheet - the root of a parsed CSS document:

    public sealed class Stylesheet : StylesheetNode
    {
        public IEnumerable<ICharsetRule>   CharacterSetRules { get; }
        public IEnumerable<IFontFaceRule>  FontfaceSetRules  { get; }
        public IEnumerable<IMediaRule>     MediaRules        { get; }
        public IEnumerable<IContainerRule> ContainerRules    { get; }
        public IEnumerable<IImportRule>    ImportRules       { get; }
        public IEnumerable<INamespaceRule> NamespaceRules    { get; }
        public IEnumerable<IPageRule>      PageRules         { get; }
        public IEnumerable<IStyleRule>     StyleRules        { get; }

        public IRule Add(RuleType ruleType);          // append a new empty rule
        public void  RemoveAt(int index);             // remove rule at index
        public int   Insert(string ruleText, int index);  // parse + insert
        public override void ToCss(TextWriter writer, IStyleFormatter formatter);
    }

There is no typed collection for @keyframes, @supports, @document or
@viewport, and no public "all rules in order" collection: the Stylesheet.Rules
member is internal. Use Children instead:

    foreach (IRule rule in stylesheet.Children.OfType<IRule>())
    {
        // document order, every rule kind
    }

The Stylesheet constructor is internal. To start from an empty sheet, parse an
empty string: var sheet = parser.Parse(string.Empty);


RULE TYPES AND HOW TO REACH THEM
--------------------------------
Every rule implements IRule:

    public interface IRule : IStylesheetNode
    {
        RuleType  Type   { get; }        // discriminator
        string    Text   { get; set; }   // whole rule as CSS text; the setter
                                         // re-parses and can throw ParseException
        IRule     Parent { get; }        // enclosing rule, or null
        Stylesheet Owner { get; }        // owning stylesheet
    }

RuleType (public enum, byte):

    Unknown, Style, Charset, Import, Media, FontFace, Page, Keyframes,
    Keyframe, MarginBox, Namespace, CounterStyle, Supports, Document,
    FontFeatureValues, Viewport, RegionStyle, Container

Rule kind -> public contract -> how to get there:

    style rule      IStyleRule       stylesheet.StyleRules
    @charset        ICharsetRule     stylesheet.CharacterSetRules
    @import         IImportRule      stylesheet.ImportRules
    @namespace      INamespaceRule   stylesheet.NamespaceRules
    @media          IMediaRule       stylesheet.MediaRules
    @container      IContainerRule   stylesheet.ContainerRules
    @font-face      IFontFaceRule    stylesheet.FontfaceSetRules
    @page           IPageRule        stylesheet.PageRules
    @keyframes      IKeyframesRule   Children.OfType<IKeyframesRule>()
    keyframe        IKeyframeRule    keyframesRule.Rules
    @supports       ISupportsRule    Children.OfType<ISupportsRule>()
    @document       IRule + IDocumentFunction children (no dedicated interface)
    @viewport       IProperties      cast an IRule with Type == Viewport
    margin box      MarginStyleRule  pageRule.Children.OfType<MarginStyleRule>()

Only four rule classes are public: Rule (abstract base), StyleRule,
MarginStyleRule and CharsetRule. All other concrete rule classes (MediaRule,
ImportRule, KeyframesRule, SupportsRule, DocumentRule, PageRule, FontFaceRule,
NamespaceRule, ContainerRule, ViewportRule, UnknownRule) are internal - you
cannot name them in a cast or a pattern match. Cast to the interface instead.

Grouping and condition contracts:

    public interface IRuleCreator
    {
        IRule AddNewRule(RuleType ruleType);
    }

    public interface IRuleList : IEnumerable<IRule>
    {
        IRule this[int index] { get; }
        int Length { get; }
    }

    public interface IGroupingRule : IRule, IRuleCreator
    {
        IRuleList Rules { get; }
        int  Insert(string rule, int index);
        void RemoveAt(int index);
    }

    public interface IConditionRule : IGroupingRule
    {
        string ConditionText { get; set; }
    }

IMediaRule, IContainerRule and ISupportsRule all derive from IConditionRule,
so they all carry Rules, ConditionText, Insert, RemoveAt and AddNewRule.

    public abstract class Rule : StylesheetNode, IRule
    {
        public Stylesheet Owner { get; }
        public RuleType   Type  { get; }
        public string     Text  { get; set; }
        public IRule      Parent { get; }
        protected virtual void ReplaceWith(IRule rule);
        protected void ReplaceSingle(IStylesheetNode oldNode, IStylesheetNode newNode);
    }


STYLE RULES
-----------
    public interface IStyleRule : IRule
    {
        string           SelectorText { get; set; }  // setter re-parses
        ISelector        Selector     { get; set; }
        StyleDeclaration Style        { get; }
    }

    public sealed class StyleRule : Rule, IStyleRule
    {
        public StyleRule(StylesheetParser parser);   // publicly constructible
    }

    public sealed class MarginStyleRule : Rule, IStyleRule
    {
        public MarginStyleRule(StylesheetParser parser);
    }

MarginStyleRule models a margin box inside @page (for example @top-center).
Note two quirks: it implements IStyleRule (not IMarginRule), its Type is
RuleType.Style, and its SelectorText getter prefixes the selector text with
"@". IMarginRule is declared in the public API but nothing in this library
implements it; treat it as a contract you may implement yourself, not as
something you will receive from the parser.

    public interface IMarginRule : IRule
    {
        string           Name  { get; }
        StyleDeclaration Style { get; }
    }

Example:

    foreach (var rule in stylesheet.StyleRules)
    {
        Console.WriteLine($"Selector: {rule.SelectorText}");
        Console.WriteLine($"Color: {rule.Style.Color}");
        Console.WriteLine($"Font size: {rule.Style.FontSize}");
    }


STYLE DECLARATIONS
------------------
StyleDeclaration is the content between { and }.

    public sealed class StyleDeclaration : StylesheetNode, IProperties
    {
        public event Action<string> Changed;   // fires with the new CssText

        public string CssText { get; set; }    // set => re-parse the block
        public IEnumerable<Property> Declarations { get; }
        public int    Length { get; }
        public IRule  Parent { get; }
        public bool   IsStrictMode { get; }    // == !includeUnknownDeclarations
        public string this[int index]  { get; }   // property NAME at index
        public string this[string name] { get; }  // property VALUE by name

        public void   Update(string value);       // replace all declarations
        public void   SetProperty(string propertyName, string propertyValue,
                                  string priority = null);
        public void   SetPropertyValue(string propertyName, string propertyValue);
        public void   SetPropertyPriority(string propertyName, string priority);
        public string GetPropertyValue(string propertyName);
        public string GetPropertyPriority(string propertyName);
        public string RemoveProperty(string propertyName);
        public IEnumerator<IProperty> GetEnumerator();
    }

Behaviour that is easy to get wrong (all verified in source):

  -> SetProperty fails SILENTLY. If the value does not parse, or the property
     name is unknown in strict mode, or priority is neither null nor
     "important", the call simply returns and nothing changes. There is no
     exception and no bool result. Re-read the value to confirm.
  -> SetProperty(name, null) and SetProperty(name, "") remove the property.
  -> priority must be the bare word "important" (case-insensitive), never
     "!important".
  -> Setting a shorthand explodes it into its longhands; the shorthand itself
     is not stored (see the shorthand note below).
  -> Changed fires after SetProperty and RemoveProperty succeed, carrying the
     declaration block's new CssText.

SHORTHANDS: when a stylesheet is parsed, a shorthand declaration is expanded
into its longhand properties. "p { margin: 10px }" produces FOUR declarations
named margin-top, margin-right, margin-bottom and margin-left - there is no
declaration named "margin" in Declarations, and Length is 4. Reading the
shorthand back (style["margin"] or style.Margin) works in strict mode: the
value is re-assembled from the longhands. With includeUnknownDeclarations:
true (strict mode off) that re-assembly does NOT happen and the shorthand
getter returns an empty string.

TYPED CSS PROPERTY ACCESSORS: StyleDeclaration exposes 228 named CSS property
accessors. THEY ARE ALL OF TYPE string, get and set - they are thin wrappers
over GetPropertyValue/SetPropertyValue with the matching PropertyNames
constant. style.Color is a string, NOT the Color struct; style.Width is a
string, NOT a Length. See "THE TYPED CSS VALUE MODEL" for how to get real
typed values.

The accessors, by area (this is the complete list):

  Alignment/box    AlignContent AlignItems AlignSelf AlignmentBaseline
                   VerticalAlign TextAnchor DominantBaseline BaselineShift
                   BoxSizing BoxShadow Bottom Top Left Right Height Width
                   MinHeight MinWidth MaxHeight MaxWidth Position Float Clear
                   Display Visibility Overflow OverflowX OverflowY Zoom ZIndex
                   Opacity Order Clip ClipTop ClipRight ClipBottom ClipLeft
                   ClipPath ClipRule PointerEvents Filter Cursor
  Animation        Animation AnimationDelay AnimationDirection
                   AnimationDuration AnimationFillMode AnimationIterationCount
                   AnimationName AnimationPlayState AnimationTimingFunction
  Background       Background BackgroundAttachment BackgroundClip
                   BackgroundColor BackgroundImage BackgroundOrigin
                   BackgroundPosition BackgroundPositionX BackgroundPositionY
                   BackgroundRepeat BackgroundSize EnableBackground
  Border           Border BorderColor BorderStyle BorderWidth BorderCollapse
                   BorderSpacing BorderRadius BorderTop BorderTopColor
                   BorderTopStyle BorderTopWidth BorderTopLeftRadius
                   BorderTopRightRadius BorderRight BorderRightColor
                   BorderRightStyle BorderRightWidth BorderBottom
                   BorderBottomColor BorderBottomStyle BorderBottomWidth
                   BorderBottomLeftRadius BorderBottomRightRadius BorderLeft
                   BorderLeftColor BorderLeftStyle BorderLeftWidth BorderImage
                   BorderImageOutset BorderImageRepeat BorderImageSlice
                   BorderImageSource BorderImageWidth Outline OutlineColor
                   OutlineStyle OutlineWidth
  Break/page       BreakAfter BreakBefore BreakInside PageBreakAfter
                   PageBreakBefore PageBreakInside Orphans Widows
  Columns          ColumnCount ColumnFill ColumnGap ColumnRule ColumnRuleColor
                   ColumnRuleStyle ColumnRuleWidth Columns ColumnSpan
                   ColumnWidth Gap RowGap
  Container        ContainerName ContainerType
  Content/counter  Content CounterIncrement CounterReset Quotes
  Flexbox          Flex FlexBasis FlexDirection FlexFlow FlexGrow FlexShrink
                   FlexWrap JustifyContent
  Font/text        Font FontFamily FontFeatureSettings FontSize
                   FontSizeAdjust FontStretch FontStyle FontVariant
                   FontWeight Color Direction LetterSpacing LineHeight
                   TextAlign TextAlignLast TextAutospace TextDecoration
                   TextIndent TextJustify TextOverflow TextShadow
                   TextTransform TextUnderlinePosition WhiteSpace WordBreak
                   WordSpacing WritingMode OverflowWrap UnicodeBidirectional
                   ImeMode RubyAlign RubyOverhang RubyPosition
  Lists/tables     ListStyle ListStyleImage ListStylePosition ListStyleType
                   CaptionSide EmptyCells TableLayout
  Margin/padding   Margin MarginTop MarginRight MarginBottom MarginLeft
                   Padding PaddingTop PaddingRight PaddingBottom PaddingLeft
  Mask/SVG paint   Mask Marker MarkerStart MarkerMid MarkerEnd Fill
                   FillOpacity FillRule Stroke StrokeDasharray
                   StrokeDashoffset StrokeLinecap StrokeLinejoin
                   StrokeMiterlimit StrokeOpacity StrokeWidth
                   ColorInterpolationFilters GlyphOrientationHorizontal
                   GlyphOrientationVertical
  Transform        Transform TransformOrigin TransformStyle Perspective
                   PerspectiveOrigin BackfaceVisibility
  Transition       Transition TransitionDelay TransitionDuration
                   TransitionProperty TransitionTimingFunction
  Legacy/vendor    Accelerator Behavior LayoutGrid LayoutGridChar
                   LayoutGridLine LayoutGridMode LayoutGridType
                   Scrollbar3DLightColor ScrollbarArrowColor
                   ScrollbarDarkShadowColor ScrollbarFaceColor
                   ScrollbarHighlightColor ScrollbarShadowColor
                   ScrollbarTrackColor

WATCH OUT: style.Clear is the CSS "clear" property (a string). It hides
StylesheetNode.Clear(), the method that removes all children. To empty a
declaration block use style.CssText = string.Empty (or Update(string.Empty)),
and use ((StylesheetNode)style).Clear() only if you really mean the method.


PROPERTY OBJECTS
----------------
    public interface IProperty : IStylesheetNode
    {
        string Name       { get; }
        string Value      { get; }
        string Original   { get; }
        bool   IsImportant { get; }
    }

    public interface IProperties : IEnumerable<IProperty>
    {
        string this[string propertyName] { get; }
        int    Length { get; }
        string GetPropertyValue(string propertyName);
        string GetPropertyPriority(string propertyName);
        void   SetProperty(string propertyName, string propertyValue,
                           string priority = null);
        string RemoveProperty(string propertyName);
    }

    public abstract class Property : StylesheetNode, IProperty
    {
        public string Name        { get; }
        public string Value       { get; }   // normalized value, or "initial"
        public string Original    { get; }   // raw source text of the value
        public bool   IsImportant { get; set; }
        public string CssText     { get; }   // "name: value" (+ " !important")
        public bool   IsInherited { get; }
        public bool   IsInitial   { get; }
        public bool   IsAnimatable { get; }
        public bool   CanBeInherited { get; }
    }

Property is abstract and every concrete property class in the library is
internal, so you can never pattern-match on a concrete property type. Work
with Property / IProperty.

Value is NORMALIZED, Original is not. Parsing "background-color: #5a5eed"
gives Value == "rgb(90, 94, 237)" and Original == "#5a5eed". If you need the
author's exact text, read Original.

IProperties is also implemented by @font-face and @viewport rules, so those
rules can be enumerated for their IProperty items or queried by name even
though their rule classes are internal.


THE SELECTOR OBJECT MODEL
-------------------------
    public interface ISelector : IStylesheetNode
    {
        Priority Specificity { get; }
        string   Text        { get; }
    }

ISelector has NO Match/matches method in this library - there is no DOM to
match against. Specificity and Text (plus the concrete type) are everything a
selector gives you.

Specificity is the Priority struct:

    public struct Priority : IEquatable<Priority>, IComparable<Priority>
    {
        public Priority(uint priority);
        public Priority(byte inlines, byte ids, byte classes, byte tags);
        public byte Inlines { get; }   // (a) inline style
        public byte Ids     { get; }   // (b) id selectors
        public byte Classes { get; }   // (c) class, attribute, pseudo-class
        public byte Tags    { get; }   // (d) element, pseudo-element
        // static: Zero, OneTag, OneClass, OneId, Inline
        // operators: + == != < > <= >=   (ToString => "(a, b, c, d)")
    }

Abstract bases:

    public abstract class SelectorBase : StylesheetNode, ISelector
        Text and Specificity are fixed at construction; ToCss writes Text.

    public abstract class Selectors : StylesheetNode, IEnumerable<ISelector>
        public Priority Specificity { get; }   // sum of the children
        public string   Text        { get; }
        public int      Length      { get; }
        public ISelector this[int index] { get; set; }
        public void Add(ISelector selector);
        public void Remove(ISelector selector);

SIMPLE SELECTORS (all derive from SelectorBase; all created through a static
Create factory method, their constructors are private):

    AllSelector            "*"        Specificity Zero
                           static AllSelector Create()
    TypeSelector           "div"      Specificity OneTag
                           public string Name { get; }
                           static TypeSelector Create(string name)
    ClassSelector          ".foo"     Specificity OneClass
                           public string Class { get; }
                           static ClassSelector Create(string name)
    IdSelector             "#foo"     Specificity OneId
                           public string Id { get; }
                           static IdSelector Create(string name)
    PseudoClassSelector    ":hover"   Specificity OneClass
                           public string Class { get; }
                           static ISelector Create(string name)
    PseudoElementSelector  "::before" Specificity OneTag
                           public string Name { get; }
                           static ISelector Create(string name)
    NamespaceSelector      "svg|"     Specificity Zero
                           static NamespaceSelector Create(string prefix)

ATTRIBUTE SELECTORS - one class per CSS operator, all deriving from
AttrSelectorBase, all with a public (string attribute, string value)
constructor and all with Specificity OneClass:

    public interface IAttrSelector : ISelector
    {
        string Attribute { get; }
        string Value     { get; }
    }

    public abstract class AttrSelectorBase : SelectorBase, IAttrSelector

    AttrAvailableSelector   [attr]            value is ignored in the text
    AttrMatchSelector       [attr=value]
    AttrNotMatchSelector    [attr!=value]     non-standard, supported here
    AttrListSelector        [attr~=value]
    AttrHyphenSelector      [attr|=value]
    AttrBeginsSelector      [attr^=value]
    AttrEndsSelector        [attr$=value]
    AttrContainsSelector    [attr*=value]

NTH-CHILD FAMILY - ":nth-*(An+B)" forms become a ChildSelector subclass:

    public abstract class ChildSelector : StylesheetNode, ISelector
    {
        public int Step   { get; }   // the A in An+B
        public int Offset { get; }   // the B in An+B
        public Priority Specificity => Priority.OneClass;
        public string   Text { get; }
    }

    FirstChildSelector    :nth-child(An+B)
    LastChildSelector     :nth-last-child(An+B)
    FirstTypeSelector     :nth-of-type(An+B)
    LastTypeSelector      :nth-last-of-type(An+B)
    FirstColumnSelector   :nth-column(An+B)
    LastColumnSelector    :nth-last-column(An+B)

Note the naming: FirstChildSelector models :nth-child(...), not :first-child.
The bare ":first-child" and ":last-child" forms are PseudoClassSelector
instances, not ChildSelector instances.

COMBINING SELECTORS - what the parser hands back for a given selector text:

    "div"               a single simple selector object
    "div.foo[bar]"      CompoundSelector   (no combinators between the parts)
    "div > p"           ComplexSelector    (parts joined by combinators)
    "h1, h2"            ListSelector       (comma-separated group)
    unparsable          UnknownSelector    (only when tolerated; see above)

    public sealed class CompoundSelector : Selectors, ISelector
        Enumerates its ISelector parts; supports LINQ (First(), Last(), Any()).

    public sealed class ListSelector : Selectors, ISelector
        public bool IsInvalid { get; }      // set by the parser
        Enumerates the comma-separated ISelector alternatives.

    public sealed class ComplexSelector
        : StylesheetNode, ISelector, IEnumerable<CombinatorSelector>
    {
        public string   Text     { get; }
        public int      Length   { get; }
        public bool     IsReady  { get; }
        public Priority Specificity { get; }   // sum over the parts
        public void ConcludeSelector(ISelector selector);
        public void AppendSelector(ISelector selector, Combinator combinator);
    }

    public struct CombinatorSelector
    {
        public string    Delimiter { get; }   // ">", " ", "+", "~", "||", "|"
        public ISelector Selector  { get; }   // null Delimiter on the last part
    }

    public sealed class UnknownSelector : StylesheetNode, ISelector
        Specificity is Priority.Zero. Text is the raw source text when the
        selector came from a parsed stylesheet, and empty when it came from a
        standalone ParseSelector call.

COMBINATORS:

    public abstract class Combinator
    {
        public static readonly Combinator Child;            // ">"
        public static readonly Combinator Deep;             // ">>>"
        public static readonly Combinator Descendent;       // " "
        public static readonly Combinator AdjacentSibling;  // "+"
        public static readonly Combinator Sibling;          // "~"
        public static readonly Combinator Namespace;        // "|"
        public static readonly Combinator Column;           // "||"
        public string Delimiter { get; }
        public virtual ISelector Change(ISelector selector);
    }

    public static class Combinators   // the raw delimiter strings
    {
        Exactly "="   Unlike "!="  InList "~="  InToken "|="  Begins "^="
        Ends "$="     InText "*="  Column "||"  Pipe "|"      Adjacent "+"
        Descendent " " Deep ">>>"  Child ">"    Sibling "~"
    }

OTHER SELECTOR-SHAPED NODES:

    public sealed class KeyframeSelector : StylesheetNode   // NOT an ISelector
    {
        public KeyframeSelector(IEnumerable<Percent> stops);
        public IEnumerable<Percent> Stops { get; }
        public string Text { get; }
    }

    public sealed class PageSelector : StylesheetNode, ISelector
    {
        public PageSelector();              // "" (no pseudo page)
        public PageSelector(string name);   // ":left", ":right", ":first"
        public Priority Specificity => Priority.Inline;
        public string   Text { get; }
    }

SELECTOR FACTORIES: AttributeSelectorFactory, PseudoClassSelectorFactory and
PseudoElementSelectorFactory are public classes with a public Create method,
but their instance accessors and constructors are internal - the parser owns
the only instances, and consumer code CANNOT obtain one. To build selectors
yourself, use the static Create methods on the selector classes (or the
public attribute-selector constructors), or just call parser.ParseSelector.

    // What the factories do internally, for reference:
    // AttributeSelectorFactory.Create(combinator, match, value, prefix)
    //     maps "=" -> AttrMatchSelector, "~=" -> AttrListSelector,
    //     "|=" -> AttrHyphenSelector, "^=" -> AttrBeginsSelector,
    //     "$=" -> AttrEndsSelector, "*=" -> AttrContainsSelector,
    //     "!=" -> AttrNotMatchSelector, anything else -> AttrAvailableSelector.
    // PseudoClassSelectorFactory.Create(name) returns a cached
    //     PseudoClassSelector for the ~34 standard pseudo-classes, else null.
    // PseudoElementSelectorFactory.Create(name) returns a cached
    //     PseudoElementSelector for before, after, selection, first-line,
    //     first-letter and content; for any other name it returns null unless
    //     the parser was created with tolerateInvalidSelectors: true.


MEDIA QUERIES
-------------
    public interface IMediaRule : IConditionRule
    {
        MediaList Media { get; }
    }

    public sealed class MediaList : StylesheetNode
    {
        public string MediaText { get; set; }       // whole query list
        public IEnumerable<Medium> Media { get; }
        public int    Length { get; }
        public string this[int index] { get; }      // medium as CSS text
        public void Add(string newMedium);          // throws on bad input
        public void Remove(string oldMedium);       // throws if not found
        public IEnumerator<Medium> GetEnumerator();
    }

    public sealed class Medium : StylesheetNode
    {
        public string Type        { get; }   // "screen", "print", "all", ...
        public bool   IsExclusive { get; }   // "only screen"
        public bool   IsInverse   { get; }   // "not screen"
        public string Constraints { get; }   // features joined with " and "
        public IEnumerable<MediaFeature> Features { get; }
    }

    public interface IMediaFeature : IStylesheetNode
    {
        string Name     { get; }
        string Value    { get; }
        bool   HasValue { get; }
    }

    public abstract class MediaFeature : StylesheetNode, IMediaFeature
    {
        public bool   IsMinimum { get; }   // name starts with "min-"
        public bool   IsMaximum { get; }   // name starts with "max-"
        public string Name      { get; }
        public string Value     { get; }   // "" when HasValue is false
        public bool   HasValue  { get; }
    }

All concrete media-feature classes are internal; you always receive them as
MediaFeature / IMediaFeature. Use FeatureNames for the recognised names.


CONDITION RULES: @supports, @container, @document
-------------------------------------------------
    public interface ISupportsRule : IConditionRule
    {
        IConditionFunction Condition { get; }
    }

    public interface IConditionFunction : IStylesheetNode
    {
        bool Check();
    }

Condition.Check() reports whether THIS LIBRARY can parse the declarations in
the condition - it is not a browser-support query and knows nothing about any
real user agent. "and", "or", "not" and parenthesised groups are all honoured,
and an empty condition checks true.

    public interface IContainerRule : IConditionRule
    {
        string    Name  { get; set; }    // optional container name
        MediaList Media { get; }         // the size query, as a media list
    }

@document has no dedicated public rule interface. Find it by RuleType and read
its conditions from the children:

    public interface IDocumentFunction : IStylesheetNode
    {
        string Name { get; }   // "url", "url-prefix", "domain", "regexp"
        string Data { get; }   // the argument text
    }

    var docRules = stylesheet.Children.OfType<IRule>()
                             .Where(r => r.Type == RuleType.Document);
    foreach (var rule in docRules)
        foreach (var fn in rule.Children.OfType<IDocumentFunction>())
            Console.WriteLine($"{fn.Name}({fn.Data})");


AT-RULES WITH A DEDICATED CONTRACT
----------------------------------
    public interface IKeyframesRule : IRule
    {
        string    Name  { get; set; }
        IRuleList Rules { get; }
        void Add(string rule);
        void Remove(string key);
        IKeyframeRule Find(string key);
    }

    public interface IKeyframeRule : IRule
    {
        string            KeyText { get; set; }   // "0%", "100%", "from", "to"
        StyleDeclaration  Style   { get; }
        KeyframeSelector  Key     { get; set; }
    }

    public interface IFontFaceRule : IRule, IProperties
    {
        string Family   { get; set; }   // font-family
        string Source   { get; set; }   // src
        string Style    { get; set; }   // font-style
        string Weight   { get; set; }   // font-weight
        string Stretch  { get; set; }   // font-stretch
        string Range    { get; set; }   // unicode-range
        string Variant  { get; set; }   // font-variant
        string Features { get; set; }   // font-feature-settings
    }

    public interface IImportRule : IRule
    {
        string    Href  { get; set; }
        MediaList Media { get; }
    }

    public interface INamespaceRule : IRule
    {
        string NamespaceUri { get; set; }
        string Prefix       { get; set; }
    }

    public interface ICharsetRule : IRule
    {
        string CharacterSet { get; set; }
    }

    public sealed class CharsetRule : Rule, ICharsetRule
    {
        public CharsetRule(StylesheetParser parser);   // publicly constructible
    }

    public interface IPageRule : IRule
    {
        string           SelectorText { get; set; }
        StyleDeclaration Style        { get; }
    }

Margin boxes inside @page are MarginStyleRule children of the page rule:

    foreach (var page in stylesheet.PageRules)
        foreach (var box in page.Children.OfType<MarginStyleRule>())
            Console.WriteLine($"{box.SelectorText} -> {box.Style.CssText}");


THE TYPED CSS VALUE MODEL
-------------------------
READ THIS FIRST: the parsed model NEVER hands you one of these value types.
Declaration values are strings (see "TYPED CSS PROPERTY ACCESSORS"), and every
class that would carry a typed value - the concrete Property subclasses, the
concrete ITransform implementations, the value converters - is internal.

The value types below are a public VOCABULARY: value structs and classes you
construct yourself, plus static parse helpers you point at the strings the
declaration API gives you. That is how you go from CSS text to typed data.

    // Typical flow
    var raw = rule.Style.Width;                    // "50%"  (a string)
    if (Length.TryParse(raw, out var width))       // typed
        Console.WriteLine($"{width.Value} {width.UnitString}");

LENGTH

    public struct Length : IEquatable<Length>, IComparable<Length>, IFormattable
    {
        public Length(float value, Length.Unit unit);
        public float  Value      { get; }
        public Unit   Type       { get; }
        public string UnitString { get; }     // "px", "em", "%", ...
        public bool   IsAbsolute { get; }     // In, Mm, Pc, Px, Pt, Cm
        public bool   IsRelative { get; }
        public float  ToPixel();              // absolute units only
        public float  To(Unit unit);          // absolute units only
        public static bool TryParse(string s, out Length result);
        public static Unit GetUnit(string s); // "px" -> Unit.Px
        // static values: Zero, Half (50%), Full (100%), Thin (1px),
        //                Medium (3px), Thick (5px), Missing
        // operators: == != < > <= >=
    }

    public enum Length.Unit : byte
    { None, Px, Em, Ex, Cm, Mm, In, Pt, Pc, Ch, Rem, Vw, Vh, Vmin, Vmax, Percent }

ToPixel() and To() throw InvalidOperationException for relative units - guard
with IsAbsolute. TryParse also accepts a bare "0" (returns Length.Zero).

ANGLE / TIME / FREQUENCY / RESOLUTION / NUMBER / PERCENT - the same shape:

    public struct Angle : ... { Angle(float, Angle.Unit); Value; Type;
        UnitString; float ToRadian(); float ToTurns();
        static bool TryParse(string, out Angle); static Unit GetUnit(string);
        static values Zero, HalfQuarter (45deg), Quarter (90deg),
        TripleHalfQuarter (135deg), Half (180deg) }
    public enum Angle.Unit : byte { None, Deg, Rad, Grad, Turn }

    public struct Time : ... { Time(float, Time.Unit); Value; Type; UnitString;
        float ToMilliseconds(); static Unit GetUnit(string); static Time Zero }
    public enum Time.Unit : byte { None, Ms, S }
    // NOTE: Time has NO TryParse - use GetUnit plus your own number parse.

    public struct Frequency : ... { Frequency(float, Frequency.Unit); Value;
        Type; UnitString; float ToHertz();
        static bool TryParse(string, out Frequency); static Unit GetUnit(string) }
    public enum Frequency.Unit : byte { None, Hz, Khz }

    public struct Resolution : ... { Resolution(float, Resolution.Unit); Value;
        Type; UnitString; float ToDotsPerPixel(); float To(Unit);
        static bool TryParse(string, out Resolution); static Unit GetUnit(string) }
    public enum Resolution.Unit : byte { None, Dpi, Dpcm, Dppx }

    public struct Number : ... { Number(float, Number.Unit); float Value;
        bool IsInteger; static values Zero, One, Infinite }
    public enum Number.Unit : byte { Integer, Float, Percent }

    public struct Percent : ... { Percent(float value); float Value;
        float NormalizedValue;   // Value * 0.01
        static values Zero, Fifty, Hundred }

All of them implement IEquatable<T>, IComparable<T> and IFormattable, and all
define == != < > <= >=.

COLOR

    public struct Color : IEquatable<Color>, IComparable<Color>, IFormattable
    {
        public Color(byte r, byte g, byte b);
        public Color(byte red, byte green, byte blue, byte alpha);
        public byte   R { get; }  public byte G { get; }  public byte B { get; }
        public byte   A { get; }                  // raw alpha 0-255
        public double Alpha { get; }              // 0.0-1.0, rounded to 2 dp
        public int    Value { get; }              // packed hash value

        public static Color  FromRgb(byte red, byte green, byte blue);
        public static Color  FromRgba(byte red, byte green, byte blue, float alpha);
        public static Color  FromRgba(float red, float green, float blue, float alpha);
        public static Color  FromGray(byte number, float alpha = 1f);
        public static Color  FromGray(float value, float alpha = 1f);
        public static Color  FromHex(string color);
        public static bool   TryFromHex(string color, out Color value);
        public static Color  FromFlexHex(string color);   // quirks-mode hex
        public static Color  FromHsl(float hue, float saturation, float luminosity);
        public static Color  FromHsla(float h, float s, float l, float alpha);
        public static Color  FromHwb(float hue, float whiteness, float blackness);
        public static Color  FromHwba(float h, float w, float b, float alpha);
        public static Color? FromName(string name);       // "rebeccapurple"
        public static Color  Mix(Color above, Color below);
        public static Color  Mix(double alpha, Color above, Color below);
        // static values: Black, White, Red, Magenta, Green (0,128,0),
        //                PureGreen (0,255,0), Blue, Transparent
    }

FromHex/TryFromHex take 3, 4, 6 or 8 hex digits WITHOUT a leading '#'. Four
digits are #rgba and eight are #rrggbbaa. FromHex does not validate - use
TryFromHex for untrusted input. Color.ToString() emits "rgb(r, g, b)", or
"rgba(r, g, b, a)" when alpha is not 255 - the same normalized form the
declaration API returns.

    public static class Colors
    {
        public static IEnumerable<string> Names { get; }   // all CSS names
        public static Color? GetColor(string name);        // name -> color
        public static string GetName(Color color);         // color -> name
    }

COMPOSITE VALUE TYPES (constructible; the parser does not produce them)

    public struct Point
    {
        public Point(Length x, Length y);
        public Length X { get; }  public Length Y { get; }
        // static: Center, LeftTop, RightTop, RightBottom, LeftBottom
    }

    public sealed class Shadow
    {
        public Shadow(bool inset, Length offsetX, Length offsetY,
                      Length blurRadius, Length spreadRadius, Color color);
        public Color  Color { get; }        public bool   IsInset { get; }
        public Length OffsetX { get; }      public Length OffsetY { get; }
        public Length BlurRadius { get; }   public Length SpreadRadius { get; }
    }

    public sealed class Shape           // the rect() clip shape
    {
        public Shape(Length top, Length right, Length bottom, Length left);
        public Length Top { get; }    public Length Right { get; }
        public Length Bottom { get; } public Length Left { get; }
    }

    public sealed class Counter
    {
        public Counter(string identifier, string listStyle, string separator);
        public string CounterIdentifier { get; }
        public string ListStyle { get; }
        public string DefinedSeparator { get; }
    }

IMAGE SOURCES AND GRADIENTS

    public interface IImageSource { }            // pure marker, no members
    public interface IGradient : IImageSource
    {
        IEnumerable<GradientStop> Stops { get; }
        bool IsRepeating { get; }
    }

    public struct GradientStop
    {
        public GradientStop(Color color, Length location);
        public Color  Color    { get; }
        public Length Location { get; }
    }

    public sealed class LinearGradient : IGradient
    {
        public LinearGradient(Angle angle, GradientStop[] stops,
                              bool repeating = false);
        public Angle Angle { get; }
        public IEnumerable<GradientStop> Stops { get; }
        public bool IsRepeating { get; }
    }

    public sealed class RadialGradient : IImageSource     // note: not IGradient
    {
        public RadialGradient(bool circle, Point pt, Length width,
                              Length height, RadialGradient.SizeMode sizeMode,
                              GradientStop[] stops, bool repeating = false);
        public bool   IsCircle { get; }   public SizeMode Mode { get; }
        public Point  Position { get; }
        public Length MajorRadius { get; } public Length MinorRadius { get; }
        public IEnumerable<GradientStop> Stops { get; }
        public bool   IsRepeating { get; }
    }

    public enum RadialGradient.SizeMode : byte
    { None, ClosestCorner, ClosestSide, FarthestCorner, FarthestSide }

TRANSFORMS

    public interface ITransform
    {
        TransformMatrix ComputeMatrix();
    }

    public sealed class TransformMatrix : IEquatable<TransformMatrix>
    {
        public TransformMatrix(float[] values);          // 15 values
        public TransformMatrix(float m11, float m12, float m13,
                               float m21, float m22, float m23,
                               float m31, float m32, float m33,
                               float tx,  float ty,  float tz,
                               float px,  float py,  float pz);
        public float Tx { get; }  public float Ty { get; }  public float Tz { get; }
        // static values: Zero, One (identity)
    }

Every concrete ITransform implementation (translate, rotate, scale, skew,
matrix, perspective) is internal. ITransform is here so you can implement it
yourself; the parser will not give you one.

TIMING FUNCTIONS

    public interface ITimingFunction { }          // pure marker, no members

    public sealed class CubicBezierTimingFunction : ITimingFunction
    {
        public CubicBezierTimingFunction(float x1, float y1, float x2, float y2);
        public float X1 { get; } public float Y1 { get; }
        public float X2 { get; } public float Y2 { get; }
    }

    public sealed class StepsTimingFunction : ITimingFunction
    {
        public StepsTimingFunction(int intervals, bool start = false);
        public int  Intervals { get; }
        public bool IsStart   { get; }
    }

URL

    public sealed class Url : IEquatable<Url>
    {
        public Url(string address);
        public Url(Url baseAddress, string relativeAddress);
        public Url(Url address);
        public static Url Create(string address);
        public static Url Convert(Uri uri);
        public static implicit operator Uri(Url value);
        public string Href { get; }      public string Origin   { get; }
        public string Scheme { get; }    public string Host     { get; }
        public string HostName { get; }  public string Port     { get; }
        public string Path { get; }      public string Query    { get; }
        public string Fragment { get; }  public string Data     { get; }
        public string UserName { get; set; }  public string Password { get; set; }
        public bool   IsInvalid { get; } public bool   IsRelative { get; }
    }

    public static class ProtocolNames
    {
        // Http, Https, Ftp, JavaScript, Data, Mailto, File, Ws, Wss, Telnet,
        // Ssh, Gopher, Blob
        public static bool IsRelative(string protocol);
        public static bool IsOriginable(string protocol);
    }


CSS ENUMERATIONS
----------------
The library ships around fifty public CSS enums (all "byte"-backed). Like the
value types, they are a VOCABULARY: no public member returns one, so you use
them for your own modelling and for mapping the strings you read out of
declarations. Values are listed for the commonly used ones.

LAYOUT AND BOX

    DisplayMode        None, Inline, Block, ListItem, InlineBlock, InlineTable,
                       Table, TableCaption, TableCell, TableColumn,
                       TableColumnGroup, TableFooterGroup, TableHeaderGroup,
                       TableRow, TableRowGroup, Flex, InlineFlex, Grid,
                       InlineGrid
    PositionMode       Static, Relative, Absolute, Fixed, Sticky
    Visibility         Visible, Hidden, Collapse
    Overflow           Auto, Visible, Hidden, Scroll
    Floating           None, Left, Right
    ClearMode          None, Left, Right, Both
    BoxModel           BorderBox, PaddingBox, ContentBox
    IntrinsicSizing    MaxContent, MinContent, FitContent, Content
    ObjectFitting      None, Fill, Contain, Cover, ScaleDown
    ContainerType      Normal, Size, InlineSize
    BreakMode          Auto, Always, Avoid, Left, Right, Page, Column,
                       AvoidPage, AvoidColumn, AvoidRegion

FLEXBOX AND ALIGNMENT

    FlexDirection      Row, RowReverse, Column, ColumnReverse
    FlexWrap           NoWrap, Wrap, WrapReverse
    AlignItem          Normal, Stretch, Center, Start, End, FlexStart, FlexEnd,
                       SelfStart, SelfEnd, Baseline
    AlignContent       Center, Start, End, FlexStart, FlexEnd, Normal,
                       Baseline, SpaceBetween, SpaceAround, SpaceEvenly, Stretch
    JustifyContent     Start, Center, SpaceBetween, SpaceAround, SpaceEvenly,
                       End, FlexStart, FlexEnd, Left, Right, Normal, Stretch
    HorizontalAlignment  Left, Center, Right, Justify
    VerticalAlignment    Baseline, Sub, Super, TextTop, TextBottom, Middle,
                         Top, Bottom

FONTS AND TEXT

    FontWeight         Normal, Bold, Bolder, Lighter
    FontStyle          Normal, Italic, Oblique
    FontVariant        Normal, SmallCaps
    FontStretch        Normal, UltraCondensed, ExtraCondensed, Condensed,
                       SemiCondensed, SemiExpanded, Expanded, ExtraExpanded,
                       UltraExpanded
    FontSize           Custom, Tiny, Little, Smaller, Small, Medium, Large,
                       Larger, Big, Huge
    SystemFont         Caption, Icon, Menu, MessageBox, SmallCaption, StatusBar
    TextAlignLast      Auto, Start, End, Left, Right, Center, Justify
    TextDecorationLine Underline, Overline, LineThrough, Blink
    TextDecorationStyle Solid, Double, Dotted, Dashed, Wavy
    TextTransform      None, Capitalize, Uppercase, Lowercase, FullWidth
    TextAnchor         Start, Middle, End
    TextJustify        Auto, InterWord, InterIdeograph, InterCluster,
                       Distribute, DistributeAllLines, DistributeCenterLast,
                       Kashida, Newspaper
    Whitespace         Normal, Pre, NoWrap, PreWrap, PreLine
    WordBreak          Normal, BreakAll, KeepAll
    OverflowWrap       Normal, BreakWord
    DirectionMode      Ltr, Rtl
    UnicodeMode        Normal, Embed, Isolate, BidirectionalOverride,
                       IsolateOverride, Plaintext

PAINT, BORDERS AND EFFECTS

    BlendMode          Normal, Multiply, Screen, Overlay, Darken, Lighten,
                       ColorDodge, ColorBurn, HardLight, SoftLight, Difference,
                       Exclusion, Hue, Saturation, Color, Luminosity
    FillRule           Nonzero, Evenodd
    LineStyle          None, Hidden, Dotted, Dashed, Solid, Double, Groove,
                       Ridge, Inset, Outset
    BorderRepeat       Stretch, Repeat, Round
    StrokeLinecap      Butt, Round, Square
    StrokeLinejoin     Miter, Round, Bevel

BACKGROUNDS, LISTS AND ANIMATION

    BackgroundAttachment  Fixed, Local, Scroll
    BackgroundRepeat      Repeat, Space, Round, NoRepeat
    ListPosition          Inside, Outside
    ListStyle             None, Disc, Circle, Square, Decimal,
                          DecimalLeadingZero, LowerRoman, UpperRoman,
                          LowerGreek, LowerLatin, UpperLatin, Armenian,
                          Georgian
    AnimationDirection    Normal, Alternate, Reverse, AlternateReverse
    AnimationFillStyle    None, Forwards, Backwards, Both
    PlayState             Running, Paused

INTERACTION AND MEDIA-FEATURE VOCABULARY

    SystemCursor       Auto, Default, None, ContextMenu, Help, Pointer,
                       Progress, Wait, Cell, Crosshair, Text, VerticalText,
                       Alias, Copy, Move, NoDrop, NotAllowed, EResize, NResize,
                       NeResize, NwResize, SResize, SeResize, SwResize,
                       WResize, EwResize, NsResize, NeswResize, NwseResize,
                       ColResize, RowResize, AllScroll, ZoomIn, ZoomOut, Grab,
                       Grabbing
    HoverAbility       None, OnDemand, Hover
    PointerAccuracy    None, Coarse, Fine
    ScriptingState     None, InitialOnly, Enabled
    UpdateFrequency    None, Slow, Normal

STRUCTURAL

    RuleType           (see "RULE TYPES AND HOW TO REACH THEM")
    ParseError         (see "ERRORS, DIAGNOSTICS AND SOURCE POSITIONS")
    Length.Unit, Angle.Unit, Time.Unit, Frequency.Unit, Resolution.Unit,
    Number.Unit, RadialGradient.SizeMode  (nested; see the value model)


CSS SERIALIZATION
-----------------
Every node implements IStyleFormattable, so anything in the tree can be
written out:

    void ToCss(TextWriter writer, IStyleFormatter formatter)

Extension methods (public static class FormatExtensions):

    string ToCss(this IStyleFormattable style)
    string ToCss(this IStyleFormattable style, IStyleFormatter formatter)
    void   ToCss(this IStyleFormattable style, TextWriter writer)

The parameterless overloads use CompressedStyleFormatter.Instance.

    public sealed class CompressedStyleFormatter : IStyleFormatter
    {
        public static readonly IStyleFormatter Instance;
    }

CompressedStyleFormatter implements every IStyleFormatter member EXPLICITLY,
so you must hold it as IStyleFormatter to call anything on it (the static
Instance field is already typed IStyleFormatter). Despite the name its output
is readable, not minified: rules are joined with Environment.NewLine and a
block is written as "selector { a: b; c: d }".

    public interface IStyleFormatter
    {
        string Sheet(IEnumerable<IStyleFormattable> rules);
        string Block(IEnumerable<IStyleFormattable> rules);
        string Declaration(string name, string value, bool important);
        string Declarations(IEnumerable<string> declarations);
        string Medium(bool exclusive, bool inverse, string type,
                      IEnumerable<IStyleFormattable> constraints);
        string Constraint(string name, string value, string constraintDelimiter);
        string Rule(string name, string value);
        string Rule(string name, string prelude, string rules);
        string Style(string selector, IStyleFormattable rules);
        string Comment(string data);
    }

Implement IStyleFormatter to control minified or pretty output; there is only
one formatter in the box.


CONSTANTS AND REFERENCE CLASSES
-------------------------------
Public static classes of string constants. Use them instead of literals.

    PropertyNames        237 CSS property names
                         PropertyNames.Color, .FontSize, .Margin, .Padding,
                         .Display, .Position, .AlignContent, .Animation,
                         .BackgroundColor, .BorderWidth, ...
    RuleNames            11 at-rule names (no leading '@'):
                         Supports, Charset, Document, FontFace, ViewPort,
                         Import, Keyframes, Media, Namespace, Page, Container
    FunctionNames        44 CSS function names: Url, UrlPrefix, Domain,
                         Regexp, Rgb, Rgba, Hsl, Hsla, Hwb, Gray, Rect, Attr,
                         Calc, Toggle, Counter, Counters, Image,
                         LinearGradient, RadialGradient,
                         RepeatingLinearGradient, RepeatingRadialGradient,
                         Translate/TranslateX/Y/Z/3d, Matrix, Matrix3d,
                         Rotate/RotateX/Y/Z/3d, Skew/SkewX/SkewY,
                         Scale/ScaleX/Y/Z/3d, Steps, CubicBezier, Perspective
    PseudoClassNames     48 constants: 47 pseudo-class names plus
                         Separator (":")
                         Root, Scope, Active, Focus, FocusVisible, FocusWithin,
                         Hover, Link, Visited, AnyLink, Target, Empty, Enabled,
                         Disabled, Checked, Unchecked, Indeterminate, Default,
                         Valid, Invalid, Required, Optional, ReadOnly,
                         ReadWrite, InRange, OutOfRange, PlaceholderShown,
                         FirstChild, LastChild, OnlyChild, FirstOfType,
                         LastOfType, OnlyType, NthChild, NthLastChild,
                         NthOfType, NthLastOfType, NthColumn, NthLastColumn,
                         Shadow, ...
    PseudoElementNames   Before, After, Selection, FirstLine, FirstLetter,
                         Content, plus Separator ("::")
    FeatureNames         42 media-feature names: Width, Height, MinWidth,
                         MaxWidth, MinHeight, MaxHeight, DeviceWidth,
                         DeviceHeight, AspectRatio, DeviceAspectRatio,
                         Resolution, Color, ColorIndex, Monochrome,
                         Orientation, Grid, Scan, DevicePixelRatio,
                         UpdateFrequency, Scripting, Pointer, Hover,
                         BlockSize, InlineSize (each with min-/max- variants)
    UnitNames            26 unit strings: Px, Em, Ex, Cm, Mm, In, Pt, Pc, Ch,
                         Rem, Vw, Vh, Vmin, Vmax, Percent ("%"), Deg, Rad,
                         Grad, Turn, Ms, S, Hz, Khz, Dpi, Dpcm, Dppx
    Combinators          the raw selector delimiter strings (see the selector
                         section)
    Colors               named-color lookups: Names, GetColor, GetName
    ProtocolNames        URL scheme constants and IsRelative/IsOriginable

There is NO public Keywords class. The CSS keyword constants ("important",
"inherit", "initial", "unset", "auto", "none", ...) are internal to this
library; write the literal strings in your own code.

    public static class TextEncoding      // charset helpers for @charset work
    {
        public static HashSet<string> AvailableEncodings;
        public static readonly Encoding Utf8, Utf16Be, Utf16Le, Utf32Le,
            Utf32Be, Gb18030, Big5, Windows874, Windows1250 ... Windows1258,
            Latin2, Latin3, Latin4, Latin5, Latin13, UsAscii, Korean;
        public static bool IsUnicode(this Encoding encoding);
        public static bool IsSupported(string charset);
        public static Encoding Resolve(string charset);
    }


ERRORS, DIAGNOSTICS AND SOURCE POSITIONS
----------------------------------------
Parsing is forgiving by design: malformed rules, selectors and declarations
are normally DROPPED, not reported. There is no public error-callback or
error-collection API. What you do get:

1. ParseException - thrown from the MUTATION and re-parse paths.

    public class ParseException : Exception
    {
        public ParseException(string message);
    }

   Thrown by (verified): IRule.Text setter ("Unable to parse rule", "Invalid
   rule type"); Stylesheet.Insert / RemoveAt and IGroupingRule.Insert /
   RemoveAt ("Invalid index", "Rule argument cannot be null", "Cannot insert
   Charset rule", "Cannot insert namespace or declarative rules", "Cannot
   remove namespace or declarative rules"); MediaList.MediaText setter, Add
   and Remove ("Unable to parse media list element", "Unable to parse medium",
   "Media list element not found"); IKeyframeRule.KeyText setter; and, during
   parsing, an unterminated media prelude, an unparsable @document condition
   list, an unparsable @supports condition, an out-of-range integer, and an
   unsupported rule in an @namespace context.

   Guard every mutation call - especially Insert/RemoveAt and MediaList work -
   with try/catch (ParseException).

2. Source positions - available on nodes, not on errors.

    public class StylesheetText
    {
        public TextRange Range { get; }
        public string    Text  { get; }   // the raw source slice
    }

    public struct TextRange : IEquatable<TextRange>, IComparable<TextRange>
    {
        public TextRange(TextPosition start, TextPosition end);
        public TextPosition Start { get; }
        public TextPosition End   { get; }
        // operators < > ; ToString() => "Start -> End"
    }

    public struct TextPosition : IEquatable<TextPosition>, IComparable<TextPosition>
    {
        public TextPosition(ushort line, ushort column, int position);
        public int Line     { get; }   // 1-based
        public int Column   { get; }   // 1-based
        public int Position { get; }   // absolute character index
        public TextPosition Shift(int columns);
        public TextPosition After(char chr);
        public TextPosition After(string str);
        public static readonly TextPosition Empty;
        // ToString() => "Line L, Column C, Position P"
    }

   Rules and selectors parsed from a stylesheet carry StylesheetText, so
   rule.StylesheetText.Range.Start.Line tells you where a rule came from, and
   rule.StylesheetText.Text gives you its exact original text. Nodes built by
   hand, and selectors from a standalone ParseSelector call, have a null
   StylesheetText - always null-check it.

3. ParseError / TokenizerError - the tokenizer's error vocabulary.

    public enum ParseError : byte
    {
        EOF = 0, InvalidCharacter = 16, InvalidBlockStart = 17,
        InvalidToken = 18, ColonMissing = 19, IdentExpected = 20,
        InputUnexpected = 21, LineBreakUnexpected = 22, UnknownAtRule = 32,
        InvalidSelector = 48, InvalidKeyframe = 49, ValueMissing = 64,
        InvalidValue = 65, UnknownDeclarationName = 80
    }

    public class TokenizerError
    {
        public TokenizerError(ParseError code, TextPosition position);
        public TextPosition Position { get; }
        public int          Code     { get; }   // the numeric ParseError value
        public string       Message  { get; }   // always "An unknown error
                                                //  occurred." in this version
    }

   IMPORTANT: the tokenizer raises these internally, but the class that
   exposes the Error event (Lexer) is internal, so CONSUMER CODE CANNOT
   SUBSCRIBE. Treat ParseError and TokenizerError as a vocabulary you may use
   in your own diagnostics. To detect problems, compare what you parsed with
   what you expected (rule counts, null selectors, empty property values).

4. TextSource - the reader the parser wraps around your input. Public and
   usable, mainly of interest when you care about encoding.

    public sealed class TextSource : IDisposable
    {
        public TextSource(string source);
        public TextSource(Stream baseStream, Encoding encoding = null);
        public string   Text  { get; }      public char this[int index] { get; }
        public int      Index { get; set; } public int Length { get; }
        public Encoding CurrentEncoding { get; set; }
        public char     ReadCharacter();
        public string   ReadCharacters(int characters);
        public Task     PrefetchAllAsync(CancellationToken cancellationToken);
        public void     Dispose();
    }


================================================================================

COMPLETE EXAMPLES
=================

Example 1: Parse and Inspect a Stylesheet
-----------------------------------------
    using System;
    using System.Linq;
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
------------------------------------------
    using System;
    using System.Linq;
    using CodeBrix.StyleSheetParse;

    var parser = new StylesheetParser();
    var stylesheet = parser.Parse("h1 { color: red; } p { margin: 10px; }");

    // Modify a property
    var firstRule = stylesheet.StyleRules.First();
    firstRule.Style.SetProperty("color", "blue");
    firstRule.Style.SetProperty("font-weight", "bold");

    // Add a new property with !important ("important", never "!important")
    firstRule.Style.SetProperty("text-align", "center", "important");

    // SetProperty is silent on failure - verify when the input is untrusted
    firstRule.Style.SetProperty("color", "not-a-color");
    Console.WriteLine(firstRule.Style.Color);   // still "rgb(0, 0, 255)"

    // Serialize back to CSS
    string css = stylesheet.ToCss();
    Console.WriteLine(css);


Example 3: Parse Selectors and Calculate Specificity
----------------------------------------------------
    using System;
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
        if (selector == null)
        {
            Console.WriteLine($"{selectorText} => invalid");
            continue;
        }

        var s = selector.Specificity;
        Console.WriteLine($"{selectorText} => ({s.Inlines},{s.Ids},{s.Classes},{s.Tags})");
    }

    // Compare two selectors with the overloaded operators
    var a = parser.ParseSelector("#main .row");
    var b = parser.ParseSelector("div.row span");
    Console.WriteLine(a.Specificity > b.Specificity);   // True


Example 4: Async Parsing from a Stream
--------------------------------------
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using CodeBrix.StyleSheetParse;

    await using var stream = File.OpenRead("styles.css");
    var parser = new StylesheetParser();

    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
    var stylesheet = await parser.ParseAsync(stream, cts.Token);

    Console.WriteLine($"Parsed {stylesheet.StyleRules.Count()} style rules");
    Console.WriteLine($"Parsed {stylesheet.MediaRules.Count()} media rules");
    Console.WriteLine($"Parsed {stylesheet.FontfaceSetRules.Count()} font-face rules");


Example 5: Tolerant Parsing with Comments
-----------------------------------------
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


Example 6: Walking Every Rule, Including @keyframes
---------------------------------------------------
    using System;
    using System.Linq;
    using CodeBrix.StyleSheetParse;

    var parser = new StylesheetParser();
    var stylesheet = parser.Parse(@"
        @keyframes fadeIn {
            from { opacity: 0; }
            to { opacity: 1; }
        }
        .animated { animation: fadeIn 1s ease-in; }
    ");

    // Stylesheet.Rules is internal - Children is the public way to walk
    // every rule in document order.
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
        else
        {
            Console.WriteLine($"{rule.Type}: {rule.Text}");
        }
    }


Example 7: Working with @font-face
----------------------------------
    using System;
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

        // IFontFaceRule is also an IProperties, so it enumerates declarations
        foreach (var declaration in fontFace)
        {
            Console.WriteLine($"  {declaration.Name}: {declaration.Value}");
        }
    }


Example 8: Working with @container Queries
------------------------------------------
    using System;
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

        foreach (var medium in container.Media.Media)
            foreach (var feature in medium.Features)
                Console.WriteLine($"  {feature.Name} = {feature.Value}");

        foreach (IRule inner in container.Rules)
            Console.WriteLine($"  inner rule: {inner.Text}");
    }


Example 9: Walking a Parsed Selector
------------------------------------
    using System;
    using System.Linq;
    using CodeBrix.StyleSheetParse;

    var parser = new StylesheetParser();

    void Describe(ISelector selector, int depth = 0)
    {
        var pad = new string(' ', depth * 2);

        switch (selector)
        {
            case ListSelector list:                 // "h1, h2"
                Console.WriteLine($"{pad}list ({list.Length}) {list.Text}");
                foreach (var part in list) Describe(part, depth + 1);
                break;

            case ComplexSelector complex:           // "div > p"
                Console.WriteLine($"{pad}complex ({complex.Length}) {complex.Text}");
                foreach (CombinatorSelector part in complex)
                {
                    Describe(part.Selector, depth + 1);
                    if (part.Delimiter != null)
                        Console.WriteLine($"{pad}  combinator '{part.Delimiter}'");
                }
                break;

            case CompoundSelector compound:         // "div.foo[bar]"
                Console.WriteLine($"{pad}compound ({compound.Length}) {compound.Text}");
                foreach (var part in compound) Describe(part, depth + 1);
                break;

            case IdSelector id:
                Console.WriteLine($"{pad}id #{id.Id}");
                break;
            case ClassSelector cls:
                Console.WriteLine($"{pad}class .{cls.Class}");
                break;
            case TypeSelector type:
                Console.WriteLine($"{pad}type {type.Name}");
                break;
            case AllSelector:
                Console.WriteLine($"{pad}universal *");
                break;
            case PseudoElementSelector pe:
                Console.WriteLine($"{pad}pseudo-element ::{pe.Name}");
                break;
            case PseudoClassSelector pc:
                Console.WriteLine($"{pad}pseudo-class :{pc.Class}");
                break;
            case ChildSelector nth:                 // :nth-child(2n+1) etc.
                Console.WriteLine($"{pad}nth {nth.Text} step={nth.Step} offset={nth.Offset}");
                break;
            case IAttrSelector attr:                // every [attr...] form
                Console.WriteLine($"{pad}attr [{attr.Attribute}] value='{attr.Value}' " +
                                  $"kind={attr.GetType().Name}");
                break;
            case UnknownSelector:
                Console.WriteLine($"{pad}unparsable selector");
                break;
            default:
                Console.WriteLine($"{pad}{selector.GetType().Name} {selector.Text}");
                break;
        }
    }

    var parsed = parser.ParseSelector("ul li > a.link[href^='https']:hover, #main *");
    Describe(parsed);
    Console.WriteLine($"specificity {parsed.Specificity}");

    // Build the same kinds of selector by hand
    var byHand = new CompoundSelector();
    byHand.Add(TypeSelector.Create("a"));
    byHand.Add(ClassSelector.Create("link"));
    byHand.Add(new AttrBeginsSelector("href", "https"));
    Console.WriteLine(byHand.Text);          // a.link[href^="https"]
    Console.WriteLine(byHand.Specificity);   // (0, 0, 2, 1)

    // Find every rule whose selector targets a given class
    var sheet = parser.Parse("a.link { color: red } .link span { color: blue }");
    var hits = sheet.StyleRules.Where(r =>
        r.Selector is ClassSelector { Class: "link" } ||
        (r.Selector is CompoundSelector cs && cs.Any(s => s is ClassSelector { Class: "link" })));


Example 10: Turning Declaration Strings into Typed Values
---------------------------------------------------------
    using System;
    using System.Globalization;
    using System.Linq;
    using CodeBrix.StyleSheetParse;

    var parser = new StylesheetParser();
    var sheet = parser.Parse(@"
        .box {
            width: 50%;
            border-top-width: 2pt;
            color: #5a5eed;
            background-color: rebeccapurple;
            transition-duration: 250ms;
            transform: rotate(45deg);
        }
    ");

    var style = sheet.StyleRules.First().Style;

    // Lengths: the accessor is a string; Length.TryParse makes it typed.
    if (Length.TryParse(style.Width, out var width))
        Console.WriteLine($"{width.Value}{width.UnitString} relative={width.IsRelative}");

    if (Length.TryParse(style.BorderTopWidth, out var border) && border.IsAbsolute)
        Console.WriteLine($"{border.ToPixel()} px  ({border.To(Length.Unit.Cm)} cm)");

    // Colors: values come back normalized as rgb()/rgba(), and the original
    // author text is on the Property object.
    Console.WriteLine(style.Color);                       // rgb(90, 94, 237)
    var colorProperty = style.Declarations.First(p => p.Name == PropertyNames.Color);
    Console.WriteLine(colorProperty.Original);            // #5a5eed

    if (Color.TryFromHex(colorProperty.Original.TrimStart('#'), out var parsedColor))
        Console.WriteLine($"R={parsedColor.R} G={parsedColor.G} B={parsedColor.B}");

    var named = Colors.GetColor("rebeccapurple");          // Color?
    if (named.HasValue)
        Console.WriteLine($"{named.Value} -> {Colors.GetName(named.Value)}");

    // Times: Time has GetUnit but NO TryParse, so split the text yourself.
    var durationText = style.TransitionDuration;           // "250ms"
    var unitText = durationText.TrimStart('0','1','2','3','4','5','6','7','8',
                                          '9','.','-','+');
    var numberText = durationText.Substring(0, durationText.Length - unitText.Length);
    var duration = new Time(float.Parse(numberText, CultureInfo.InvariantCulture),
                            Time.GetUnit(unitText));
    Console.WriteLine(duration.ToMilliseconds());          // 250

    if (Angle.TryParse("45deg", out var angle))
        Console.WriteLine($"{angle.ToRadian()} rad, {angle.ToTurns()} turns");

    // Build typed values for your own model - the parser never returns these.
    var shadow = new Shadow(inset: false,
                            offsetX: new Length(2f, Length.Unit.Px),
                            offsetY: new Length(2f, Length.Unit.Px),
                            blurRadius: new Length(4f, Length.Unit.Px),
                            spreadRadius: Length.Zero,
                            color: Color.FromRgba(0, 0, 0, 0.5f));

    var gradient = new LinearGradient(
        Angle.Quarter,
        new[]
        {
            new GradientStop(Color.Red, Length.Zero),
            new GradientStop(Color.Blue, Length.Full)
        });

    var easing = new CubicBezierTimingFunction(0.25f, 0.1f, 0.25f, 1f);
    Console.WriteLine($"{shadow.BlurRadius} {gradient.Stops.Count()} {easing.X1}");


Example 11: Error Handling and Source Positions
-----------------------------------------------
    using System;
    using System.IO;
    using System.Linq;
    using CodeBrix.StyleSheetParse;

    var parser = new StylesheetParser();
    var sheet = parser.Parse(@"h1 { color: red }
                               @media screen { p { margin: 0 } }");

    // Where did each rule come from? (StylesheetText can be null on nodes
    // that were built by hand rather than parsed.)
    foreach (var rule in sheet.Children.OfType<IRule>())
    {
        var text = rule.StylesheetText;
        if (text == null)
        {
            Console.WriteLine($"{rule.Type}: no source information");
            continue;
        }

        var start = text.Range.Start;
        Console.WriteLine($"{rule.Type} at line {start.Line}, column {start.Column} " +
                          $"(offset {start.Position})");
        Console.WriteLine($"    source: {text.Text}");
    }

    // Mutation APIs throw ParseException - always guard them.
    try
    {
        sheet.Insert("@charset \"utf-8\";", 0);  // "Cannot insert Charset rule"
    }
    catch (ParseException ex)
    {
        Console.WriteLine($"insert failed: {ex.Message}");
    }

    try
    {
        sheet.RemoveAt(99);                     // "Invalid index"
    }
    catch (ParseException ex)
    {
        Console.WriteLine($"remove failed: {ex.Message}");
    }

    try
    {
        var media = sheet.MediaRules.First();
        media.Media.Remove("tv");    // "Media list element not found"
    }
    catch (ParseException ex)
    {
        Console.WriteLine($"media remove failed: {ex.Message}");
    }

    // Bad CSS is dropped, not reported: detect it by comparing what you
    // got with what you expected.
    const string expected = "h1";
    var partial = parser.Parse("h1 { color: red } p { color: blue }");
    if (!partial.StyleRules.Any(r => r.SelectorText == expected))
        Console.WriteLine($"{expected} did not survive parsing");

    // A style rule whose selector cannot be parsed disappears entirely
    // unless the parser tolerates invalid values, in which case it survives
    // carrying an UnknownSelector.
    var suspectCss = File.ReadAllText("suspect.css");
    var strict  = new StylesheetParser();
    var lenient = new StylesheetParser(tolerateInvalidValues: true);
    var strictCount  = strict.Parse(suspectCss).StyleRules.Count();
    var lenientCount = lenient.Parse(suspectCss).StyleRules.Count();
    Console.WriteLine($"{lenientCount - strictCount} rule(s) had bad selectors");

    // ParseSelector returns null instead of throwing.
    var bad = parser.ParseSelector("div >>");
    Console.WriteLine(bad == null ? "invalid selector" : bad.Text);


Example 12: Reading and Editing @media and @supports
----------------------------------------------------
    using System;
    using System.Linq;
    using CodeBrix.StyleSheetParse;

    var parser = new StylesheetParser();
    var sheet = parser.Parse(@"
        @media only screen and (min-width: 600px), print {
            .card { padding: 1em }
        }
        @supports (display: grid) and (gap: 1rem) {
            .grid { display: grid }
        }
    ");

    foreach (var media in sheet.MediaRules)
    {
        Console.WriteLine($"condition: {media.ConditionText}");
        Console.WriteLine($"media text: {media.Media.MediaText}");

        foreach (var medium in media.Media.Media)
        {
            Console.WriteLine($"  type={medium.Type} only={medium.IsExclusive} " +
                              $"not={medium.IsInverse}");
            foreach (var feature in medium.Features)
                Console.WriteLine($"    {feature.Name}" +
                                  (feature.HasValue ? $" = {feature.Value}" : "") +
                                  $" min={feature.IsMinimum} max={feature.IsMaximum}");
        }

        // Grouping rules can be edited in place
        media.Insert(".card { margin: 0 }", media.Rules.Length);
        media.Media.Add("tv");
        Console.WriteLine(media.Media.MediaText);
    }

    foreach (var supports in sheet.Children.OfType<ISupportsRule>())
    {
        Console.WriteLine($"@supports {supports.ConditionText}");
        // Check() means "can THIS library parse the declarations", not
        // "does a browser support them".
        Console.WriteLine($"  parseable: {supports.Condition.Check()}");
        foreach (IRule inner in supports.Rules)
            Console.WriteLine($"  {inner.Text}");
    }


================================================================================

MINIMUM VIABLE PROJECT
======================

MyCssTool.csproj
----------------
    <Project Sdk="Microsoft.NET.Sdk">

      <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net10.0</TargetFramework>
        <Nullable>disable</Nullable>
        <ImplicitUsings>enable</ImplicitUsings>
      </PropertyGroup>

      <ItemGroup>
        <PackageReference Include="CodeBrix.StyleSheetParse.MitLicenseForever" />
      </ItemGroup>

    </Project>

(Add the version attribute your repository's convention requires, or let
central package management supply it.)

Program.cs
----------
    using System;
    using System.IO;
    using System.Linq;
    using CodeBrix.StyleSheetParse;

    var css = args.Length > 0
        ? File.ReadAllText(args[0])
        : "h1 { color: red; margin: 5px } @media print { h1 { color: black } }";

    var parser = new StylesheetParser();
    var sheet = parser.Parse(css);

    foreach (var rule in sheet.StyleRules)
    {
        Console.WriteLine(rule.SelectorText + "  " + rule.Selector.Specificity);
        foreach (var declaration in rule.Style.Declarations)
        {
            Console.Write($"    {declaration.Name}: {declaration.Value}");
            Console.WriteLine(declaration.IsImportant ? " !important" : "");
        }
    }

    // Build a rule from scratch and serialize it
    var newRule = new StyleRule(parser);
    newRule.SelectorText = "h2";
    newRule.Style.BackgroundColor = "green";
    Console.WriteLine(newRule.ToCss());   // h2 { background-color: rgb(0, 128, 0) }

    Console.WriteLine("--- round trip ---");
    Console.WriteLine(sheet.ToCss());

Expected output for the built-in CSS: the h1 rule lists margin-top,
margin-right, margin-bottom and margin-left (the shorthand is expanded) and
color as "rgb(255, 0, 0)".


================================================================================

PERFORMANCE TIPS
================

1. Reuse StylesheetParser instances. The parser keeps no state between parse
   calls, so one instance can serve the whole process.

2. Use ParseAsync with a CancellationToken for large stylesheets to allow
   timeout/cancellation.

3. Use Stream-based parsing for large CSS files instead of loading the entire
   string into memory first.

4. Only enable the tolerance options you need. Each one keeps nodes that would
   otherwise be discarded.

5. Only enable preserveComments if you need comments retained; they add nodes
   to the tree (and you cannot read their text anyway - see the pitfalls).

6. Use ToCss with a TextWriter for large output instead of building strings in
   memory.

7. Prefer the typed rule collections (StyleRules, MediaRules, ...) over
   Children.OfType<IRule>() plus type tests when the rule kind you want has
   one; each typed collection is a single filtered pass.

8. Cache the results of the Stylesheet collections and of Style.Declarations.
   They are LINQ queries over Children, re-evaluated on every enumeration -
   calling Count() twice walks the tree twice.

9. Use the string indexer (rule.Style["color"]) or a PropertyNames constant
   for lookups; the named accessors do exactly the same work.

10. Length/Angle/Color and the other value types are structs or small sealed
    classes with no allocation-heavy parsing; TryParse in a tight loop is
    cheap. Avoid re-parsing the same declaration text repeatedly all the same.


================================================================================

COMMON PITFALLS TO AVOID
========================

1. DO NOT confuse the NuGet package name with the namespace.
   - Package: CodeBrix.StyleSheetParse.MitLicenseForever
     Namespace: CodeBrix.StyleSheetParse

2. DO NOT use ExCSS namespaces, and do not assume an ExCSS API exists here.
   Many types that look public in the upstream project are internal in this
   fork: Keywords, Lexer, Comment, RuleList, every concrete Property class,
   every concrete MediaFeature class, every ITransform implementation, and all
   rule classes except Rule, StyleRule, MarginStyleRule and CharsetRule.

3. DO NOT target .NET versions below 10.0.

4. DO NOT expect Parse() to return a list of rules. It returns a Stylesheet;
   reach rules through the typed collections or Children.OfType<IRule>().
   Stylesheet.Rules is internal and the Stylesheet constructor is internal -
   to create an empty sheet, parse an empty string.

5. DO NOT assume invalid CSS throws. By default invalid rules, selectors and
   values are silently dropped, and there is no public error event: the Lexer
   class that raises TokenizerError is internal. Detect problems by comparing
   what you got with what you expected.

6. DO NOT confuse Property.Value with Property.Original. Value is normalized
   ("#5a5eed" becomes "rgb(90, 94, 237)"); Original is the raw source text.

7. DO NOT expect the named style accessors to be typed. rule.Style.Color is a
   string, not a Color; rule.Style.Width is a string, not a Length. Use
   Length.TryParse / Color.TryFromHex / Colors.GetColor yourself.

8. DO NOT look for a shorthand in Declarations after parsing. "margin: 10px"
   is stored as four longhand declarations; only the read-back accessors
   (style["margin"], style.Margin) re-assemble it, and only in strict mode
   (that is, when includeUnknownDeclarations is left false).

9. DO NOT pass "!important" to SetProperty or SetPropertyPriority. Use the
   bare word "important" (case-insensitive), or null for normal priority.

10. DO NOT assume SetProperty succeeded. It returns void and silently does
    nothing when the value fails to parse, when the name is unknown in strict
    mode, or when priority is neither null nor "important". Read the value
    back if it matters. SetProperty(name, null) removes the property.

11. DO NOT call style.Clear() expecting to empty the block: Clear is the CSS
    "clear" property and hides StylesheetNode.Clear(). Use
    style.CssText = string.Empty.

12. DO NOT ignore the null from ParseSelector. It returns null for invalid
    input unless the parser was created with tolerateInvalidSelectors: true
    (in which case you get an UnknownSelector).

13. DO NOT confuse the two selector tolerance options. Inside a stylesheet, a
    style rule whose selector fails to parse is dropped unless
    tolerateInvalidValues is true; tolerateInvalidSelectors governs
    ParseSelector and non-standard pseudo-elements.

14. DO NOT try to instantiate AttributeSelectorFactory,
    PseudoClassSelectorFactory or PseudoElementSelectorFactory. The classes
    are public but their constructors and Instance accessors are internal.
    Use the static Create methods on the selector classes instead.

15. DO NOT pass a leading '#' to Color.FromHex or Color.TryFromHex - they
    expect 3, 4, 6 or 8 bare hex digits. FromHex does not validate; use
    TryFromHex on untrusted input.

16. DO NOT call Length.ToPixel() or Length.To() on a relative unit (em, rem,
    %, vw, ...). They throw InvalidOperationException. Check IsAbsolute first.

17. DO NOT expect preserveComments to give you readable comments. The comment
    node class is internal, so the text is not reachable from consumer code;
    the option only affects what stays in the tree.

18. DO NOT forget that the specificity operators exist. Compare Priority
    values directly with <, >, ==; do not compare the ToString() output.

19. DO NOT forget IsImportant when reading declarations - the !important flag
    is separate from the value, on Property/IProperty.

20. DO NOT expect a typed value object from the parser. Color, Length, Shadow,
    LinearGradient, TransformMatrix, CubicBezierTimingFunction and friends are
    types you construct; no public API returns one.

21. DO NOT copy test code from this repository verbatim. The test project has
    InternalsVisibleTo access and uses internal members (sheet.Rules,
    MediaRule, SupportsRule, DocumentRule, KeyframeRule, MediaFeatureFactory,
    ParseDeclaration, Keywords, ...). Translate those to the public
    equivalents shown in this file.


================================================================================

WHAT THIS PACKAGE DOES NOT DO
=============================

Do NOT attempt to use this library for the following:

  - Rendering CSS or applying styles to elements (this is a parser and object
    model only - it does not render anything)
  - Matching selectors against a document. ISelector has no Match method and
    there is no DOM here; selectors only carry text, type and specificity
  - Computing cascaded or inherited styles for an element
  - Validating CSS against a specific CSS specification version. @supports
    Condition.Check() only reports whether this library can parse the
    declaration, and says nothing about any browser
  - Minifying or prettifying CSS. The single built-in formatter,
    CompressedStyleFormatter, emits readable spacing; implement IStyleFormatter
    for anything else
  - Resolving @import rules (the URL is parsed but never fetched)
  - Evaluating media or container queries against a device context
  - Reading CSS comments back out (see pitfall 17)
  - Reporting parse errors to your code (see pitfall 5)
  - Sass/SCSS/Less preprocessing
  - CSS module scoping or transformation
  - PostCSS-style plugin processing
  - CSS custom properties/variables resolution (var() is preserved as text,
    never substituted)

CodeBrix.StyleSheetParse IS for: parsing CSS text into a structured object
model, querying and manipulating that model, calculating selector specificity,
and serializing the model back to CSS text.


================================================================================

WORKING EXAMPLES ON GITHUB
==========================

The test project contains hundreds of working, compiling usages. Browse them
at:

    https://github.com/ellisnet/CodeBrix.StyleSheetParse/tree/main/tests/CodeBrix.StyleSheetParse.Tests

Feature-to-test-file mapping (paths relative to that folder):

  CSS selector parsing (attribute, class, general selectors):
    -> AttrSelectorTests.cs
    -> ClassSelectorTests.cs
    -> SelectorsTests.cs

  CSS color values:
    -> ColorTests.cs

  CSS construction helpers used by the other test files:
    -> CssConstructionFunctions.cs

  @container queries:
    -> CssContainerTests.cs

  @font-face rules:
    -> FontFaceTests.cs

  Font properties:
    -> CssFontPropertyTests.cs

  CSS gradient functions:
    -> GradientTests.cs

  @import rules:
    -> CssImportRuleTests.cs

  @keyframes rules:
    -> CssKeyframeRuleTests.cs

  List properties (list-style, etc.):
    -> CssListPropertyTests.cs

  Media features and media lists:
    -> CssMediaFeaturesTests.cs
    -> CssMediaListTests.cs

  Object sizing properties:
    -> CssObjectSizingTests.cs

  CSS properties (general):
    -> CssPropertyTests.cs

  Per-area property tests (animation, background, border, border-image,
  border-radius, box-sizing, columns, content, coordinates, outline, padding,
  transform, transition, fill, flex, gap, margin, opacity, row-gap, stroke,
  text):
    -> PropertyTests/

  Flexbox properties:
    -> Flexbox.cs
    -> PropertyTests/FlexPropertyTests.cs

  Real-world CSS parsing (e.g. bootstrap.css):
    -> RealWorld.cs
    -> CssCasesTests.cs

  Stylesheet parsing and manipulation:
    -> CssSheetTests.cs

  @supports rules:
    -> CssSupportsTests.cs

  CSS tokenization:
    -> CssTokenizationTests.cs

  @document function:
    -> CssDocumentFunctionTests.cs

HOW TO USE: fetch the raw file content from GitHub with a URL like

    https://raw.githubusercontent.com/ellisnet/CodeBrix.StyleSheetParse/main/tests/CodeBrix.StyleSheetParse.Tests/CssSheetTests.cs

REMEMBER (pitfall 21): the tests can see internal members. Anything in them
that names sheet.Rules, MediaRule, SupportsRule, DocumentRule, KeyframeRule,
Keywords, MediaFeatureFactory or parser.ParseDeclaration will NOT compile in
consumer code - use the public equivalents documented above.


================================================================================

QUICK REFERENCE CARD
====================

--- Install ---
dotnet add package CodeBrix.StyleSheetParse.MitLicenseForever

--- Namespace ---
using CodeBrix.StyleSheetParse;

--- Parse ---
Parse CSS:          var sheet = new StylesheetParser().Parse(cssText);
Parse stream:       var sheet = parser.Parse(stream);
Parse async:        var sheet = await parser.ParseAsync(cssText);
Parse async cancel: var sheet = await parser.ParseAsync(cssText, token);
Parse selector:     var sel = parser.ParseSelector("div > p.class");  // may be null
Empty sheet:        var sheet = parser.Parse(string.Empty);

--- Access Rules ---
Style rules:        sheet.StyleRules
Media rules:        sheet.MediaRules
Container rules:    sheet.ContainerRules
Font-face rules:    sheet.FontfaceSetRules
Import rules:       sheet.ImportRules
Namespace rules:    sheet.NamespaceRules
Page rules:         sheet.PageRules
Charset rules:      sheet.CharacterSetRules
Everything else:    sheet.Children.OfType<IRule>()
@keyframes:         sheet.Children.OfType<IKeyframesRule>()
@supports:          sheet.Children.OfType<ISupportsRule>()
@document:          rules with Type == RuleType.Document, then
                    rule.Children.OfType<IDocumentFunction>()
@page margin boxes: page.Children.OfType<MarginStyleRule>()

--- Style Rule ---
Selector text:      rule.SelectorText
Parsed selector:    rule.Selector
Specificity:        rule.Selector.Specificity      // Priority(a,b,c,d)
Declarations:       rule.Style
Source position:    rule.StylesheetText?.Range.Start.Line

--- Style Declarations ---
Get value:          rule.Style["color"] / rule.Style.GetPropertyValue("color")
Get priority:       rule.Style.GetPropertyPriority("color")
Set property:       rule.Style.SetProperty("color", "red")
Set with priority:  rule.Style.SetProperty("color", "red", "important")
Remove property:    rule.Style.RemoveProperty("color")
All declarations:   rule.Style.Declarations        // IEnumerable<Property>
Count:              rule.Style.Length
Named accessors:    rule.Style.Color, rule.Style.FontSize, ...  // all string
Raw author text:    property.Original
Empty the block:    rule.Style.CssText = string.Empty

--- Selector Types ---
Simple:             TypeSelector, ClassSelector, IdSelector, AllSelector,
                    PseudoClassSelector, PseudoElementSelector,
                    NamespaceSelector
Attribute:          AttrMatch/List/Hyphen/Begins/Ends/Contains/NotMatch/
                    Available Selector (all IAttrSelector)
nth-*:              FirstChildSelector, LastChildSelector, FirstTypeSelector,
                    LastTypeSelector, FirstColumnSelector, LastColumnSelector
Combining:          CompoundSelector (div.foo), ComplexSelector (div > p),
                    ListSelector (h1, h2), UnknownSelector (invalid)
Build one:          ClassSelector.Create("foo"), TypeSelector.Create("div"),
                    IdSelector.Create("main"), AllSelector.Create(),
                    new AttrMatchSelector("type", "text")

--- Typed Values (you construct / parse these) ---
Length:             Length.TryParse("2em", out var l); l.ToPixel() (absolute only)
Angle:              Angle.TryParse("45deg", out var a); a.ToRadian()
Time:               Time.GetUnit("ms"); new Time(250f, Time.Unit.Ms)
Frequency:          Frequency.TryParse("44khz", out var f); f.ToHertz()
Resolution:         Resolution.TryParse("2dppx", out var r); r.ToDotsPerPixel()
Color:              Color.TryFromHex("5a5eed", out var c);  // no '#'
                    Color.FromRgba(0,0,0,0.5f); Colors.GetColor("red")
Others:             Percent, Number, Point, Shadow, Shape, Counter,
                    GradientStop, LinearGradient, RadialGradient,
                    TransformMatrix, CubicBezierTimingFunction,
                    StepsTimingFunction, Url

--- Media / Conditions ---
Condition text:     mediaRule.ConditionText
Nested rules:       mediaRule.Rules          // IRuleList
Media list:         mediaRule.Media          // MediaList
Individual media:   mediaRule.Media.Media    // IEnumerable<Medium>
Features:           medium.Features          // IEnumerable<MediaFeature>
Supports check:     supportsRule.Condition.Check()   // parseable, not browser

--- Serialize ---
To string:          sheet.ToCss()
With formatter:     sheet.ToCss(CompressedStyleFormatter.Instance)
To writer:          sheet.ToCss(writer)
Custom output:      implement IStyleFormatter

--- Constants ---
PropertyNames, RuleNames, FunctionNames, PseudoClassNames,
PseudoElementNames, FeatureNames, UnitNames, Combinators, Colors,
ProtocolNames, TextEncoding      (there is NO public Keywords class)

--- Errors ---
Thrown:             ParseException (mutation + re-parse paths)
Never thrown:       parsing malformed CSS - bad input is dropped silently
Diagnostics:        node.StylesheetText?.Range (TextRange/TextPosition)
Vocabulary only:    ParseError, TokenizerError (no public error event)

--- Parser Options (all default false) ---
includeUnknownRules, includeUnknownDeclarations, tolerateInvalidSelectors,
tolerateInvalidValues, tolerateInvalidConstraints, preserveComments,
preserveDuplicateProperties

Target: .NET 10 or later
License: MIT


================================================================================

END OF AGENT-README
