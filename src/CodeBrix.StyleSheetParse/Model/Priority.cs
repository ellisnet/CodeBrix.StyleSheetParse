using System;
using System.Runtime.InteropServices;

namespace CodeBrix.StyleSheetParse; //Was previously: namespace ExCSS;

/// <summary>Represents a CSS specificity priority value.</summary>
[StructLayout(LayoutKind.Explicit, Pack = 1, CharSet = CharSet.Unicode)]
public struct Priority : IEquatable<Priority>, IComparable<Priority>
{
    [FieldOffset(0)] private readonly uint _priority;

    /// <summary>Gets the zero priority value.</summary>
    public static readonly Priority Zero = new (0u);
    /// <summary>Gets the priority value for a single tag selector.</summary>
    public static readonly Priority OneTag = new (0, 0, 0, 1);
    /// <summary>Gets the priority value for a single class selector.</summary>
    public static readonly Priority OneClass = new (0, 0, 1, 0);
    /// <summary>Gets the priority value for a single ID selector.</summary>
    public static readonly Priority OneId = new (0, 1, 0, 0);
    /// <summary>Gets the priority value for an inline style.</summary>
    public static readonly Priority Inline = new (1, 0, 0, 0);

    /// <summary>Initializes a new instance of the <see cref="Priority"/> struct with a raw priority value.</summary>
    public Priority(uint priority)
    {
        Inlines = Ids = Classes = Tags = 0;
        _priority = priority;
    }

    /// <summary>Initializes a new instance of the <see cref="Priority"/> struct with individual specificity components.</summary>
    public Priority(byte inlines, byte ids, byte classes, byte tags)
    {
        _priority = 0;
        Inlines = inlines;
        Ids = ids;
        Classes = classes;
        Tags = tags;
    }

    /// <summary>Gets the number of ID selectors.</summary>
    [field: FieldOffset(2)]
    public byte Ids { get; }

    /// <summary>Gets the number of tag selectors.</summary>
    [field: FieldOffset(0)]
    public byte Tags { get; }

    /// <summary>Gets the number of class selectors.</summary>
    [field: FieldOffset(1)]
    public byte Classes { get; }

    /// <summary>Gets the number of inline styles.</summary>
    [field: FieldOffset(3)]
    public byte Inlines { get; }

    /// <summary>Adds two priority values.</summary>
    public static Priority operator +(Priority a, Priority b)
    {
        return new(a._priority + b._priority);
    }

    /// <summary>Determines whether two priority values are equal.</summary>
    public static bool operator ==(Priority a, Priority b)
    {
        return a._priority == b._priority;
    }

    /// <summary>Determines whether the first priority is greater than the second.</summary>
    public static bool operator >(Priority a, Priority b)
    {
        return a._priority > b._priority;
    }

    /// <summary>Determines whether the first priority is greater than or equal to the second.</summary>
    public static bool operator >=(Priority a, Priority b)
    {
        return a._priority >= b._priority;
    }

    /// <summary>Determines whether the first priority is less than the second.</summary>
    public static bool operator <(Priority a, Priority b)
    {
        return a._priority < b._priority;
    }

    /// <summary>Determines whether the first priority is less than or equal to the second.</summary>
    public static bool operator <=(Priority a, Priority b)
    {
        return a._priority <= b._priority;
    }

    /// <summary>Determines whether two priority values are not equal.</summary>
    public static bool operator !=(Priority a, Priority b)
    {
        return a._priority != b._priority;
    }

    /// <summary>Determines whether the specified priority is equal to the current priority.</summary>
    public bool Equals(Priority other)
    {
        return _priority == other._priority;
    }

    /// <summary>Determines whether the specified object is equal to the current priority.</summary>
    public override bool Equals(object obj)
    {
        return obj is Priority priority && Equals(priority);
    }

    /// <summary>Returns the hash code for this priority.</summary>
    public override int GetHashCode()
    {
        return (int) _priority;
    }

    /// <summary>Compares the current priority with another priority.</summary>
    public int CompareTo(Priority other)
    {
        return this == other ? 0 : (this > other ? 1 : -1);
    }

    /// <summary>Returns the string representation of this priority.</summary>
    public override string ToString()
    {
        return $"({Inlines}, {Ids}, {Classes}, {Tags})";
    }
}
