//-----------------------------------------------------------------------
// <copyright file="HoconTests.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2016 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using Akka.Configuration;
using Shouldly;

namespace Hocon.Tests;

public class HoconTests
{
    //Added tests to conform to the HOCON spec https://github.com/Lightbendhub/config/blob/master/HOCON.md
    [Fact]
    public void Can_use_paths_as_keys_3_14()
    {
        var hocon1 = """3.14 : 42""";
        var hocon2 = """3 { 14 : 42}""";
        Assert.Equal(ConfigurationFactory.ParseString(hocon1).GetString("3.14"),
            ConfigurationFactory.ParseString(hocon2).GetString("3.14"));
    }

    [Fact]
    public void Can_use_paths_as_keys_3()
    {
        var hocon1 = """3 : 42""";
        var hocon2 = """
                "3" : 42
                """;
        Assert.Equal(ConfigurationFactory.ParseString(hocon1).GetString("3"),
            ConfigurationFactory.ParseString(hocon2).GetString("3"));
    }

    [Fact]
    public void Can_use_paths_as_keys_true()
    {
        var hocon1 = """true : 42""";
        var hocon2 = """
                "true" : 42
                """;
        Assert.Equal(ConfigurationFactory.ParseString(hocon1).GetString("true"),
            ConfigurationFactory.ParseString(hocon2).GetString("true"));
    }

    [Fact]
    public void Can_use_paths_as_keys_FooBar()
    {
        var hocon1 = """foo.bar : 42""";
        var hocon2 = """foo { bar : 42 }""";
        Assert.Equal(ConfigurationFactory.ParseString(hocon1).GetString("foo.bar"),
            ConfigurationFactory.ParseString(hocon2).GetString("foo.bar"));
    }

    [Fact]
    public void Can_use_paths_as_keys_FooBarBaz()
    {
        var hocon1 = """foo.bar.baz : 42""";
        var hocon2 = """foo { bar { baz : 42 } }""";
        Assert.Equal(ConfigurationFactory.ParseString(hocon1).GetString("foo.bar.baz"),
            ConfigurationFactory.ParseString(hocon2).GetString("foo.bar.baz"));
    }

    [Fact]
    public void Can_use_paths_as_keys_AX_AY()
    {
        var hocon1 = """a.x : 42, a.y : 43""";
        var hocon2 = """a { x : 42, y : 43 }""";
        Assert.Equal(ConfigurationFactory.ParseString(hocon1).GetString("a.x"),
            ConfigurationFactory.ParseString(hocon2).GetString("a.x"));
        Assert.Equal(ConfigurationFactory.ParseString(hocon1).GetString("a.y"),
            ConfigurationFactory.ParseString(hocon2).GetString("a.y"));
    }

    [Fact]
    public void Can_use_paths_as_keys_A_B_C()
    {
        var hocon1 = """a b c : 42""";
        var hocon2 = """
                "a b c" : 42
                """;
        Assert.Equal(ConfigurationFactory.ParseString(hocon1).GetString("a b c"),
            ConfigurationFactory.ParseString(hocon2).GetString("a b c"));
    }


    [Fact]
    public void Can_parse_sub_config()
    {
        var hocon = """

                a {
                   b {
                     c = 1
                     d = true
                   }
                }
                """;
        var config = ConfigurationFactory.ParseString(hocon);
        var subConfig = config.GetConfig("a");
        Assert.Equal(1, subConfig.GetInt("b.c"));
        Assert.Equal(true, subConfig.GetBoolean("b.d"));
    }


