namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents the contract for a CSS transform node.</summary>
public interface ITransform
{
    /// <summary>The compute matrix member.</summary>
    TransformMatrix ComputeMatrix();
}
