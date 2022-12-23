//-----------------------------------------------------------------------
// <copyright file="Config.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2016 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using Akka.Configuration.Hocon;

namespace Hocon;

/// <summary>
///     This class represents the main configuration object used by Akka.NET
///     when configuring objects within the system. To put it simply, it's
///     the internal representation of a HOCON (Human-Optimized Config Object Notation)
///     configuration string.
/// </summary>
public class Config
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Config" /> class.
    /// </summary>
    public Config()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Config" /> class.
    /// </summary>
    /// <param name="root">The root node to base this configuration.</param>
    /// <exception cref="ArgumentNullException">"The root value cannot be null."</exception>
    public Config(HoconRoot root)
    {
        if (root.Value == null)
            throw new ArgumentNullException("root.Value");

        Root = root.Value;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Config" /> class.
    /// </summary>
    /// <param name="source">The configuration to use as the primary source.</param>
    /// <param name="fallback">The configuration to use as a secondary source.</param>
    /// <exception cref="ArgumentNullException">The source configuration cannot be null.</exception>
    public Config(Config source, Config fallback)
    {
        if (source == null)
            throw new ArgumentNullException("source");

        Root = source.Root;
        Fallback = fallback;
    }

    /// <summary>
    ///     The configuration used as a secondary source.
    /// </summary>
    public Config Fallback { get; }

    /// <summary>
    ///     Determines if this root node contains any values
    /// </summary>
    public virtual bool IsEmpty => Root == null || Root.IsEmpty;

    /// <summary>
    ///     The root node of this configuration section
    /// </summary>
    public virtual HoconValue Root { get; }


    private HoconValue GetNode(string path)
    {
        var elements = path.SplitDottedPathHonouringQuotes();
        var currentNode = Root;
        if (currentNode == null) throw new InvalidOperationException("Current node should not be null");
        foreach (var key in elements)
        {
            currentNode = currentNode.GetChildObject(key);
            if (currentNode == null)
            {
                if (Fallback != null)
                    return Fallback.GetNode(path);

                return null;
            }
        }

        return currentNode;
    }

    /// <summary>
    ///     Retrieves a boolean value from the specified path in the configuration.
    /// </summary>
    /// <param name="path">The path that contains the value to retrieve.</param>
    /// <param name="default">The default value to return if the value doesn't exist.</param>
    /// <returns>The boolean value defined in the specified path.</returns>
    public virtual bool GetBoolean(string path, bool @default = false)
    {
        var value = GetNode(path);
        if (value == null)
            return @default;

        return value.GetBoolean();
    }

    /// <summary>
    ///     Retrieves a long value, optionally suffixed with a 'b', from the specified path in the configuration.
    /// </summary>
    /// <param name="path">The path that contains the value to retrieve.</param>
    /// <returns>The long value defined in the specified path.</returns>
    public virtual long? GetByteSize(string path)
    {
        var value = GetNode(path);
        if (value == null) return null;
        return value.GetByteSize();
    }

    /// <summary>
    ///     Retrieves an integer value from the specified path in the configuration.
    /// </summary>
    /// <param name="path">The path that contains the value to retrieve.</param>
    /// <param name="default">The default value to return if the value doesn't exist.</param>
    /// <returns>The integer value defined in the specified path.</returns>
    public virtual int GetInt(string path, int @default = 0)
    {
        var value = GetNode(path);
        if (value == null)
            return @default;

        return value.GetInt();
    }

    /// <summary>
    ///     Retrieves a long value from the specified path in the configuration.
    /// </summary>
    /// <param name="path">The path that contains the value to retrieve.</param>
    /// <param name="default">The default value to return if the value doesn't exist.</param>
    /// <returns>The long value defined in the specified path.</returns>
    public virtual long GetLong(string path, long @default = 0)
    {
        var value = GetNode(path);
        if (value == null)
            return @default;

        return value.GetLong();
    }

    /// <summary>
    ///     Retrieves a string value from the specified path in the configuration.
    /// </summary>
    /// <param name="path">The path that contains the value to retrieve.</param>
    /// <param name="default">The default value to return if the value doesn't exist.</param>
    /// <returns>The string value defined in the specified path.</returns>
    public virtual string GetString(string path, string @default = null)
    {
        var value = GetNode(path);
        if (value == null)
            return @default;

        return value.GetString();
    }

    /// <summary>
    ///     Retrieves a float value from the specified path in the configuration.
    /// </summary>
    /// <param name="path">The path that contains the value to retrieve.</param>
    /// <param name="default">The default value to return if the value doesn't exist.</param>
    /// <returns>The float value defined in the specified path.</returns>
    public virtual float GetFloat(string path, float @default = 0)
    {
        var value = GetNode(path);
        if (value == null)
            return @default;

        return value.GetFloat();
    }

    /// <summary>
    ///     Retrieves a decimal value from the specified path in the configuration.
    /// </summary>
    /// <param name="path">The path that contains the value to retrieve.</param>
    /// <param name="default">The default value to return if the value doesn't exist.</param>
    /// <returns>The decimal value defined in the specified path.</returns>
    public virtual decimal GetDecimal(string path, decimal @default = 0)
    {
        var value = GetNode(path);
        if (value == null)
            return @default;

        return value.GetDecimal();
    }

    /// <summary>
    ///     Retrieves a double value from the specified path in the configuration.
    /// </summary>
    /// <param name="path">The path that contains the value to retrieve.</param>
    /// <param name="default">The default value to return if the value doesn't exist.</param>
    /// <returns>The double value defined in the specified path.</returns>
    public virtual double GetDouble(string path, double @default = 0)
    {
        var value = GetNode(path);
        if (value == null)
            return @default;

        return value.GetDouble();
    }

    /// <summary>
    ///     Retrieves a list of boolean values from the specified path in the configuration.
    /// </summary>
    /// <param name="path">The path that contains the values to retrieve.</param>
    /// <returns>The list of boolean values defined in the specified path.</returns>
    public virtual IList<bool> GetBooleanList(string path)
    {
        var value = GetNode(path);
        return value.GetBooleanList();
    }

    /// <summary>
    ///     Retrieves a list of decimal values from the specified path in the configuration.
    /// </summary>
    /// <param name="path">The path that contains the values to retrieve.</param>
    /// <returns>The list of decimal values defined in the specified path.</returns>
    public virtual IList<decimal> GetDecimalList(string path)
    {
        var value = GetNode(path);
        return value.GetDecimalList();
    }

    /// <summary>
    ///     Retrieves a list of float values from the specified path in the configuration.
    /// </summary>
    /// <param name="path">The path that contains the values to retrieve.</param>
    /// <returns>The list of float values defined in the specified path.</returns>
    public virtual IList<float> GetFloatList(string path)
    {
        var value = GetNode(path);
        return value.GetFloatList();
    }

    /// <summary>
    ///     Retrieves a list of double values from the specified path in the configuration.
    /// </summary>
    /// <param name="path">The path that contains the values to retrieve.</param>
    /// <returns>The list of double values defined in the specified path.</returns>
    public virtual IList<double> GetDoubleList(string path)
    {
        var value = GetNode(path);
        return value.GetDoubleList();
    }

    /// <summary>
    ///     Retrieves a list of int values from the specified path in the configuration.
    /// </summary>
    /// <param name="path">The path that contains the values to retrieve.</param>
    /// <returns>The list of int values defined in the specified path.</returns>
    public virtual IList<int> GetIntList(string path)
    {
        var value = GetNode(path);
        return value.GetIntList();
    }

    /// <summary>
    ///     Retrieves a list of long values from the specified path in the configuration.
    /// </summary>
    /// <param name="path">The path that contains the values to retrieve.</param>
    /// <returns>The list of long values defined in the specified path.</returns>
    public virtual IList<long> GetLongList(string path)
    {
        var value = GetNode(path);
        return value.GetLongList();
    }

    /// <summary>
    ///     Retrieves a list of byte values from the specified path in the configuration.
    /// </summary>
    /// <param name="path">The path that contains the values to retrieve.</param>
    /// <returns>The list of byte values defined in the specified path.</returns>
    public virtual IList<byte> GetByteList(string path)
    {
        var value = GetNode(path);
        return value.GetByteList();
    }

    /// <summary>
    ///     Retrieves a list of string values from the specified path in the configuration.
    /// </summary>
    /// <param name="path">The path that contains the values to retrieve.</param>
    /// <returns>The list of string values defined in the specified path.</returns>
    public virtual IList<string> GetStringList(string path)
    {
        var value = GetNode(path);
        if (value == null) return new string[0];
        return value.GetStringList();
    }

    /// <summary>
    ///     Retrieves a new configuration from the current configuration
    ///     with the root node being the supplied path.
    /// </summary>
    /// <param name="path">The path that contains the configuration to retrieve.</param>
    /// <returns>A new configuration with the root node being the supplied path.</returns>
    public virtual Config GetConfig(string path)
    {
        var value = GetNode(path);


        if (value == null)
            return null;

        return new Config(new HoconRoot(value));
    }

    /// <summary>
    ///     Retrieves a <see cref="HoconValue" /> from a specific path.
    /// </summary>
    /// <param name="path">The path that contains the value to retrieve.</param>
    /// <returns>The <see cref="HoconValue" /> found at the location if one exists, otherwise <c>null</c>.</returns>
    public HoconValue GetValue(string path)
    {
        var value = GetNode(path);
        return value;
    }

    /// <summary>
    ///     Obsolete. Use <see cref="GetTimeSpan" /> to retrieve <see cref="TimeSpan" /> information. This method will be
    ///     removed in future versions.
    /// </summary>
    /// <param name="path">N/A</param>
    /// <param name="default">N/A</param>
    /// <param name="allowInfinite">N/A</param>
    /// <returns>N/A</returns>
    [Obsolete("Use GetTimeSpan to retrieve TimeSpan information. This method will be removed in future versions.")]
    public TimeSpan GetMillisDuration(string path, TimeSpan? @default = null, bool allowInfinite = true)
    {
        return GetTimeSpan(path, @default, allowInfinite);
    }

    /// <summary>
    ///     Retrieves a <see cref="TimeSpan" /> value from the specified path in the configuration.
    /// </summary>
    /// <param name="path">The path that contains the value to retrieve.</param>
    /// <param name="default">The default value to return if the value doesn't exist.</param>
    /// <param name="allowInfinite"><c>true</c> if infinite timespans are allowed; otherwise <c>false</c>.</param>
    /// <returns>The <see cref="TimeSpan" /> value defined in the specified path.</returns>
    public virtual TimeSpan GetTimeSpan(string path, TimeSpan? @default = null, bool allowInfinite = true)
    {
        var value = GetNode(path);
        if (value == null)
            return @default.GetValueOrDefault();

        return value.GetTimeSpan(allowInfinite);
    }

    /// <summary>
    ///     Converts the current configuration to a string.
    /// </summary>
    /// <returns>A string containing the current configuration.</returns>
    public override string ToString()
    {
        if (Root == null)
            return "";

        return Root.ToString();
    }


    /// <summary>
    ///     Determine if a HOCON configuration element exists at the specified location
    /// </summary>
    /// <param name="path">The location to check for a configuration value.</param>
    /// <returns><c>true</c> if a value was found, <c>false</c> otherwise.</returns>
    public virtual bool HasPath(string path)
    {
        var value = GetNode(path);
        return value != null;
    }


    /// <summary>
    ///     Retrieves an enumerable key value pair representation of the current configuration.
    /// </summary>
    /// <returns>The current configuration represented as an enumerable key value pair.</returns>
    public virtual IEnumerable<KeyValuePair<string, HoconValue>> AsEnumerable()
    {
        var used = new HashSet<string>();
        var current = this;
        while (current != null)
        {
            foreach (var kvp in current.Root.GetObject().Items)
                if (!used.Contains(kvp.Key))
                {
                    yield return kvp;
                    used.Add(kvp.Key);
                }

            current = current.Fallback;
        }
    }
}

public static class Extensions
{
    public static IEnumerable<string> SplitDottedPathHonouringQuotes(this string path)
    {
        return path.Split('\"')
            .AlternateSelectMany(
                outsideQuote => outsideQuote.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries),
                insideQuote => new[] { insideQuote });
    }

    public static IEnumerable<TOut> AlternateSelectMany<TIn, TOut>(this IEnumerable<TIn> self,
        Func<TIn, IEnumerable<TOut>> evenSelector, Func<TIn, IEnumerable<TOut>> oddSelector)
    {
        return self.SelectMany((val, i) => i % 2 == 0 ? evenSelector(val) : oddSelector(val));
    }
}