    [Fact]
    public void Can_parse_hocon()
    {
        var hocon = """

                root {
                  int = 1
                  quoted-string = "foo"
                  unquoted-string = bar
                  concat-string = foo bar
                  object {
                    hasContent = true
                  }
                  array = [1,2,3,4]

                  array-single-element = [1 2 3 4]
                  array-newline-element = [
                    1
                    2
                    3
                    4
                  ]
                  null = null
                  double = 1.23
                  bool = true
                }

                """;
        var config = ConfigurationFactory.ParseString(hocon);
        Assert.Equal("1", config.GetString("root.int"));
        Assert.Equal("1.23", config.GetString("root.double"));
        Assert.Equal(true, config.GetBoolean("root.bool"));
        Assert.Equal(true, config.GetBoolean("root.object.hasContent"));
        Assert.Equal(null, config.GetString("root.null"));
        Assert.Equal("foo", config.GetString("root.quoted-string"));
        Assert.Equal("bar", config.GetString("root.unquoted-string"));
        Assert.Equal("foo bar", config.GetString("root.concat-string"));
        Assert.True(
            new[] { 1, 2, 3, 4 }.SequenceEqual(ConfigurationFactory.ParseString(hocon).GetIntList("root.array")));
        Assert.True(
            new[] { 1, 2, 3, 4 }.SequenceEqual(
                ConfigurationFactory.ParseString(hocon).GetIntList("root.array-newline-element")));
        Assert.True(
            new[] { "1 2 3 4" }.SequenceEqual(
                ConfigurationFactory.ParseString(hocon).GetStringList("root.array-single-element")));
    }

    [Fact]
    public void Can_parse_json()
    {
        var hocon = """

                "root" : {
                  "int" : 1,
                  "string" : "foo",
                  "object" : {
                        "hasContent" : true
                    },
                  "array" : [1,2,3],
                  "null" : null,
                  "double" : 1.23,
                  "bool" : true
                }

                """;
        var config = ConfigurationFactory.ParseString(hocon);
        Assert.Equal("1", config.GetString("root.int"));
        Assert.Equal("1.23", config.GetString("root.double"));
        Assert.Equal(true, config.GetBoolean("root.bool"));
        Assert.Equal(true, config.GetBoolean("root.object.hasContent"));
        Assert.Equal(null, config.GetString("root.null"));
        Assert.Equal("foo", config.GetString("root.string"));
        Assert.True(new[] { 1, 2, 3 }.SequenceEqual(ConfigurationFactory.ParseString(hocon).GetIntList("root.array")));
    }


    [Fact]
    public void Can_parse_object()
    {
        var hocon = """

                a {
                  b = 1
                }

                """;
        Assert.Equal("1", ConfigurationFactory.ParseString(hocon).GetString("a.b"));
    }

    [Fact]
    public void Can_trim_value()
    {
        var hocon = "a= \t \t 1 \t \t,";
        Assert.Equal("1", ConfigurationFactory.ParseString(hocon).GetString("a"));
    }

    [Fact]
    public void Can_trim_concatenated_value()
    {
        var hocon = "a= \t \t 1 2 3 \t \t,";
        Assert.Equal("1 2 3", ConfigurationFactory.ParseString(hocon).GetString("a"));
    }

    [Fact]
    public void Can_consume_comma_after_value()
    {
        var hocon = "a=1,";
        Assert.Equal("1", ConfigurationFactory.ParseString(hocon).GetString("a"));
    }

    [Fact]
    public void Can_assign_ipaddress_to_field()
    {
        var hocon = """a=127.0.0.1""";
        Assert.Equal("127.0.0.1", ConfigurationFactory.ParseString(hocon).GetString("a"));
    }

    [Fact]
    public void Can_assign_concatenated_value_to_field()
    {
        var hocon = """a=1 2 3""";
        Assert.Equal("1 2 3", ConfigurationFactory.ParseString(hocon).GetString("a"));
    }

    [Fact]
    public void Can_assign_value_to_quoted_field()
    {
        var hocon = """
                "a"=1
                """;
        Assert.Equal(1L, ConfigurationFactory.ParseString(hocon).GetLong("a"));
    }

    [Fact]
    public void Can_assign_value_to_path_expression()
    {
        var hocon = """a.b.c=1""";
        Assert.Equal(1L, ConfigurationFactory.ParseString(hocon).GetLong("a.b.c"));
    }

    [Fact]
    public void Can_assign_values_to_path_expressions()
    {
        var hocon = """

                a.b.c=1
                a.b.d=2
                a.b.e.f=3

                """;
        var config = ConfigurationFactory.ParseString(hocon);
        Assert.Equal(1L, config.GetLong("a.b.c"));
        Assert.Equal(2L, config.GetLong("a.b.d"));
        Assert.Equal(3L, config.GetLong("a.b.e.f"));
    }

    [Fact]
    public void Can_assign_long_to_field()
    {
        var hocon = """a=1""";
        Assert.Equal(1L, ConfigurationFactory.ParseString(hocon).GetLong("a"));
    }

