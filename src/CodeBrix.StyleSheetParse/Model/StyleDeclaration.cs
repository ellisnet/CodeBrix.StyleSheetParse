using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// ReSharper disable UnusedMember.Global

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS style declaration.</summary>
public sealed class StyleDeclaration : StylesheetNode, IProperties
{
    private readonly Rule _parent;
    private readonly StylesheetParser _parser;
    /// <summary>The changed member.</summary>
    public event Action<string> Changed;

    private StyleDeclaration(Rule parent, StylesheetParser parser)
    {
        _parent = parent;
        _parser = parser;
    }

    internal StyleDeclaration(StylesheetParser parser) : this(null, parser)
    {
    }

    internal StyleDeclaration() : this(null, null)
    {
    }

    internal StyleDeclaration(Rule parent) : this(parent, parent.Parser)
    {
    }

    /// <summary>Performs the update operation.</summary>
    public void Update(string value)
    {
        Clear();

        if (!string.IsNullOrEmpty(value)) _parser.AppendDeclarations(this, value);
    }

    /// <summary>Performs the to css operation.</summary>
    public override void ToCss(TextWriter writer, IStyleFormatter formatter)
    {
        var list = new List<string>();
        var serialized = new List<string>();
        foreach (var declaration in Declarations)
        {
            var property = declaration.Name;
            if (IsStrictMode)
            {
                if (serialized.Contains(property)) continue;

                var shorthands = PropertyFactory.Instance.GetShorthands(property).ToList();
                if (shorthands.Any())
                {
                    var longhands = Declarations.Where(m => !serialized.Contains(m.Name)).ToList();
                    foreach (var shorthand in shorthands.OrderByDescending(m =>
                        PropertyFactory.Instance.GetLonghands(m).Length))
                    {
                        var rule = PropertyFactory.Instance.CreateShorthand(shorthand);
                        var properties = PropertyFactory.Instance.GetLonghands(shorthand);
                        var currentLonghands = longhands.Where(m => properties.Contains(m.Name)).ToArray();

                        if (currentLonghands.Length == 0) continue;

                        var important = currentLonghands.Count(m => m.IsImportant);

                        if (important > 0 && important != currentLonghands.Length) continue;

                        if (properties.Length != currentLonghands.Length) continue;

                        var value = rule.Stringify(currentLonghands);

                        if (string.IsNullOrEmpty(value)) continue;

                        list.Add(CompressedStyleFormatter.Instance.Declaration(shorthand, value, important != 0));

                        foreach (var longhand in currentLonghands)
                        {
                            serialized.Add(longhand.Name);
                            longhands.Remove(longhand);
                        }
                    }
                }

                if (serialized.Contains(property)) continue;
                serialized.Add(property);
            }

            list.Add(declaration.ToCss(formatter));
        }

        writer.Write(formatter.Declarations(list));
    }

    /// <summary>Performs the remove property operation.</summary>
    public string RemoveProperty(string propertyName)
    {
        var value = GetPropertyValue(propertyName);
        RemovePropertyByName(propertyName);
        RaiseChanged();

        return value;
    }

    private void RemovePropertyByName(string propertyName)
    {
        foreach (var declaration in Declarations)
        {
            if (!declaration.Name.Is(propertyName)) continue;
            RemoveChild(declaration);
            break;
        }

        if (!IsStrictMode || !PropertyFactory.Instance.IsShorthand(propertyName)) return;

        var longhands = PropertyFactory.Instance.GetLonghands(propertyName);
        foreach (var longhand in longhands) RemovePropertyByName(longhand);
    }

    /// <summary>Performs the get property priority operation.</summary>
    public string GetPropertyPriority(string propertyName)
    {
        var property = GetProperty(propertyName);
        if (property is {IsImportant: true}) return Keywords.Important;
        if (!IsStrictMode || !PropertyFactory.Instance.IsShorthand(propertyName)) return string.Empty;

        var longhands = PropertyFactory.Instance.GetLonghands(propertyName);

        return longhands.Any(longhand => !GetPropertyPriority(longhand)
            .Isi(Keywords.Important))
            ? string.Empty
            : Keywords.Important;
    }

    /// <summary>Performs the get property value operation.</summary>
    public string GetPropertyValue(string propertyName)
    {
        var property = GetProperty(propertyName);
        if (property != null) return property.Value;

        if (!IsStrictMode || !PropertyFactory.Instance.IsShorthand(propertyName)) return string.Empty;

        var shortHand = PropertyFactory.Instance.CreateShorthand(propertyName);
        var declarations = PropertyFactory.Instance.GetLonghands(propertyName);
        var properties = new List<Property>();

        foreach (var declaration in declarations)
        {
            property = GetProperty(declaration);
            if (property == null) return string.Empty;
            properties.Add(property);
        }

        return shortHand.Stringify(properties.ToArray());
    }

    /// <summary>Performs the set property value operation.</summary>
    public void SetPropertyValue(string propertyName, string propertyValue)
    {
        SetProperty(propertyName, propertyValue);
    }

    /// <summary>Performs the set property priority operation.</summary>
    public void SetPropertyPriority(string propertyName, string priority)
    {
        if (!string.IsNullOrEmpty(priority) && !priority.Isi(Keywords.Important)) return;

        var important = !string.IsNullOrEmpty(priority);
        var mappings = IsStrictMode && PropertyFactory.Instance.IsShorthand(propertyName)
            ? PropertyFactory.Instance.GetLonghands(propertyName)
            : Enumerable.Repeat(propertyName, 1);

        foreach (var mapping in mappings)
        {
            var property = GetProperty(mapping);
            if (property != null) property.IsImportant = important;
        }
    }

    /// <summary>Performs the set property operation.</summary>
    public void SetProperty(string propertyName, string propertyValue, string priority = null)
    {
        if (!string.IsNullOrEmpty(propertyValue))
        {
            if (priority != null && !priority.Isi(Keywords.Important)) return;

            var value = _parser.ParseValue(propertyValue);
            if (value == null) return;

            var property = CreateProperty(propertyName);
            if (property == null || !property.TrySetValue(value)) return;

            property.IsImportant = priority != null;
            SetProperty(property);
            RaiseChanged();
        }
        else
        {
            RemoveProperty(propertyName);
        }
    }

    internal Property CreateProperty(string propertyName)
    {
        var property = GetProperty(propertyName);
        if (property != null) return property;

        property = PropertyFactory.Instance.Create(propertyName);
        if (property != null || IsStrictMode) return property;

        return new UnknownProperty(propertyName);
    }

    internal Property GetProperty(string name)
    {
        return Declarations.FirstOrDefault(m => m.Name.Isi(name));
    }

    internal void SetProperty(Property property)
    {
        if (property is ShorthandProperty shorthand)
        {
            SetShorthand(shorthand);
        }
        else
        {
            SetLonghand(property);
        }
    }

    internal void SetDeclarations(IEnumerable<Property> declarations)
    {
        ChangeDeclarations(declarations, _ => false, (o, n) => !o.IsImportant || n.IsImportant);
    }

    internal void UpdateDeclarations(IEnumerable<Property> declarations)
    {
        ChangeDeclarations(declarations, m => !m.CanBeInherited, (o, _) => o.IsInherited);
    }

    private void ChangeDeclarations(IEnumerable<Property> declarations, Predicate<Property> defaultSkip,
        Func<Property, Property, bool> removeExisting)
    {
        var propertyList = new List<Property>();
        foreach (var newDeclaration in declarations)
        {
            var skip = defaultSkip(newDeclaration);
            foreach (var oldDeclaration in Declarations)
            {
                if (!oldDeclaration.Name.Is(newDeclaration.Name)) continue;

                if (removeExisting(oldDeclaration, newDeclaration))
                    RemoveChild(oldDeclaration);
                else
                    skip = true;
                break;
            }

            if (!skip) propertyList.Add(newDeclaration);
        }

        foreach (var declaration in propertyList) AppendChild(declaration);
    }

