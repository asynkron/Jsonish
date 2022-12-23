//-----------------------------------------------------------------------
// <copyright file="HoconParser.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2016 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

namespace Akka.Configuration.Hocon;

/// <summary>
///     This class contains methods used to parse HOCON (Human-Optimized Config Object Notation)
///     configuration strings.
/// </summary>
public class Parser
{
    private HoconTokenizer _reader;
    private HoconValue _root;


    /// <summary>
    ///     Parses the supplied HOCON configuration string into a root element.
    /// </summary>
    /// <param name="text">The string that contains a HOCON configuration string.</param>
    /// <returns>The root element created from the supplied HOCON configuration string.</returns>
    /// <exception cref="System.Exception">
    ///     This exception is thrown when an unresolved substitution is encountered.
    ///     It also occurs when the end of the file has been reached while trying
    ///     to read a value.
    /// </exception>
    public static HoconRoot Parse(string text)
    {
        return new Parser().ParseText(text);
    }

    private HoconRoot ParseText(string text)
    {
        _root = new HoconValue();
        _reader = new HoconTokenizer(text);
        _reader.PullWhitespaceAndComments();
        ParseObject(_root, true, "");

        return new HoconRoot(_root);
    }

    private void ParseObject(HoconValue owner, bool root, string currentPath)
    {
        if (owner.IsObject())
        {
            //the value of this KVP is already an object
        }
        else
        {
            //the value of this KVP is not an object, thus, we should add a new
            owner.NewValue(new HoconObject());
        }

        var currentObject = owner.GetObject();

        while (!_reader.EoF)
        {
            var t = _reader.PullNext();
            switch (t.Type)
            {
                case TokenType.EoF:
                    break;
                case TokenType.Key:
                    var value = currentObject.GetOrCreateKey(t.Value);
                    var nextPath = currentPath == "" ? t.Value : $"{currentPath}.{t.Value}";
                    ParseKeyContent(value, nextPath);
                    if (!root)
                        return;
                    break;

                case TokenType.ObjectEnd:
                    return;
            }
        }
    }

    private void ParseKeyContent(HoconValue value, string currentPath)
    {
        while (!_reader.EoF)
        {
            var t = _reader.PullNext();
            switch (t.Type)
            {
                case TokenType.Dot:
                    ParseObject(value, false, currentPath);
                    return;
                case TokenType.Assign:

                    if (!value.IsObject())
                        //if not an object, then replace the value.
                        //if object. value should be merged
                        value.Clear();
                    ParseValue(value, currentPath);
                    return;
                case TokenType.ObjectStart:
                    ParseObject(value, true, currentPath);
                    return;
            }
        }
    }

    /// <summary>
    ///     Retrieves the next value token from the tokenizer and appends it
    ///     to the supplied element <paramref name="owner" />.
    /// </summary>
    /// <param name="owner">The element to append the next token.</param>
    /// <param name="currentPath">The location in the HOCON object hierarchy that the parser is currently reading.</param>
    /// <exception cref="System.Exception">End of file reached while trying to read a value</exception>
    private void ParseValue(HoconValue owner, string currentPath)
    {
        if (_reader.EoF)
            throw new Exception("End of file reached while trying to read a value");

        _reader.PullWhitespaceAndComments();
        while (_reader.IsValue())
        {
            var t = _reader.PullValue();

            switch (t.Type)
            {
                case TokenType.EoF:
                    break;
                case TokenType.LiteralValue:
                    if (owner.IsObject())
                        //needed to allow for override objects
                        owner.Clear();
                    var lit = new HoconLiteral
                    {
                        Value = t.Value
                    };
                    owner.AppendValue(lit);

                    break;
                case TokenType.ObjectStart:
                    ParseObject(owner, true, currentPath);
                    break;
                case TokenType.ArrayStart:
                    var arr = ParseArray(currentPath);
                    owner.AppendValue(arr);
                    break;
            }

            if (_reader.IsSpaceOrTab()) ParseTrailingWhitespace(owner);
        }

        IgnoreComma();
    }

    private void ParseTrailingWhitespace(HoconValue owner)
    {
        var ws = _reader.PullSpaceOrTab();
        //single line ws should be included if string concat
        if (ws.Value.Length > 0)
        {
            var wsLit = new HoconLiteral
            {
                Value = ws.Value
            };
            owner.AppendValue(wsLit);
        }
    }

    /// <summary>
    ///     Retrieves the next array token from the tokenizer.
    /// </summary>
    /// <param name="currentPath">The location in the HOCON object hierarchy that the parser is currently reading.</param>
    /// <returns>An array of elements retrieved from the token.</returns>
    private HoconArray ParseArray(string currentPath)
    {
        var arr = new HoconArray();
        while (!_reader.EoF && !_reader.IsArrayEnd())
        {
            var v = new HoconValue();
            ParseValue(v, currentPath);
            arr.Add(v);
            _reader.PullWhitespaceAndComments();
        }

        _reader.PullArrayEnd();
        return arr;
    }

    private void IgnoreComma()
    {
        if (_reader.IsComma()) //optional end of value
            _reader.PullComma();
    }
}