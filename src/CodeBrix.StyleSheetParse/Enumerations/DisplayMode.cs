namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Specifies the display mode options.</summary>
public enum DisplayMode : byte
{
    /// <summary>The none value.</summary>
    None,
    /// <summary>The inline value.</summary>
    Inline,
    /// <summary>The block value.</summary>
    Block,
    /// <summary>The list item value.</summary>
    ListItem,
    /// <summary>The inline block value.</summary>
    InlineBlock,
    /// <summary>The inline table value.</summary>
    InlineTable,
    /// <summary>The table value.</summary>
    Table,
    /// <summary>The table caption value.</summary>
    TableCaption,
    /// <summary>The table cell value.</summary>
    TableCell,
    /// <summary>The table column value.</summary>
    TableColumn,
    /// <summary>The table column group value.</summary>
    TableColumnGroup,
    /// <summary>The table footer group value.</summary>
    TableFooterGroup,
    /// <summary>The table header group value.</summary>
    TableHeaderGroup,
    /// <summary>The table row value.</summary>
    TableRow,
    /// <summary>The table row group value.</summary>
    TableRowGroup,
    /// <summary>The flex value.</summary>
    Flex,
    /// <summary>The inline flex value.</summary>
    InlineFlex,
    /// <summary>The grid value.</summary>
    Grid,
    /// <summary>The inline grid value.</summary>
    InlineGrid
}
