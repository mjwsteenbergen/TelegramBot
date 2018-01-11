using System;
using System.Collections.Generic;
using System.Text;
using TelegramBot;
using NUnit.Framework;
using UriParser = TelegramBot.UriParser;

namespace TelegramBotTest
{
    class UriParserTest
    {
        [Test]
        public void UrlWithNoParameters()
        {
            UriParser parser = new UriParser("parameterName");
            Assert.AreEqual("parameterName", parser.Command);
        }

        [Test]
        public void UrlWithOneParameterTest()
        {
            UriParser parser = new UriParser("command?param=value");
            var param = parser.Find("param");
            Assert.NotNull(param);
            Assert.AreEqual("value", param.value);
        }

        [Test]
        public void UrlWithOneParameterWithLength1Test()
        {
            UriParser parser = new UriParser("command?param=v");
            var param = parser.Find("param");
            Assert.NotNull(param);
            Assert.AreEqual("v", param.value);
        }

        [Test]
        public void UrlWithOneParameterWithNLength1Test()
        {
            UriParser parser = new UriParser("command?p=value");
            var param = parser.Find("p");
            Assert.NotNull(param);
            Assert.AreEqual("value", param.value);
        }

        [Test]
        public void UrlWithMultipleValuesTest()
        {
            UriParser parser = new UriParser("command?param=value&param2=value2");
            var param = parser.Find("param2");
            Assert.NotNull(param);
            Assert.AreEqual("value2", param.value);
        }

        [Test]
        public void UrlWithEscapedValueTest()
        {
            UriParser parser = new UriParser("value");
            parser.Set("param", "value", true);
            UriParser parser2 = new UriParser(parser.ToUrl());
            var param = parser2.Find("param");
            Assert.NotNull(param);
            Assert.AreEqual("value", param.value);
        }

        [Test]
        public void UrlWithEqualsVarTest()
        {
            UriParser parser = new UriParser("value");
            parser.Set("param", "this is an = sign", true);
            UriParser parser2 = new UriParser(parser.ToUrl());
            var param = parser2.Find("param");
            Assert.NotNull(param);
            Assert.AreEqual("this is an = sign", param.value);
        }

        [Test]
        public void UrlWithEmpercantVarTest()
        {
            UriParser parser = new UriParser("value");
            parser.Set("param", "this is an & sign", true);
            var url = parser.ToUrl();
            UriParser parser2 = new UriParser(url);
            var param = parser2.Find("param");
            Assert.NotNull(param);
            Assert.AreEqual("this is an & sign", param.value);
        }

        [Test]
        public void UrlWithQuoteVarTest()
        {
            UriParser parser = new UriParser("value");
            parser.Set("param", "this is an \" sign", true);
            var url = parser.ToUrl();
            UriParser parser2 = new UriParser(url);
            var param = parser2.Find("param");
            Assert.NotNull(param);
            Assert.AreEqual("this is an ' sign", param.value);
        }


        [Test]
        public void ParsingDataTest()
        {
            UriParser parser = new UriParser("value");
            parser.Set("param", new Example
            {
                a = "hello",
                b = "hi"
            });
            var url = parser.ToUrl();
            UriParser parser2 = new UriParser(url);
            var param = parser2.Find<Example>("param");
            Assert.NotNull(param);
            Assert.AreEqual("hello", param.a);
            Assert.AreEqual("hi", param.b);
        }

        [Test]
        public void ParsingDataWithWrongDataTest()
        {
            UriParser parser = new UriParser("value");
            parser.Set("param", new Example
            {
                a = "&=&&=",
                b = "\"\"\"\"\"\"\"\"\"\""
            });
            var url = parser.ToUrl();
            UriParser parser2 = new UriParser(url);
            var param = parser2.Find<Example>("param");
            Assert.NotNull(param);
            Assert.AreEqual("&=&&=", param.a);
        }

        [Test]
        public void ParsingDataWithSingleQuoteTest()
        {
            UriParser parser = new UriParser("value");
            parser.Set("param", new Example
            {
                a = "It's fun",
            });
            var url = parser.ToUrl();
            UriParser parser2 = new UriParser(url);
            var param = parser2.Find<Example>("param");
            Assert.NotNull(param);
            Assert.AreEqual("It's fun", param.a);
        }

        [Test]
        public void ParsingDataMultipleTimesTest()
        {
            UriParser parser = new UriParser("value");
            parser.Set("param", new Example
            {
                a = "It's fun&=",
            });
            var url = parser.ToUrl();
            UriParser parser2 = new UriParser(url);
            var url2 = parser2.ToUrl();
            UriParser parser3 = new UriParser(url2);
            Assert.AreEqual("It's fun&=", parser3.Find<Example>("param")?.a);
        }

        [Test]
        public void ParsingMultipleValuesTest()
        {
            UriParser parser = new UriParser("value");
            parser.Set("param", new Example
            {
                a = "It's fun&=",
            });

            parser.Set("a", "b");
            parser.Set("ab", "bc", true);

            UriParser parser2 = new UriParser(parser.ToUrl());
            Assert.AreEqual("It's fun&=", parser2.Find<Example>("param")?.a);
        }

    }

    class Example
    {
        public string a;
        public string b;
    }
}
