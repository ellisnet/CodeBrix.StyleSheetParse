using System;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

internal sealed class UrlPrefixFunction : DocumentFunction
{
    public UrlPrefixFunction(string url) : base(FunctionNames.UrlPrefix, url)
    {
    }

    public override bool Matches(Url url)
    {
        return url.Href.StartsWith(Data, StringComparison.OrdinalIgnoreCase);
    }
}