    [Fact]
    public void Can_assign_array_to_field()
    {
        var hocon = """
                a=
                [
                    1
                    2
                    3
                ]
                """;
        Assert.True(new[] { 1, 2, 3 }.SequenceEqual(ConfigurationFactory.ParseString(hocon).GetIntList("a")));

        //hocon = @"a= [ 1, 2, 3 ]";
        //Assert.True(new[] { 1, 2, 3 }.SequenceEqual(ConfigurationFactory.ParseString(hocon).GetIntList("a")));
    }


    [Fact]
    public void Can_assign_double_to_field()
    {
        var hocon = """a=1.1""";
        Assert.Equal(1.1, ConfigurationFactory.ParseString(hocon).GetDouble("a"));
    }

    [Fact]
    public void Can_assign_null_to_field()
    {
        var hocon = """a=null""";
        Assert.Null(ConfigurationFactory.ParseString(hocon).GetString("a"));
    }

    [Fact]
    public void Can_assign_boolean_to_field()
    {
        var hocon = """a=true""";
        Assert.Equal(true, ConfigurationFactory.ParseString(hocon).GetBoolean("a"));
        hocon = """a=false""";
        Assert.Equal(false, ConfigurationFactory.ParseString(hocon).GetBoolean("a"));

        hocon = """a=on""";
        Assert.Equal(true, ConfigurationFactory.ParseString(hocon).GetBoolean("a"));
        hocon = """a=off""";
        Assert.Equal(false, ConfigurationFactory.ParseString(hocon).GetBoolean("a"));
    }

    [Fact]
    public void Can_assign_quoted_string_to_field()
    {
        var hocon = """
                a="hello"
                """;
        Assert.Equal("hello", ConfigurationFactory.ParseString(hocon).GetString("a"));
    }

    [Fact]
    public void Can_assign_un_quoted_string_to_field()
    {
        var hocon = """a=hello""";
        Assert.Equal("hello", ConfigurationFactory.ParseString(hocon).GetString("a"));
    }

    [Fact]
    public void Can_assign_triple_quoted_string_to_field()
    {
        var hocon = """"
                a="""hello"""
                """";
        Assert.Equal("hello", ConfigurationFactory.ParseString(hocon).GetString("a"));
    }

    [Fact]
    public void Can_assign_triple_quoted_string_with_unescaped_chars_to_field()
    {
        var hocon = """"
                a="""hello\y\o\u"""
                """";
        Assert.Equal("hello\\y\\o\\u", ConfigurationFactory.ParseString(hocon).GetString("a"));
    }

    [Fact]
    public void Can_assign_unescaped_path_like_variable_to_field()
    {
        var hocon = """"
                a="""C:\Dev\somepath\to\a\file.txt"""
                """";
        Assert.Equal("C:\\Dev\\somepath\\to\\a\\file.txt", ConfigurationFactory.ParseString(hocon).GetString("a"));
    }


    [Fact]
    public void Can_parse_quoted_keys()
    {
        var hocon = """

                a {
                   "some quoted, key": 123
                }

                """;
        var config = ConfigurationFactory.ParseString(hocon);
        config.GetInt("a.some quoted, key").ShouldBe(123);
    }

    [Fact]
    public void Can_parse_quoted_keys_with_dots()
    {
        var hocon = """

                a {
                   "/abc/d.ev/*": 123
                }

                """;
        var config = ConfigurationFactory.ParseString(hocon);
        config.GetConfig("a").Root.GetObject().GetKey("/abc/d.ev/*").GetInt().ShouldBe(123);
    }

    [Fact]
    public void Get_config_supports_quoting()
    {
        var hocon = """

                a {
                   "/abc/d.ev/*": 123
                }

                """;
        var config = ConfigurationFactory.ParseString(hocon);
        config.GetConfig("a").GetConfig("""
                "/abc/d.ev/*"
                """).ShouldNotBe(null);
    }

    [Fact]
    public void Get_config_supports_quoting_combined_with_dotting()
    {
        var hocon = """

                a {
                   "/abc/d.ev/*".d: 123
                }

                """;
        var config = ConfigurationFactory.ParseString(hocon);
        config.GetConfig("""
                a."/abc/d.ev/*"
                """).ShouldNotBe(null);
        config.GetConfig("""a."/abc/d.ev/*".d""").ShouldNotBe(null);
    }

