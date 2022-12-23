//-----------------------------------------------------------------------
// <copyright file="HoconRoot.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2016 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

namespace Akka.Configuration.Hocon;

/// <summary>
///     This class represents the root element in a HOCON (Human-Optimized Config Object Notation)
///     configuration string.
/// </summary>
public class HoconRoot
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="HoconRoot" /> class.
    /// </summary>
    /// <param name="value">The value to associate with this element.</param>
    public HoconRoot(HoconValue value)
    {
        Value = value;
    }

    /// <summary>
    ///     Retrieves the value associated with this element.
    /// </summary>
    public HoconValue Value { get; }
}