    private void SetLonghand(Property property)
    {
        if (!_parser.Options.PreserveDuplicateProperties)
        {
            foreach (var declaration in Declarations)
            {
                if (!declaration.Name.Is(property.Name)) continue;
                RemoveChild(declaration);
                break;
            }
        }

        AppendChild(property);
    }

    private void SetShorthand(ShorthandProperty shorthand)
    {
        var properties = PropertyFactory.Instance.CreateLonghandsFor(shorthand.Name);
        shorthand.Export(properties);

        foreach (var property in properties) SetLonghand(property);
    }

    private void RaiseChanged()
    {
        Changed?.Invoke(CssText);
    }

    /// <summary>Performs the get enumerator operation.</summary>
    public IEnumerator<IProperty> GetEnumerator()
    {
        return Declarations.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>Gets the parent.</summary>
    public IRule Parent => _parent;
    /// <summary>The this member.</summary>
    public string this[int index] => Declarations.GetItemByIndex(index).Name;
    /// <summary>The this member.</summary>
    public string this[string name] => GetPropertyValue(name);
    /// <summary>Gets the length.</summary>
    public int Length => Declarations.Count();

    /// <summary>Gets the is strict mode.</summary>
    public bool IsStrictMode => /* IsReadOnly ||*/ _parser.Options.IncludeUnknownDeclarations == false;

    /// <summary>Gets the declarations.</summary>
    public IEnumerable<Property> Declarations => Children.OfType<Property>();

    /// <summary>The css text member.</summary>
    public string CssText
    {
        get => this.ToCss();
        set
        {
            Update(value);
            RaiseChanged();
        }
    }

    /// <summary>The align content member.</summary>
    public string AlignContent
    {
        get => GetPropertyValue(PropertyNames.AlignContent);
        set => SetPropertyValue(PropertyNames.AlignContent, value);
    }

    /// <summary>The align items member.</summary>
    public string AlignItems
    {
        get => GetPropertyValue(PropertyNames.AlignItems);
        set => SetPropertyValue(PropertyNames.AlignItems, value);
    }

    /// <summary>The align self member.</summary>
    public string AlignSelf
    {
        get => GetPropertyValue(PropertyNames.AlignSelf);
        set => SetPropertyValue(PropertyNames.AlignSelf, value);
    }

    /// <summary>The accelerator member.</summary>
    public string Accelerator
    {
        get => GetPropertyValue(PropertyNames.Accelerator);
        set => SetPropertyValue(PropertyNames.Accelerator, value);
    }

    /// <summary>The alignment baseline member.</summary>
    public string AlignmentBaseline
    {
        get => GetPropertyValue(PropertyNames.AlignBaseline);
        set => SetPropertyValue(PropertyNames.AlignBaseline, value);
    }

    /// <summary>The animation member.</summary>
    public string Animation
    {
        get => GetPropertyValue(PropertyNames.Animation);
        set => SetPropertyValue(PropertyNames.Animation, value);
    }

    /// <summary>The animation delay member.</summary>
    public string AnimationDelay
    {
        get => GetPropertyValue(PropertyNames.AnimationDelay);
        set => SetPropertyValue(PropertyNames.AnimationDelay, value);
    }

    /// <summary>The animation direction member.</summary>
    public string AnimationDirection
    {
        get => GetPropertyValue(PropertyNames.AnimationDirection);
        set => SetPropertyValue(PropertyNames.AnimationDirection, value);
    }

    /// <summary>The animation duration member.</summary>
    public string AnimationDuration
    {
        get => GetPropertyValue(PropertyNames.AnimationDuration);
        set => SetPropertyValue(PropertyNames.AnimationDuration, value);
    }

    /// <summary>The animation fill mode member.</summary>
    public string AnimationFillMode
    {
        get => GetPropertyValue(PropertyNames.AnimationFillMode);
        set => SetPropertyValue(PropertyNames.AnimationFillMode, value);
    }

    /// <summary>The animation iteration count member.</summary>
    public string AnimationIterationCount
    {
        get => GetPropertyValue(PropertyNames.AnimationIterationCount);
        set => SetPropertyValue(PropertyNames.AnimationIterationCount, value);
    }

    /// <summary>The animation name member.</summary>
    public string AnimationName
    {
        get => GetPropertyValue(PropertyNames.AnimationName);
        set => SetPropertyValue(PropertyNames.AnimationName, value);
    }

    /// <summary>The animation play state member.</summary>
    public string AnimationPlayState
    {
        get => GetPropertyValue(PropertyNames.AnimationPlayState);
        set => SetPropertyValue(PropertyNames.AnimationPlayState, value);
    }

    /// <summary>The animation timing function member.</summary>
    public string AnimationTimingFunction
    {
        get => GetPropertyValue(PropertyNames.AnimationTimingFunction);
        set => SetPropertyValue(PropertyNames.AnimationTimingFunction, value);
    }

    /// <summary>The backface visibility member.</summary>
    public string BackfaceVisibility
    {
        get => GetPropertyValue(PropertyNames.BackfaceVisibility);
        set => SetPropertyValue(PropertyNames.BackfaceVisibility, value);
    }

    /// <summary>The background member.</summary>
    public string Background
    {
        get => GetPropertyValue(PropertyNames.Background);
        set => SetPropertyValue(PropertyNames.Background, value);
    }

    /// <summary>The background attachment member.</summary>
    public string BackgroundAttachment
    {
        get => GetPropertyValue(PropertyNames.BackgroundAttachment);
        set => SetPropertyValue(PropertyNames.BackgroundAttachment, value);
    }

    /// <summary>The background clip member.</summary>
    public string BackgroundClip
    {
        get => GetPropertyValue(PropertyNames.BackgroundClip);
        set => SetPropertyValue(PropertyNames.BackgroundClip, value);
    }

    /// <summary>The background color member.</summary>
    public string BackgroundColor
    {
        get => GetPropertyValue(PropertyNames.BackgroundColor);
        set => SetPropertyValue(PropertyNames.BackgroundColor, value);
    }

    /// <summary>The background image member.</summary>
    public string BackgroundImage
    {
        get => GetPropertyValue(PropertyNames.BackgroundImage);
        set => SetPropertyValue(PropertyNames.BackgroundImage, value);
    }

    /// <summary>The background origin member.</summary>
    public string BackgroundOrigin
    {
        get => GetPropertyValue(PropertyNames.BackgroundOrigin);
        set => SetPropertyValue(PropertyNames.BackgroundOrigin, value);
    }

    /// <summary>The background position member.</summary>
    public string BackgroundPosition
    {
        get => GetPropertyValue(PropertyNames.BackgroundPosition);
        set => SetPropertyValue(PropertyNames.BackgroundPosition, value);
    }

    /// <summary>The background position x member.</summary>
    public string BackgroundPositionX
    {
        get => GetPropertyValue(PropertyNames.BackgroundPositionX);
        set => SetPropertyValue(PropertyNames.BackgroundPositionX, value);
    }

    /// <summary>The background position y member.</summary>
    public string BackgroundPositionY
    {
        get => GetPropertyValue(PropertyNames.BackgroundPositionY);
        set => SetPropertyValue(PropertyNames.BackgroundPositionY, value);
    }

    /// <summary>The background repeat member.</summary>
    public string BackgroundRepeat
    {
        get => GetPropertyValue(PropertyNames.BackgroundRepeat);
        set => SetPropertyValue(PropertyNames.BackgroundRepeat, value);
    }

    /// <summary>The background size member.</summary>
    public string BackgroundSize
    {
        get => GetPropertyValue(PropertyNames.BackgroundSize);
        set => SetPropertyValue(PropertyNames.BackgroundSize, value);
    }

    /// <summary>The baseline shift member.</summary>
    public string BaselineShift
    {
        get => GetPropertyValue(PropertyNames.BaselineShift);
        set => SetPropertyValue(PropertyNames.BaselineShift, value);
    }

    /// <summary>The behavior member.</summary>
    public string Behavior
    {
        get => GetPropertyValue(PropertyNames.Behavior);
        set => SetPropertyValue(PropertyNames.Behavior, value);
    }

    /// <summary>The bottom member.</summary>
    public string Bottom
    {
        get => GetPropertyValue(PropertyNames.Bottom);
        set => SetPropertyValue(PropertyNames.Bottom, value);
    }

    /// <summary>The border member.</summary>
    public string Border
    {
        get => GetPropertyValue(PropertyNames.Border);
        set => SetPropertyValue(PropertyNames.Border, value);
    }

    /// <summary>The border bottom member.</summary>
    public string BorderBottom
    {
        get => GetPropertyValue(PropertyNames.BorderBottom);
        set => SetPropertyValue(PropertyNames.BorderBottom, value);
    }

    /// <summary>The border bottom color member.</summary>
    public string BorderBottomColor
    {
        get => GetPropertyValue(PropertyNames.BorderBottomColor);
        set => SetPropertyValue(PropertyNames.BorderBottomColor, value);
    }

    /// <summary>The border bottom left radius member.</summary>
    public string BorderBottomLeftRadius
    {
        get => GetPropertyValue(PropertyNames.BorderBottomLeftRadius);
        set => SetPropertyValue(PropertyNames.BorderBottomLeftRadius, value);
    }

    /// <summary>The border bottom right radius member.</summary>
    public string BorderBottomRightRadius
    {
        get => GetPropertyValue(PropertyNames.BorderBottomRightRadius);
        set => SetPropertyValue(PropertyNames.BorderBottomRightRadius, value);
    }

    /// <summary>The border bottom style member.</summary>
    public string BorderBottomStyle
    {
        get => GetPropertyValue(PropertyNames.BorderBottomStyle);
        set => SetPropertyValue(PropertyNames.BorderBottomStyle, value);
    }

    /// <summary>The border bottom width member.</summary>
    public string BorderBottomWidth
    {
        get => GetPropertyValue(PropertyNames.BorderBottomWidth);
        set => SetPropertyValue(PropertyNames.BorderBottomWidth, value);
    }

    /// <summary>The border collapse member.</summary>
    public string BorderCollapse
    {
        get => GetPropertyValue(PropertyNames.BorderCollapse);
        set => SetPropertyValue(PropertyNames.BorderCollapse, value);
    }

    /// <summary>The border color member.</summary>
    public string BorderColor
    {
        get => GetPropertyValue(PropertyNames.BorderColor);
        set => SetPropertyValue(PropertyNames.BorderColor, value);
    }

    /// <summary>The border image member.</summary>
    public string BorderImage
    {
        get => GetPropertyValue(PropertyNames.BorderImage);
        set => SetPropertyValue(PropertyNames.BorderImage, value);
    }

    /// <summary>The border image outset member.</summary>
    public string BorderImageOutset
    {
        get => GetPropertyValue(PropertyNames.BorderImageOutset);
        set => SetPropertyValue(PropertyNames.BorderImageOutset, value);
    }

    /// <summary>The border image repeat member.</summary>
    public string BorderImageRepeat
    {
        get => GetPropertyValue(PropertyNames.BorderImageRepeat);
        set => SetPropertyValue(PropertyNames.BorderImageRepeat, value);
    }

    /// <summary>The border image slice member.</summary>
    public string BorderImageSlice
    {
        get => GetPropertyValue(PropertyNames.BorderImageSlice);
        set => SetPropertyValue(PropertyNames.BorderImageSlice, value);
    }

    /// <summary>The border image source member.</summary>
    public string BorderImageSource
    {
        get => GetPropertyValue(PropertyNames.BorderImageSource);
        set => SetPropertyValue(PropertyNames.BorderImageSource, value);
    }

    /// <summary>The border image width member.</summary>
    public string BorderImageWidth
    {
        get => GetPropertyValue(PropertyNames.BorderImageWidth);
        set => SetPropertyValue(PropertyNames.BorderImageWidth, value);
    }

    /// <summary>The border left member.</summary>
    public string BorderLeft
    {
        get => GetPropertyValue(PropertyNames.BorderLeft);
        set => SetPropertyValue(PropertyNames.BorderLeft, value);
    }

    /// <summary>The border left color member.</summary>
    public string BorderLeftColor
    {
        get => GetPropertyValue(PropertyNames.BorderLeftColor);
        set => SetPropertyValue(PropertyNames.BorderLeftColor, value);
    }

    /// <summary>The border left style member.</summary>
    public string BorderLeftStyle
    {
        get => GetPropertyValue(PropertyNames.BorderLeftStyle);
        set => SetPropertyValue(PropertyNames.BorderLeftStyle, value);
    }

    /// <summary>The border left width member.</summary>
    public string BorderLeftWidth
    {
        get => GetPropertyValue(PropertyNames.BorderLeftWidth);
        set => SetPropertyValue(PropertyNames.BorderLeftWidth, value);
    }

    /// <summary>The border radius member.</summary>
    public string BorderRadius
    {
        get => GetPropertyValue(PropertyNames.BorderRadius);
        set => SetPropertyValue(PropertyNames.BorderRadius, value);
    }

    /// <summary>The border right member.</summary>
    public string BorderRight
    {
        get => GetPropertyValue(PropertyNames.BorderRight);
        set => SetPropertyValue(PropertyNames.BorderRight, value);
    }

    /// <summary>The border right color member.</summary>
    public string BorderRightColor
    {
        get => GetPropertyValue(PropertyNames.BorderRightColor);
        set => SetPropertyValue(PropertyNames.BorderRightColor, value);
    }

    /// <summary>The border right style member.</summary>
    public string BorderRightStyle
    {
        get => GetPropertyValue(PropertyNames.BorderRightStyle);
        set => SetPropertyValue(PropertyNames.BorderRightStyle, value);
    }

    /// <summary>The border right width member.</summary>
    public string BorderRightWidth
    {
        get => GetPropertyValue(PropertyNames.BorderRightWidth);
        set => SetPropertyValue(PropertyNames.BorderRightWidth, value);
    }

    /// <summary>The border spacing member.</summary>
    public string BorderSpacing
    {
        get => GetPropertyValue(PropertyNames.BorderSpacing);
        set => SetPropertyValue(PropertyNames.BorderSpacing, value);
    }

    /// <summary>The border style member.</summary>
    public string BorderStyle
    {
        get => GetPropertyValue(PropertyNames.BorderStyle);
        set => SetPropertyValue(PropertyNames.BorderStyle, value);
    }

    /// <summary>The border top member.</summary>
    public string BorderTop
    {
        get => GetPropertyValue(PropertyNames.BorderTop);
        set => SetPropertyValue(PropertyNames.BorderTop, value);
    }

    /// <summary>The border top color member.</summary>
    public string BorderTopColor
    {
        get => GetPropertyValue(PropertyNames.BorderTopColor);
        set => SetPropertyValue(PropertyNames.BorderTopColor, value);
    }

    /// <summary>The border top left radius member.</summary>
    public string BorderTopLeftRadius
    {
        get => GetPropertyValue(PropertyNames.BorderTopLeftRadius);
        set => SetPropertyValue(PropertyNames.BorderTopLeftRadius, value);
    }

    /// <summary>The border top right radius member.</summary>
    public string BorderTopRightRadius
    {
        get => GetPropertyValue(PropertyNames.BorderTopRightRadius);
        set => SetPropertyValue(PropertyNames.BorderTopRightRadius, value);
    }

    /// <summary>The border top style member.</summary>
    public string BorderTopStyle
    {
        get => GetPropertyValue(PropertyNames.BorderTopStyle);
        set => SetPropertyValue(PropertyNames.BorderTopStyle, value);
    }

    /// <summary>The border top width member.</summary>
    public string BorderTopWidth
    {
        get => GetPropertyValue(PropertyNames.BorderTopWidth);
        set => SetPropertyValue(PropertyNames.BorderTopWidth, value);
    }

    /// <summary>The border width member.</summary>
    public string BorderWidth
    {
        get => GetPropertyValue(PropertyNames.BorderWidth);
        set => SetPropertyValue(PropertyNames.BorderWidth, value);
    }

    /// <summary>The box shadow member.</summary>
    public string BoxShadow
    {
        get => GetPropertyValue(PropertyNames.BoxShadow);
        set => SetPropertyValue(PropertyNames.BoxShadow, value);
    }

    /// <summary>The box sizing member.</summary>
    public string BoxSizing
    {
        get => GetPropertyValue(PropertyNames.BoxSizing);
        set => SetPropertyValue(PropertyNames.BoxSizing, value);
    }

    /// <summary>The break after member.</summary>
    public string BreakAfter
    {
        get => GetPropertyValue(PropertyNames.BreakAfter);
        set => SetPropertyValue(PropertyNames.BreakAfter, value);
    }

    /// <summary>The break before member.</summary>
    public string BreakBefore
    {
        get => GetPropertyValue(PropertyNames.BreakBefore);
        set => SetPropertyValue(PropertyNames.BreakBefore, value);
    }

    /// <summary>The break inside member.</summary>
    public string BreakInside
    {
        get => GetPropertyValue(PropertyNames.BreakInside);
        set => SetPropertyValue(PropertyNames.BreakInside, value);
    }

    /// <summary>The caption side member.</summary>
    public string CaptionSide
    {
        get => GetPropertyValue(PropertyNames.CaptionSide);
        set => SetPropertyValue(PropertyNames.CaptionSide, value);
    }

    /// <summary>The clear member.</summary>
    public new string Clear
    {
        get => GetPropertyValue(PropertyNames.Clear);
        set => SetPropertyValue(PropertyNames.Clear, value);
    }

    /// <summary>The clip member.</summary>
    public string Clip
    {
        get => GetPropertyValue(PropertyNames.Clip);
        set => SetPropertyValue(PropertyNames.Clip, value);
    }

    /// <summary>The clip bottom member.</summary>
    public string ClipBottom
    {
        get => GetPropertyValue(PropertyNames.ClipBottom);
        set => SetPropertyValue(PropertyNames.ClipBottom, value);
    }

    /// <summary>The clip left member.</summary>
    public string ClipLeft
    {
        get => GetPropertyValue(PropertyNames.ClipLeft);
        set => SetPropertyValue(PropertyNames.ClipLeft, value);
    }

    /// <summary>The clip path member.</summary>
    public string ClipPath
    {
        get => GetPropertyValue(PropertyNames.ClipPath);
        set => SetPropertyValue(PropertyNames.ClipPath, value);
    }

    /// <summary>The clip right member.</summary>
    public string ClipRight
    {
        get => GetPropertyValue(PropertyNames.ClipRight);
        set => SetPropertyValue(PropertyNames.ClipRight, value);
    }

    /// <summary>The clip rule member.</summary>
    public string ClipRule
    {
        get => GetPropertyValue(PropertyNames.ClipRule);
        set => SetPropertyValue(PropertyNames.ClipRule, value);
    }

    /// <summary>The clip top member.</summary>
    public string ClipTop
    {
        get => GetPropertyValue(PropertyNames.ClipTop);
        set => SetPropertyValue(PropertyNames.ClipTop, value);
    }

    /// <summary>The color member.</summary>
    public string Color
    {
        get => GetPropertyValue(PropertyNames.Color);
        set => SetPropertyValue(PropertyNames.Color, value);
    }

    /// <summary>The color interpolation filters member.</summary>
    public string ColorInterpolationFilters
    {
        get => GetPropertyValue(PropertyNames.ColorInterpolationFilters);
        set => SetPropertyValue(PropertyNames.ColorInterpolationFilters, value);
    }

    /// <summary>The column count member.</summary>
    public string ColumnCount
    {
        get => GetPropertyValue(PropertyNames.ColumnCount);
        set => SetPropertyValue(PropertyNames.ColumnCount, value);
    }

    /// <summary>The column fill member.</summary>
    public string ColumnFill
    {
        get => GetPropertyValue(PropertyNames.ColumnFill);
        set => SetPropertyValue(PropertyNames.ColumnFill, value);
    }

    /// <summary>The column gap member.</summary>
    public string ColumnGap
    {
        get => GetPropertyValue(PropertyNames.ColumnGap);
        set => SetPropertyValue(PropertyNames.ColumnGap, value);
    }

    /// <summary>The column rule member.</summary>
    public string ColumnRule
    {
        get => GetPropertyValue(PropertyNames.ColumnRule);
        set => SetPropertyValue(PropertyNames.ColumnRule, value);
    }

    /// <summary>The column rule color member.</summary>
    public string ColumnRuleColor
    {
        get => GetPropertyValue(PropertyNames.ColumnRuleColor);
        set => SetPropertyValue(PropertyNames.ColumnRuleColor, value);
    }

    /// <summary>The column rule style member.</summary>
    public string ColumnRuleStyle
    {
        get => GetPropertyValue(PropertyNames.ColumnRuleStyle);
        set => SetPropertyValue(PropertyNames.ColumnRuleStyle, value);
    }

    /// <summary>The column rule width member.</summary>
    public string ColumnRuleWidth
    {
        get => GetPropertyValue(PropertyNames.ColumnRuleWidth);
        set => SetPropertyValue(PropertyNames.ColumnRuleWidth, value);
    }

    /// <summary>The columns member.</summary>
    public string Columns
    {
        get => GetPropertyValue(PropertyNames.Columns);
        set => SetPropertyValue(PropertyNames.Columns, value);
    }

    /// <summary>The column span member.</summary>
    public string ColumnSpan
    {
        get => GetPropertyValue(PropertyNames.ColumnSpan);
        set => SetPropertyValue(PropertyNames.ColumnSpan, value);
    }

    /// <summary>The column width member.</summary>
    public string ColumnWidth
    {
        get => GetPropertyValue(PropertyNames.ColumnWidth);
        set => SetPropertyValue(PropertyNames.ColumnWidth, value);
    }

    /// <summary>The container name member.</summary>
    public string ContainerName
    {
        get => GetPropertyValue(PropertyNames.ContainerName);
        set => SetPropertyValue(PropertyNames.ContainerName, value);
    }

    /// <summary>The container type member.</summary>
    public string ContainerType
    {
        get => GetPropertyValue(PropertyNames.ContainerType);
        set => SetPropertyValue(PropertyNames.ContainerType, value);
    }

    /// <summary>The content member.</summary>
    public string Content
    {
        get => GetPropertyValue(PropertyNames.Content);
        set => SetPropertyValue(PropertyNames.Content, value);
    }

    /// <summary>The counter increment member.</summary>
    public string CounterIncrement
    {
        get => GetPropertyValue(PropertyNames.CounterIncrement);
        set => SetPropertyValue(PropertyNames.CounterIncrement, value);
    }

    /// <summary>The counter reset member.</summary>
    public string CounterReset
    {
        get => GetPropertyValue(PropertyNames.CounterReset);
        set => SetPropertyValue(PropertyNames.CounterReset, value);
    }

    /// <summary>The float member.</summary>
    public string Float
    {
        get => GetPropertyValue(PropertyNames.Float);
        set => SetPropertyValue(PropertyNames.Float, value);
    }

    /// <summary>The cursor member.</summary>
    public string Cursor
    {
        get => GetPropertyValue(PropertyNames.Cursor);
        set => SetPropertyValue(PropertyNames.Cursor, value);
    }

    /// <summary>The direction member.</summary>
    public string Direction
    {
        get => GetPropertyValue(PropertyNames.Direction);
        set => SetPropertyValue(PropertyNames.Direction, value);
    }

    /// <summary>The display member.</summary>
    public string Display
    {
        get => GetPropertyValue(PropertyNames.Display);
        set => SetPropertyValue(PropertyNames.Display, value);
    }

    /// <summary>The dominant baseline member.</summary>
    public string DominantBaseline
    {
        get => GetPropertyValue(PropertyNames.DominantBaseline);
        set => SetPropertyValue(PropertyNames.DominantBaseline, value);
    }

    /// <summary>The empty cells member.</summary>
    public string EmptyCells
    {
        get => GetPropertyValue(PropertyNames.EmptyCells);
        set => SetPropertyValue(PropertyNames.EmptyCells, value);
    }

    /// <summary>The enable background member.</summary>
    public string EnableBackground
    {
        get => GetPropertyValue(PropertyNames.EnableBackground);
        set => SetPropertyValue(PropertyNames.EnableBackground, value);
    }

    /// <summary>The fill member.</summary>
    public string Fill
    {
        get => GetPropertyValue(PropertyNames.Fill);
        set => SetPropertyValue(PropertyNames.Fill, value);
    }

    /// <summary>The fill opacity member.</summary>
    public string FillOpacity
    {
        get => GetPropertyValue(PropertyNames.FillOpacity);
        set => SetPropertyValue(PropertyNames.FillOpacity, value);
    }

    /// <summary>The fill rule member.</summary>
    public string FillRule
    {
        get => GetPropertyValue(PropertyNames.FillRule);
        set => SetPropertyValue(PropertyNames.FillRule, value);
    }

    /// <summary>The filter member.</summary>
    public string Filter
    {
        get => GetPropertyValue(PropertyNames.Filter);
        set => SetPropertyValue(PropertyNames.Filter, value);
    }

    /// <summary>The flex member.</summary>
    public string Flex
    {
        get => GetPropertyValue(PropertyNames.Flex);
        set => SetPropertyValue(PropertyNames.Flex, value);
    }

    /// <summary>The flex basis member.</summary>
    public string FlexBasis
    {
        get => GetPropertyValue(PropertyNames.FlexBasis);
        set => SetPropertyValue(PropertyNames.FlexBasis, value);
    }

    /// <summary>The flex direction member.</summary>
    public string FlexDirection
    {
        get => GetPropertyValue(PropertyNames.FlexDirection);
        set => SetPropertyValue(PropertyNames.FlexDirection, value);
    }

    /// <summary>The flex flow member.</summary>
    public string FlexFlow
    {
        get => GetPropertyValue(PropertyNames.FlexFlow);
        set => SetPropertyValue(PropertyNames.FlexFlow, value);
    }

    /// <summary>The flex grow member.</summary>
    public string FlexGrow
    {
        get => GetPropertyValue(PropertyNames.FlexGrow);
        set => SetPropertyValue(PropertyNames.FlexGrow, value);
    }

    /// <summary>The flex shrink member.</summary>
    public string FlexShrink
    {
        get => GetPropertyValue(PropertyNames.FlexShrink);
        set => SetPropertyValue(PropertyNames.FlexShrink, value);
    }

    /// <summary>The flex wrap member.</summary>
    public string FlexWrap
    {
        get => GetPropertyValue(PropertyNames.FlexWrap);
        set => SetPropertyValue(PropertyNames.FlexWrap, value);
    }

    /// <summary>The font member.</summary>
    public string Font
    {
        get => GetPropertyValue(PropertyNames.Font);
        set => SetPropertyValue(PropertyNames.Font, value);
    }

    /// <summary>The font family member.</summary>
    public string FontFamily
    {
        get => GetPropertyValue(PropertyNames.FontFamily);
        set => SetPropertyValue(PropertyNames.FontFamily, value);
    }

    /// <summary>The font feature settings member.</summary>
    public string FontFeatureSettings
    {
        get => GetPropertyValue(PropertyNames.FontFeatureSettings);
        set => SetPropertyValue(PropertyNames.FontFeatureSettings, value);
    }

    /// <summary>The font size member.</summary>
    public string FontSize
    {
        get => GetPropertyValue(PropertyNames.FontSize);
        set => SetPropertyValue(PropertyNames.FontSize, value);
    }

    /// <summary>The font size adjust member.</summary>
    public string FontSizeAdjust
    {
        get => GetPropertyValue(PropertyNames.FontSizeAdjust);
        set => SetPropertyValue(PropertyNames.FontSizeAdjust, value);
    }

    /// <summary>The font stretch member.</summary>
    public string FontStretch
    {
        get => GetPropertyValue(PropertyNames.FontStretch);
        set => SetPropertyValue(PropertyNames.FontStretch, value);
    }

    /// <summary>The font style member.</summary>
    public string FontStyle
    {
        get => GetPropertyValue(PropertyNames.FontStyle);
        set => SetPropertyValue(PropertyNames.FontStyle, value);
    }

    /// <summary>The font variant member.</summary>
    public string FontVariant
    {
        get => GetPropertyValue(PropertyNames.FontVariant);
        set => SetPropertyValue(PropertyNames.FontVariant, value);
    }

    /// <summary>The font weight member.</summary>
    public string FontWeight
    {
        get => GetPropertyValue(PropertyNames.FontWeight);
        set => SetPropertyValue(PropertyNames.FontWeight, value);
    }

    /// <summary>The gap member.</summary>
    public string Gap
    {
        get => GetPropertyValue(PropertyNames.Gap);
        set => SetPropertyValue(PropertyNames.Gap, value);
    }

    /// <summary>The glyph orientation horizontal member.</summary>
    public string GlyphOrientationHorizontal
    {
        get => GetPropertyValue(PropertyNames.GlyphOrientationHorizontal);
        set => SetPropertyValue(PropertyNames.GlyphOrientationHorizontal, value);
    }

    /// <summary>The glyph orientation vertical member.</summary>
    public string GlyphOrientationVertical
    {
        get => GetPropertyValue(PropertyNames.GlyphOrientationVertical);
        set => SetPropertyValue(PropertyNames.GlyphOrientationVertical, value);
    }

    /// <summary>The height member.</summary>
    public string Height
    {
        get => GetPropertyValue(PropertyNames.Height);
        set => SetPropertyValue(PropertyNames.Height, value);
    }

    /// <summary>The ime mode member.</summary>
    public string ImeMode
    {
        get => GetPropertyValue(PropertyNames.ImeMode);
        set => SetPropertyValue(PropertyNames.ImeMode, value);
    }

    /// <summary>The justify content member.</summary>
    public string JustifyContent
    {
        get => GetPropertyValue(PropertyNames.JustifyContent);
        set => SetPropertyValue(PropertyNames.JustifyContent, value);
    }

    /// <summary>The layout grid member.</summary>
    public string LayoutGrid
    {
        get => GetPropertyValue(PropertyNames.LayoutGrid);
        set => SetPropertyValue(PropertyNames.LayoutGrid, value);
    }

    /// <summary>The layout grid char member.</summary>
    public string LayoutGridChar
    {
        get => GetPropertyValue(PropertyNames.LayoutGridChar);
        set => SetPropertyValue(PropertyNames.LayoutGridChar, value);
    }

    /// <summary>The layout grid line member.</summary>
    public string LayoutGridLine
    {
        get => GetPropertyValue(PropertyNames.LayoutGridLine);
        set => SetPropertyValue(PropertyNames.LayoutGridLine, value);
    }

    /// <summary>The layout grid mode member.</summary>
    public string LayoutGridMode
    {
        get => GetPropertyValue(PropertyNames.LayoutGridMode);
        set => SetPropertyValue(PropertyNames.LayoutGridMode, value);
    }

    /// <summary>The layout grid type member.</summary>
    public string LayoutGridType
    {
        get => GetPropertyValue(PropertyNames.LayoutGridType);
        set => SetPropertyValue(PropertyNames.LayoutGridType, value);
    }

    /// <summary>The left member.</summary>
    public string Left
    {
        get => GetPropertyValue(PropertyNames.Left);
        set => SetPropertyValue(PropertyNames.Left, value);
    }

    /// <summary>The letter spacing member.</summary>
    public string LetterSpacing
    {
        get => GetPropertyValue(PropertyNames.LetterSpacing);
        set => SetPropertyValue(PropertyNames.LetterSpacing, value);
    }

    /// <summary>The line height member.</summary>
    public string LineHeight
    {
        get => GetPropertyValue(PropertyNames.LineHeight);
        set => SetPropertyValue(PropertyNames.LineHeight, value);
    }

    /// <summary>The list style member.</summary>
    public string ListStyle
    {
        get => GetPropertyValue(PropertyNames.ListStyle);
        set => SetPropertyValue(PropertyNames.ListStyle, value);
    }

    /// <summary>The list style image member.</summary>
    public string ListStyleImage
    {
        get => GetPropertyValue(PropertyNames.ListStyleImage);
        set => SetPropertyValue(PropertyNames.ListStyleImage, value);
    }

    /// <summary>The list style position member.</summary>
    public string ListStylePosition
    {
        get => GetPropertyValue(PropertyNames.ListStylePosition);
        set => SetPropertyValue(PropertyNames.ListStylePosition, value);
    }

    /// <summary>The list style type member.</summary>
    public string ListStyleType
    {
        get => GetPropertyValue(PropertyNames.ListStyleType);
        set => SetPropertyValue(PropertyNames.ListStyleType, value);
    }

    /// <summary>The margin member.</summary>
    public string Margin
    {
        get => GetPropertyValue(PropertyNames.Margin);
        set => SetPropertyValue(PropertyNames.Margin, value);
    }

    /// <summary>The margin bottom member.</summary>
    public string MarginBottom
    {
        get => GetPropertyValue(PropertyNames.MarginBottom);
        set => SetPropertyValue(PropertyNames.MarginBottom, value);
    }

    /// <summary>The margin left member.</summary>
    public string MarginLeft
    {
        get => GetPropertyValue(PropertyNames.MarginLeft);
        set => SetPropertyValue(PropertyNames.MarginLeft, value);
    }

    /// <summary>The margin right member.</summary>
    public string MarginRight
    {
        get => GetPropertyValue(PropertyNames.MarginRight);
        set => SetPropertyValue(PropertyNames.MarginRight, value);
    }

    /// <summary>The margin top member.</summary>
    public string MarginTop
    {
        get => GetPropertyValue(PropertyNames.MarginTop);
        set => SetPropertyValue(PropertyNames.MarginTop, value);
    }

    /// <summary>The marker member.</summary>
    public string Marker
    {
        get => GetPropertyValue(PropertyNames.Marker);
        set => SetPropertyValue(PropertyNames.Marker, value);
    }

    /// <summary>The marker end member.</summary>
    public string MarkerEnd
    {
        get => GetPropertyValue(PropertyNames.MarkerEnd);
        set => SetPropertyValue(PropertyNames.MarkerEnd, value);
    }

    /// <summary>The marker mid member.</summary>
    public string MarkerMid
    {
        get => GetPropertyValue(PropertyNames.MarkerMid);
        set => SetPropertyValue(PropertyNames.MarkerMid, value);
    }

    /// <summary>The marker start member.</summary>
    public string MarkerStart
    {
        get => GetPropertyValue(PropertyNames.MarkerStart);
        set => SetPropertyValue(PropertyNames.MarkerStart, value);
    }

    /// <summary>The mask member.</summary>
    public string Mask
    {
        get => GetPropertyValue(PropertyNames.Mask);
        set => SetPropertyValue(PropertyNames.Mask, value);
    }

    /// <summary>The max height member.</summary>
    public string MaxHeight
    {
        get => GetPropertyValue(PropertyNames.MaxHeight);
        set => SetPropertyValue(PropertyNames.MaxHeight, value);
    }

    /// <summary>The max width member.</summary>
    public string MaxWidth
    {
        get => GetPropertyValue(PropertyNames.MaxWidth);
        set => SetPropertyValue(PropertyNames.MaxWidth, value);
    }

    /// <summary>The min height member.</summary>
    public string MinHeight
    {
        get => GetPropertyValue(PropertyNames.MinHeight);
        set => SetPropertyValue(PropertyNames.MinHeight, value);
    }

    /// <summary>The min width member.</summary>
    public string MinWidth
    {
        get => GetPropertyValue(PropertyNames.MinWidth);
        set => SetPropertyValue(PropertyNames.MinWidth, value);
    }

    /// <summary>The opacity member.</summary>
    public string Opacity
    {
        get => GetPropertyValue(PropertyNames.Opacity);
        set => SetPropertyValue(PropertyNames.Opacity, value);
    }

    /// <summary>The order member.</summary>
    public string Order
    {
        get => GetPropertyValue(PropertyNames.Order);
        set => SetPropertyValue(PropertyNames.Order, value);
    }

    /// <summary>The orphans member.</summary>
    public string Orphans
    {
        get => GetPropertyValue(PropertyNames.Orphans);
        set => SetPropertyValue(PropertyNames.Orphans, value);
    }

    /// <summary>The outline member.</summary>
    public string Outline
    {
        get => GetPropertyValue(PropertyNames.Outline);
        set => SetPropertyValue(PropertyNames.Outline, value);
    }

    /// <summary>The outline color member.</summary>
    public string OutlineColor
    {
        get => GetPropertyValue(PropertyNames.OutlineColor);
        set => SetPropertyValue(PropertyNames.OutlineColor, value);
    }

    /// <summary>The outline style member.</summary>
    public string OutlineStyle
    {
        get => GetPropertyValue(PropertyNames.OutlineStyle);
        set => SetPropertyValue(PropertyNames.OutlineStyle, value);
    }

    /// <summary>The outline width member.</summary>
    public string OutlineWidth
    {
        get => GetPropertyValue(PropertyNames.OutlineWidth);
        set => SetPropertyValue(PropertyNames.OutlineWidth, value);
    }

    /// <summary>The overflow member.</summary>
    public string Overflow
    {
        get => GetPropertyValue(PropertyNames.Overflow);
        set => SetPropertyValue(PropertyNames.Overflow, value);
    }

    /// <summary>The overflow x member.</summary>
    public string OverflowX
    {
        get => GetPropertyValue(PropertyNames.OverflowX);
        set => SetPropertyValue(PropertyNames.OverflowX, value);
    }

    /// <summary>The overflow y member.</summary>
    public string OverflowY
    {
        get => GetPropertyValue(PropertyNames.OverflowY);
        set => SetPropertyValue(PropertyNames.OverflowY, value);
    }

    /// <summary>The overflow wrap member.</summary>
    public string OverflowWrap
    {
        get => GetPropertyValue(PropertyNames.WordWrap);
        set => SetPropertyValue(PropertyNames.WordWrap, value);
    }

    /// <summary>The padding member.</summary>
    public string Padding
    {
        get => GetPropertyValue(PropertyNames.Padding);
        set => SetPropertyValue(PropertyNames.Padding, value);
    }

    /// <summary>The padding bottom member.</summary>
    public string PaddingBottom
    {
        get => GetPropertyValue(PropertyNames.PaddingBottom);
        set => SetPropertyValue(PropertyNames.PaddingBottom, value);
    }

    /// <summary>The padding left member.</summary>
    public string PaddingLeft
    {
        get => GetPropertyValue(PropertyNames.PaddingLeft);
        set => SetPropertyValue(PropertyNames.PaddingLeft, value);
    }

    /// <summary>The padding right member.</summary>
    public string PaddingRight
    {
        get => GetPropertyValue(PropertyNames.PaddingRight);
        set => SetPropertyValue(PropertyNames.PaddingRight, value);
    }

    /// <summary>The padding top member.</summary>
    public string PaddingTop
    {
        get => GetPropertyValue(PropertyNames.PaddingTop);
        set => SetPropertyValue(PropertyNames.PaddingTop, value);
    }

    /// <summary>The page break after member.</summary>
    public string PageBreakAfter
    {
        get => GetPropertyValue(PropertyNames.PageBreakAfter);
        set => SetPropertyValue(PropertyNames.PageBreakAfter, value);
    }

    /// <summary>The page break before member.</summary>
    public string PageBreakBefore
    {
        get => GetPropertyValue(PropertyNames.PageBreakBefore);
        set => SetPropertyValue(PropertyNames.PageBreakBefore, value);
    }

    /// <summary>The page break inside member.</summary>
    public string PageBreakInside
    {
        get => GetPropertyValue(PropertyNames.PageBreakInside);
        set => SetPropertyValue(PropertyNames.PageBreakInside, value);
    }

    /// <summary>The perspective member.</summary>
    public string Perspective
    {
        get => GetPropertyValue(PropertyNames.Perspective);
        set => SetPropertyValue(PropertyNames.Perspective, value);
    }

    /// <summary>The perspective origin member.</summary>
    public string PerspectiveOrigin
    {
        get => GetPropertyValue(PropertyNames.PerspectiveOrigin);
        set => SetPropertyValue(PropertyNames.PerspectiveOrigin, value);
    }

    /// <summary>The pointer events member.</summary>
    public string PointerEvents
    {
        get => GetPropertyValue(PropertyNames.PointerEvents);
        set => SetPropertyValue(PropertyNames.PointerEvents, value);
    }

    /// <summary>The row gap member.</summary>
    public string RowGap
    {
        get => GetPropertyValue(PropertyNames.RowGap);
        set => SetPropertyValue(PropertyNames.RowGap, value);
    }

    /// <summary>The quotes member.</summary>
    public string Quotes
    {
        get => GetPropertyValue(PropertyNames.Quotes);
        set => SetPropertyValue(PropertyNames.Quotes, value);
    }

    /// <summary>The position member.</summary>
    public string Position
    {
        get => GetPropertyValue(PropertyNames.Position);
        set => SetPropertyValue(PropertyNames.Position, value);
    }

    /// <summary>The right member.</summary>
    public string Right
    {
        get => GetPropertyValue(PropertyNames.Right);
        set => SetPropertyValue(PropertyNames.Right, value);
    }

    /// <summary>The ruby align member.</summary>
    public string RubyAlign
    {
        get => GetPropertyValue(PropertyNames.RubyAlign);
        set => SetPropertyValue(PropertyNames.RubyAlign, value);
    }

    /// <summary>The ruby overhang member.</summary>
    public string RubyOverhang
    {
        get => GetPropertyValue(PropertyNames.RubyOverhang);
        set => SetPropertyValue(PropertyNames.RubyOverhang, value);
    }

    /// <summary>The ruby position member.</summary>
    public string RubyPosition
    {
        get => GetPropertyValue(PropertyNames.RubyPosition);
        set => SetPropertyValue(PropertyNames.RubyPosition, value);
    }

    /// <summary>The scrollbar3d light color member.</summary>
    public string Scrollbar3DLightColor
    {
        get => GetPropertyValue(PropertyNames.Scrollbar3dLightColor);
        set => SetPropertyValue(PropertyNames.Scrollbar3dLightColor, value);
    }

    /// <summary>The scrollbar arrow color member.</summary>
    public string ScrollbarArrowColor
    {
        get => GetPropertyValue(PropertyNames.ScrollbarArrowColor);
        set => SetPropertyValue(PropertyNames.ScrollbarArrowColor, value);
    }

    /// <summary>The scrollbar dark shadow color member.</summary>
    public string ScrollbarDarkShadowColor
    {
        get => GetPropertyValue(PropertyNames.ScrollbarDarkShadowColor);
        set => SetPropertyValue(PropertyNames.ScrollbarDarkShadowColor, value);
    }

    /// <summary>The scrollbar face color member.</summary>
    public string ScrollbarFaceColor
    {
        get => GetPropertyValue(PropertyNames.ScrollbarFaceColor);
        set => SetPropertyValue(PropertyNames.ScrollbarFaceColor, value);
    }

    /// <summary>The scrollbar highlight color member.</summary>
    public string ScrollbarHighlightColor
    {
        get => GetPropertyValue(PropertyNames.ScrollbarHighlightColor);
        set => SetPropertyValue(PropertyNames.ScrollbarHighlightColor, value);
    }

    /// <summary>The scrollbar shadow color member.</summary>
    public string ScrollbarShadowColor
    {
        get => GetPropertyValue(PropertyNames.ScrollbarShadowColor);
        set => SetPropertyValue(PropertyNames.ScrollbarShadowColor, value);
    }

    /// <summary>The scrollbar track color member.</summary>
    public string ScrollbarTrackColor
    {
        get => GetPropertyValue(PropertyNames.ScrollbarTrackColor);
        set => SetPropertyValue(PropertyNames.ScrollbarTrackColor, value);
    }

    /// <summary>The stroke member.</summary>
    public string Stroke
    {
        get => GetPropertyValue(PropertyNames.Stroke);
        set => SetPropertyValue(PropertyNames.Stroke, value);
    }

    /// <summary>The stroke dasharray member.</summary>
    public string StrokeDasharray
    {
        get => GetPropertyValue(PropertyNames.StrokeDasharray);
        set => SetPropertyValue(PropertyNames.StrokeDasharray, value);
    }

    /// <summary>The stroke dashoffset member.</summary>
    public string StrokeDashoffset
    {
        get => GetPropertyValue(PropertyNames.StrokeDashoffset);
        set => SetPropertyValue(PropertyNames.StrokeDashoffset, value);
    }

    /// <summary>The stroke linecap member.</summary>
    public string StrokeLinecap
    {
        get => GetPropertyValue(PropertyNames.StrokeLinecap);
        set => SetPropertyValue(PropertyNames.StrokeLinecap, value);
    }

    /// <summary>The stroke linejoin member.</summary>
    public string StrokeLinejoin
    {
        get => GetPropertyValue(PropertyNames.StrokeLinejoin);
        set => SetPropertyValue(PropertyNames.StrokeLinejoin, value);
    }

    /// <summary>The stroke miterlimit member.</summary>
    public string StrokeMiterlimit
    {
        get => GetPropertyValue(PropertyNames.StrokeMiterlimit);
        set => SetPropertyValue(PropertyNames.StrokeMiterlimit, value);
    }

    /// <summary>The stroke opacity member.</summary>
    public string StrokeOpacity
    {
        get => GetPropertyValue(PropertyNames.StrokeOpacity);
        set => SetPropertyValue(PropertyNames.StrokeOpacity, value);
    }

    /// <summary>The stroke width member.</summary>
    public string StrokeWidth
    {
        get => GetPropertyValue(PropertyNames.StrokeWidth);
        set => SetPropertyValue(PropertyNames.StrokeWidth, value);
    }

    /// <summary>The table layout member.</summary>
    public string TableLayout
    {
        get => GetPropertyValue(PropertyNames.TableLayout);
        set => SetPropertyValue(PropertyNames.TableLayout, value);
    }

    /// <summary>The text align member.</summary>
    public string TextAlign
    {
        get => GetPropertyValue(PropertyNames.TextAlign);
        set => SetPropertyValue(PropertyNames.TextAlign, value);
    }

    /// <summary>The text align last member.</summary>
    public string TextAlignLast
    {
        get => GetPropertyValue(PropertyNames.TextAlignLast);
        set => SetPropertyValue(PropertyNames.TextAlignLast, value);
    }

    /// <summary>The text anchor member.</summary>
    public string TextAnchor
    {
        get => GetPropertyValue(PropertyNames.TextAnchor);
        set => SetPropertyValue(PropertyNames.TextAnchor, value);
    }

    /// <summary>The text autospace member.</summary>
    public string TextAutospace
    {
        get => GetPropertyValue(PropertyNames.TextAutospace);
        set => SetPropertyValue(PropertyNames.TextAutospace, value);
    }

    /// <summary>The text decoration member.</summary>
    public string TextDecoration
    {
        get => GetPropertyValue(PropertyNames.TextDecoration);
        set => SetPropertyValue(PropertyNames.TextDecoration, value);
    }

    /// <summary>The text indent member.</summary>
    public string TextIndent
    {
        get => GetPropertyValue(PropertyNames.TextIndent);
        set => SetPropertyValue(PropertyNames.TextIndent, value);
    }

    /// <summary>The text justify member.</summary>
    public string TextJustify
    {
        get => GetPropertyValue(PropertyNames.TextJustify);
        set => SetPropertyValue(PropertyNames.TextJustify, value);
    }

    /// <summary>The text overflow member.</summary>
    public string TextOverflow
    {
        get => GetPropertyValue(PropertyNames.TextOverflow);
        set => SetPropertyValue(PropertyNames.TextOverflow, value);
    }

    /// <summary>The text shadow member.</summary>
    public string TextShadow
    {
        get => GetPropertyValue(PropertyNames.TextShadow);
        set => SetPropertyValue(PropertyNames.TextShadow, value);
    }

    /// <summary>The text transform member.</summary>
    public string TextTransform
    {
        get => GetPropertyValue(PropertyNames.TextTransform);
        set => SetPropertyValue(PropertyNames.TextTransform, value);
    }

    /// <summary>The text underline position member.</summary>
    public string TextUnderlinePosition
    {
        get => GetPropertyValue(PropertyNames.TextUnderlinePosition);
        set => SetPropertyValue(PropertyNames.TextUnderlinePosition, value);
    }

    /// <summary>The top member.</summary>
    public string Top
    {
        get => GetPropertyValue(PropertyNames.Top);
        set => SetPropertyValue(PropertyNames.Top, value);
    }

    /// <summary>The transform member.</summary>
    public string Transform
    {
        get => GetPropertyValue(PropertyNames.Transform);
        set => SetPropertyValue(PropertyNames.Transform, value);
    }

    /// <summary>The transform origin member.</summary>
    public string TransformOrigin
    {
        get => GetPropertyValue(PropertyNames.TransformOrigin);
        set => SetPropertyValue(PropertyNames.TransformOrigin, value);
    }

    /// <summary>The transform style member.</summary>
    public string TransformStyle
    {
        get => GetPropertyValue(PropertyNames.TransformStyle);
        set => SetPropertyValue(PropertyNames.TransformStyle, value);
    }

    /// <summary>The transition member.</summary>
    public string Transition
    {
        get => GetPropertyValue(PropertyNames.Transition);
        set => SetPropertyValue(PropertyNames.Transition, value);
    }

    /// <summary>The transition delay member.</summary>
    public string TransitionDelay
    {
        get => GetPropertyValue(PropertyNames.TransitionDelay);
        set => SetPropertyValue(PropertyNames.TransitionDelay, value);
    }

    /// <summary>The transition duration member.</summary>
    public string TransitionDuration
    {
        get => GetPropertyValue(PropertyNames.TransitionDuration);
        set => SetPropertyValue(PropertyNames.TransitionDuration, value);
    }

    /// <summary>The transition property member.</summary>
    public string TransitionProperty
    {
        get => GetPropertyValue(PropertyNames.TransitionProperty);
        set => SetPropertyValue(PropertyNames.TransitionProperty, value);
    }

    /// <summary>The transition timing function member.</summary>
    public string TransitionTimingFunction
    {
        get => GetPropertyValue(PropertyNames.TransitionTimingFunction);
        set => SetPropertyValue(PropertyNames.TransitionTimingFunction, value);
    }

    /// <summary>The unicode bidirectional member.</summary>
    public string UnicodeBidirectional
    {
        get => GetPropertyValue(PropertyNames.UnicodeBidirectional);
        set => SetPropertyValue(PropertyNames.UnicodeBidirectional, value);
    }

    /// <summary>The vertical align member.</summary>
    public string VerticalAlign
    {
        get => GetPropertyValue(PropertyNames.VerticalAlign);
        set => SetPropertyValue(PropertyNames.VerticalAlign, value);
    }

    /// <summary>The visibility member.</summary>
    public string Visibility
    {
        get => GetPropertyValue(PropertyNames.Visibility);
        set => SetPropertyValue(PropertyNames.Visibility, value);
    }

    /// <summary>The white space member.</summary>
    public string WhiteSpace
    {
        get => GetPropertyValue(PropertyNames.WhiteSpace);
        set => SetPropertyValue(PropertyNames.WhiteSpace, value);
    }

    /// <summary>The widows member.</summary>
    public string Widows
    {
        get => GetPropertyValue(PropertyNames.Widows);
        set => SetPropertyValue(PropertyNames.Widows, value);
    }

    /// <summary>The width member.</summary>
    public string Width
    {
        get => GetPropertyValue(PropertyNames.Width);
        set => SetPropertyValue(PropertyNames.Width, value);
    }

    /// <summary>The word break member.</summary>
    public string WordBreak
    {
        get => GetPropertyValue(PropertyNames.WordBreak);
        set => SetPropertyValue(PropertyNames.WordBreak, value);
    }

    /// <summary>The word spacing member.</summary>
    public string WordSpacing
    {
        get => GetPropertyValue(PropertyNames.WordSpacing);
        set => SetPropertyValue(PropertyNames.WordSpacing, value);
    }

    /// <summary>The writing mode member.</summary>
    public string WritingMode
    {
        get => GetPropertyValue(PropertyNames.WritingMode);
        set => SetPropertyValue(PropertyNames.WritingMode, value);
    }

    /// <summary>The z index member.</summary>
    public string ZIndex
    {
        get => GetPropertyValue(PropertyNames.ZIndex);
        set => SetPropertyValue(PropertyNames.ZIndex, value);
    }

    /// <summary>The zoom member.</summary>
    public string Zoom
    {
        get => GetPropertyValue(PropertyNames.Zoom);
        set => SetPropertyValue(PropertyNames.Zoom, value);
    }
}