    [Fact]
    public void Can_enumerate_quoted_keys()
    {
        var hocon = """

                a {
                   "some quoted, key": 123
                }

                """;
        var config = ConfigurationFactory.ParseString(hocon);
        var config2 = config.GetConfig("a");
        var enumerable = config2.AsEnumerable();

        enumerable.Select(kvp => kvp.Key).First().ShouldBe("some quoted, key");
    }

    [Fact]
    public void Can_enumerate_quoted_keys_with_dots()
    {
        var hocon = """

                a {
                   "/abc/d.ev/*": 123
                }

                """;
        var config = ConfigurationFactory.ParseString(hocon);
        var config2 = config.GetConfig("a");
        var enumerable = config2.AsEnumerable();

        enumerable.Select(kvp => kvp.Key).First().ShouldBe("/abc/d.ev/*");
    }

    [Fact]
    public void Can_parse_serializers_and_bindings()
    {
        var hocon = """

                akka.actor {
                    serializers {
                      akka-containers = "Akka.Remote.Serialization.MessageContainerSerializer, Akka.Remote"
                      proto = "Akka.Remote.Serialization.ProtobufSerializer, Akka.Remote"
                      daemon-create = "Akka.Remote.Serialization.DaemonMsgCreateSerializer, Akka.Remote"
                    }

                    serialization-bindings {
                      # Since com.google.protobuf.Message does not extend Serializable but
                      # GeneratedMessage does, need to use the more specific one here in order
                      # to avoid ambiguity
                      "Akka.Actor.ActorSelectionMessage" = akka-containers
                      "Akka.Remote.DaemonMsgCreate, Akka.Remote" = daemon-create
                    }

                }
                """;

        var config = ConfigurationFactory.ParseString(hocon);

        var serializersConfig = config.GetConfig("akka.actor.serializers").AsEnumerable().ToList();
        var serializerBindingConfig = config.GetConfig("akka.actor.serialization-bindings").AsEnumerable().ToList();

        serializersConfig.Select(kvp => kvp.Value)
            .First()
            .GetString()
            .ShouldBe("Akka.Remote.Serialization.MessageContainerSerializer, Akka.Remote");
        serializerBindingConfig.Select(kvp => kvp.Key).Last().ShouldBe("Akka.Remote.DaemonMsgCreate, Akka.Remote");
    }

    [Fact]
    public void Can_overwrite_value()
    {
        var hocon = """

                test {
                  value  = 123
                }
                test.value = 456

                """;
        var config = ConfigurationFactory.ParseString(hocon);
        config.GetInt("test.value").ShouldBe(456);
    }


    [Fact]
    public void Can_assign_null_string_to_field()
    {
        var hocon = """a=null""";
        Assert.Equal(null, ConfigurationFactory.ParseString(hocon).GetString("a"));
    }

    [Fact(Skip = "we currently do not make any destinction between quoted and unquoted strings once parsed")]
    public void Can_assign_quoted_null_string_to_field()
    {
        var hocon = """
                a="null"
                """;
        Assert.Equal("null", ConfigurationFactory.ParseString(hocon).GetString("a"));
    }


    [Fact]
    public void Can_parse_proto_pid()
    {
        var hocon = """
{
    "pid": 127.0.0.1:60488/$13 ,"topologyHash": "2877904074"
}
""";

        var pid = ConfigurationFactory.ParseString(hocon).GetString("pid");
        Assert.Equal("127.0.0.1:60488/$13", pid);
    }

    [Fact]
    public void Can_parse_cluster_identity()
    {
        var hocon = """
{ "clusterIdentity": echo/7a76d3-8-, "requestId": "761548392f284c3795a3c8c6bbdc7d0d", "topologyHash": "2877904074" }
""";
        var ci = ConfigurationFactory.ParseString(hocon).GetString("clusterIdentity");
        Assert.Equal("echo/7a76d3-8-", ci);
    }

    [Fact]
    public void Can_parse_cs_records()
    {
        var hocon = """
OcppCallMessage { MessageId = 8411a362-734a-4df6-9402-1a8611d99af6, MessageType = Call, Request = GetConfigurationReq { Key = System.Collections.Generic.List`1[System.String] } }
""";
        var x = ConfigurationFactory.ParseString(hocon);
        var t = x.ToString();
    }
}