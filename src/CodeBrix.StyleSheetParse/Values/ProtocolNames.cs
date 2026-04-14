namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>The protocol names member.</summary>
public static class ProtocolNames
{
    /// <summary>Gets the http value.</summary>
    public static readonly string Http = "http";
    /// <summary>Gets the https value.</summary>
    public static readonly string Https = "https";
    /// <summary>Gets the ftp value.</summary>
    public static readonly string Ftp = "ftp";
    /// <summary>Gets the java script value.</summary>
    public static readonly string JavaScript = "javascript";
    /// <summary>Gets the data value.</summary>
    public static readonly string Data = "data";
    /// <summary>Gets the mailto value.</summary>
    public static readonly string Mailto = "mailto";
    /// <summary>Gets the file value.</summary>
    public static readonly string File = "file";
    /// <summary>Gets the ws value.</summary>
    public static readonly string Ws = "ws";
    /// <summary>Gets the wss value.</summary>
    public static readonly string Wss = "wss";
    /// <summary>Gets the telnet value.</summary>
    public static readonly string Telnet = "telnet";
    /// <summary>Gets the ssh value.</summary>
    public static readonly string Ssh = "ssh";
    /// <summary>Gets the gopher value.</summary>
    public static readonly string Gopher = "gopher";
    /// <summary>Gets the blob value.</summary>
    public static readonly string Blob = "blob";

    private static readonly string[] RelativeProtocols =
    {
        Http,
        Https,
        Ftp,
        File,
        Ws,
        Wss,
        Gopher
    };

    private static readonly string[] OriginalableProtocols =
    {
        Http,
        Https,
        Ftp,
        Ws,
        Wss,
        Gopher
    };

    /// <summary>Performs the is relative operation.</summary>
    public static bool IsRelative(string protocol)
    {
        return RelativeProtocols.Contains(protocol);
    }

    /// <summary>Performs the is originable operation.</summary>
    public static bool IsOriginable(string protocol)
    {
        return OriginalableProtocols.Contains(protocol);
    }
}
