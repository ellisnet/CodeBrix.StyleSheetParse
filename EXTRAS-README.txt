================================================================================
EXTRAS-README: CodeBrix.StyleSheetParse
Samples, tools and other content in this repository that is not part of a
NuGet package
================================================================================

This repository contains no sample applications, demo projects, benchmarks or
build tools. It has exactly two projects: the library that becomes the
CodeBrix.StyleSheetParse.MitLicenseForever package, and its test project.

The only non-package content is therefore the test project.


TEST PROJECT
============
Path: tests/CodeBrix.StyleSheetParse.Tests

An xUnit v3 test project covering the tokenizer, the rule and declaration
model, the selector model, media queries, at-rules, CSS value parsing and
serialization - roughly a thousand test cases. It is not packed and not
published.

Run it with:

    dotnet test CodeBrix.StyleSheetParse.slnx

The tests double as the largest body of working usage examples for the
library, and AGENT-README.txt links to them from its "WORKING EXAMPLES ON
GITHUB" section. Two cautions apply when reading them as examples:

  -> the test project has InternalsVisibleTo access, so much of what it calls
     (Stylesheet.Rules, MediaRule, SupportsRule, DocumentRule, KeyframeRule,
     Keywords, MediaFeatureFactory, parser.ParseDeclaration) is NOT available
     to package consumers;
  -> CssConstructionFunctions.cs and TestExtensions.cs are shared test
     helpers, not library API.


OPTIONAL TEST DATA
==================
Path: tests/CodeBrix.StyleSheetParse.Tests/bootstrap.css

A full real-world stylesheet, compiled into the test assembly as an
EmbeddedResource and loaded by manifest resource name. The selector,
attribute-selector, class-selector and real-world tests assert exact rule
counts against it, so it must not be edited or replaced.
