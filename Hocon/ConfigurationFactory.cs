//-----------------------------------------------------------------------
// <copyright file="ConfigurationFactory.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2016 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using Akka.Configuration.Hocon;
using Hocon;

namespace Akka.Configuration;

/// <summary>
///     This class contains methods used to retrieve configuration information
///     from a variety of sources including user-supplied strings, configuration
///     files and assembly resources.
/// </summary>
public class ConfigurationFactory
{
    /// <summary>
    ///     Generates an empty configuration.
    /// </summary>
    public static Config Empty => ParseString("");

    /// <summary>
    ///     Generates a configuration defined in the supplied
    ///     HOCON (Human-Optimized Config Object Notation) string.
    /// </summary>
    /// <param name="hocon">A string that contains configuration options to use.</param>
    /// <param name="includeCallback">callback used to resolve includes</param>
    /// <returns>The configuration defined in the supplied HOCON string.</returns>
    public static Config ParseString(string hocon)
    {
        var res = Parser.Parse(hocon);
        return new Config(res);
    }
}