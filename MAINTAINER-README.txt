================================================================================
MAINTAINER-README: CodeBrix.StyleSheetParse
Notes for people and agents MAINTAINING this repository — not for package
consumers
================================================================================

If you are consuming the NuGet package, stop reading and open AGENT-README.txt
instead. Everything below is about the repository itself.


PURPOSE AND SCOPE
=================
This repository produces exactly one NuGet package:

    PackageId   CodeBrix.StyleSheetParse.MitLicenseForever
    Assembly    CodeBrix.StyleSheetParse
    Namespace   CodeBrix.StyleSheetParse
    Project     src/CodeBrix.StyleSheetParse/CodeBrix.StyleSheetParse.csproj
    Consumer docs  AGENT-README.txt (repo root) - this is the file that ships
                   inside the .nupkg

The library is a CSS parser and object model: tokenizer, rule/declaration
model, selector model, typed CSS value types, and a pluggable serializer.


REPOSITORY LAYOUT
=================
    AGENT-README.txt         Consumer documentation (packed into the nupkg)
    MAINTAINER-README.txt    This file
    EXTRAS-README.txt        Non-package content in the repo
    README-INDEX.txt         Map of the README files
    README.md                Human-facing overview (GitHub + nuget.org)
    LICENSE                  MIT
    THIRD-PARTY-NOTICES.txt  ExCSS notice (packed into the nupkg)
    icon-codebrix-128.png    Package icon (packed into the nupkg)
    CodeBrix.StyleSheetParse.slnx   Solution
    AGENTS.md, CLAUDE.md, .clinerules, .cursorrules, .windsurfrules,
    .cursor/rules/agent-readme.mdc, .github/copilot-instructions.md,
    .junie/guidelines.md            AI-agent pointer stubs - these are
                                    reconciled centrally against the canonical
                                    CodeBrix.SkiaSvg copies; do not hand-edit
                                    them in this repo
    src/CodeBrix.StyleSheetParse/   The library
    tests/CodeBrix.StyleSheetParse.Tests/   xUnit v3 test project

Library source folders (namespace is flat - CodeBrix.StyleSheetParse - for
every folder except Conditions/, which is internal):

    Conditions/       @supports condition tree (And/Or/Not/Group/Declaration/
                      Empty) - all internal
    Enumerations/     Public CSS enums plus the string-constant classes
                      (PropertyNames, RuleNames, FunctionNames,
                      PseudoClassNames, PseudoElementNames, FeatureNames,
                      UnitNames, Colors, Combinators) and the internal
                      Keywords / PropertyFlags / QuirksMode / TokenType
    Extensions/       FormatExtensions (public) plus internal string, char,
                      collection, value and converter extensions
    Factories/        Selector and property factories (public classes, but
                      their instances are internal to the parser)
    Formatting/       CompressedStyleFormatter - the only IStyleFormatter
    Functions/        @document functions and the IConditionFunction contract
    MediaFeatures/    MediaFeature base (public) plus 20 internal features
    Model/            Stylesheet, StylesheetNode, StyleDeclaration, MediaList,
                      Medium, Priority, Url, TransformMatrix, TextSource,
                      TextRange/TextPosition, ParseException, the core
                      interfaces, and the internal Pool/Symbols/Map plumbing
    Parser/           StylesheetParser (public), Lexer/LexerBase,
                      StylesheetComposer, SelectorConstructor, TokenizerError
    Rules/            Rule interfaces (public) and rule classes (only Rule,
                      StyleRule, MarginStyleRule and CharsetRule are public)
    Selectors/        The whole selector object model (public)
    StyleProperties/  Property base + IProperty/IProperties (public) and 191
                      internal concrete property classes, in per-area
                      sub-folders
    Tokens/           Token types - all internal
    ValueConverters/  IValueConverter implementations - all internal
    Values/           Public CSS value types (Color, Length, Angle, Time,
                      Frequency, Resolution, Number, Percent, Point, Shadow,
                      Shape, Counter, gradients, timing functions) plus the
                      internal ITransform implementations


BUILDING
========
    dotnet restore CodeBrix.StyleSheetParse.slnx
    dotnet build   CodeBrix.StyleSheetParse.slnx -c Release

The library project sets GeneratePackageOnBuild=true, so EVERY build of the
library produces a fresh .nupkg in bin/<config>/. That is intentional but it
means "just building" also packs; see the versioning note below before you
publish anything.

GenerateDocumentationFile=true is on, so missing XML doc comments surface as
CS1591 warnings. Fix them at the source - never suppress the warning.


TESTING
=======
    dotnet test CodeBrix.StyleSheetParse.slnx

The test project (tests/CodeBrix.StyleSheetParse.Tests) uses xunit.v3 with
xunit.runner.visualstudio and Microsoft.NET.Test.Sdk. There are roughly a
thousand [Fact]/[Theory] cases. No opt-in environment variables, no special
prep, no network access, nothing platform-specific.

