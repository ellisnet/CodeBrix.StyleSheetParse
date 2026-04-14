using Xunit;

namespace CodeBrix.StyleSheetParse.Tests.PropertyTests; //Was previously: namespace ExCSS.Tests.PropertyTests;

public class RowGapPropertyTests : CssConstructionFunctions
{
    [Theory]
    [MemberData(nameof(LengthOrPercentOrGlobalTestValues))]
    public void RowGapLegalValues(string value)
        => TestForLegalValue<RowGapProperty>(PropertyNames.RowGap, value);
}