bootstrap.css is an EmbeddedResource in the test project; the selector and
real-world tests load it by manifest resource name
("CodeBrix.StyleSheetParse.Tests.bootstrap.css") and assert exact rule counts
against it. Do not edit that file - the counts in AttrSelectorTests,
ClassSelectorTests and SelectorsTests are calibrated to it.

The library ships src/CodeBrix.StyleSheetParse/InternalsVisibleTo.cs granting
internals access to CodeBrix.StyleSheetParse.Tests, and the tests use it
heavily (Stylesheet.Rules, MediaRule, SupportsRule, DocumentRule,
KeyframeRule, Keywords, MediaFeatureFactory, parser.ParseDeclaration,
parser.ParseRule, parser.ParseValue). Keep that in mind when you copy test
code into documentation: those members are NOT available to package
consumers, and AGENT-README.txt warns readers about exactly this.


PACKAGING AND PUBLISHING
========================
Packing is driven by the library csproj alone; there is no pack script.

    dotnet pack src/CodeBrix.StyleSheetParse/CodeBrix.StyleSheetParse.csproj -c Release

What ships in the .nupkg (all declared as None/Pack=true in the csproj):

    icon-codebrix-128.png        PackageIcon
    README.md                    PackageReadmeFile
    AGENT-README.txt             consumer documentation for AI agents
    THIRD-PARTY-NOTICES.txt      ExCSS attribution

If the consumer documentation is ever split into multiple AGENT-README files,
the csproj ItemGroup must be updated to pack each of them. MAINTAINER-README,
EXTRAS-README and README-INDEX are deliberately NOT packed.

Versioning: date-stamped and auto-incrementing, computed in the csproj from
System.DateTime.UtcNow as 1.<years-since-base>.<day-of-year>.<minute-of-day>,
with _VersionBaseYear as the baseline. Consequences to remember:

  -> every build produces a new version, and with GeneratePackageOnBuild that
     means a new .nupkg every time;
  -> two builds inside the same UTC minute produce the SAME version, so never
     publish twice within one minute;
  -> this is not SemVer - major is pinned and minor encodes the year, so the
     numbers say nothing about API compatibility;
  -> the full rationale and caveats live in a comment block at the top of the
     csproj. Read it before changing anything about versioning.

Package metadata (id, title, authors, description, license expression, tags,
project/repository URLs, copyright, PackageRequireLicenseAcceptance) all live
in the same csproj. The copyright line is
"Copyright (c) 2026 Jeremy Ellis and contributors".


PROVENANCE AND VENDORED SOURCES
===============================
The entire library is a fork of ExCSS v4.3.1 (https://github.com/TylerBrinks/
ExCSS), MIT licensed, Copyright (c) 2024 Tyler Brinks. The attribution text is
in THIRD-PARTY-NOTICES.txt and must stay in the package.

Every file that came from upstream carries a marker comment recording the old
namespace, for example:

    namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

There are around 300 of these markers across the source. Keep them: they are
how a future maintainer maps a file back to its upstream original. When you
port a fix from upstream, keep the marker and translate the namespace.

Divergences from upstream that are already in place and must not be undone
casually:

  -> file-scoped namespaces throughout (478 files);
  -> XML doc comments added to public members to satisfy CS1591;
  -> net10.0 only - there is no multi-targeting and none is wanted;
  -> nullable reference type annotations are OFF (the csproj does not set
     <Nullable>), matching the rest of the CodeBrix family;
  -> the visibility surface has been curated: several types that are public
     upstream are internal here. AGENT-README.txt documents that list for
     consumers. If you change a type's visibility, update AGENT-README.txt in
     the same commit.


CODING CONVENTIONS
==================
  -> Target net10.0 only. Do not add other target frameworks.
  -> File-scoped namespaces; one public type per file, named after the file.
  -> Public members need XML doc comments (GenerateDocumentationFile is on).
  -> No nullable reference annotations ("?" on reference types) in this repo.
  -> Keep types in the folder that matches their role (see REPOSITORY LAYOUT);
     the namespace stays flat regardless of folder.
  -> Prefer internal for anything the consumer does not need. The public
     surface is already documented type by type in AGENT-README.txt, so every
     visibility change is a documentation change too.
  -> Test files are named <Class>Tests.cs where they map to one type; the
     older upstream-derived names (Flexbox.cs, RealWorld.cs,
     CssConstructionFunctions.cs) are kept as they are for traceability.


NOTES
=====
  -> The AI-agent pointer stubs at the repo root point at README-INDEX.txt.
     They are maintained centrally across the CodeBrix family; do not edit
     them here.
  -> README.md is human-facing and also ships in the package; keep its sample
     code compiling against the public API.
  -> There are no GitHub workflow files in this repository, and none should be
     added.
  -> CompressedStyleFormatter is the only IStyleFormatter implementation. If a
     pretty/minifying formatter is ever added, it must be public and
     AGENT-README.txt's serialization section needs updating.
  -> IMarginRule is declared but unimplemented (margin boxes are parsed as
     MarginStyleRule, which implements IStyleRule). If that is ever
     reconciled, it is a public API change - update AGENT-README.txt.
