using System;
using System.Collections.Generic;
using System.Text;
using TelegramBot;
using NUnit.Framework;

namespace TelegramBotTest
{
    class UriHandlerTest
    {
        [Test]
        public void UrlWithNoParameters()
        {
            UriHandler parser = new UriHandler("parameterName");
            Assert.AreEqual("parameterName", parser.Command);
        }

        [Test]
        public void UrlWithOneParameterTest()
        {
            UriHandler parser = new UriHandler("command?param=value");
            var param = parser.GetParameterValue("param");
            Assert.NotNull(param);
            Assert.AreEqual("value", param);
        }

        [Test]
        public void UrlWithOneParameterWithLength1Test()
        {
            UriHandler parser = new UriHandler("command?param=v");
            var param = parser.GetParameterValue("param");
            Assert.NotNull(param);
            Assert.AreEqual("v", param);
        }

        [Test]
        public void UrlWithOneParameterWithNLength1Test()
        {
            UriHandler parser = new UriHandler("command?p=value");
            var param = parser.GetParameterValue("p");
            Assert.NotNull(param);
            Assert.AreEqual("value", param);
        }

        [Test]
        public void UrlWithMultipleValuesTest()
        {
            UriHandler parser = new UriHandler("command?param=value&param2=value2");
            var param = parser.GetParameterValue("param2");
            Assert.NotNull(param);
            Assert.AreEqual("value2", param);
        }

        [Test]
        public void UrlWithEscapedValueTest()
        {
            UriHandler parser = new UriHandler("value");
            parser.Set("param", "value", true);
            UriHandler parser2 = new UriHandler(parser.ToUrl());
            var param = parser2.GetParameterValue("param");
            Assert.NotNull(param);
            Assert.AreEqual("value", param);
        }

        [Test]
        public void UrlWithEqualsVarTest()
        {
            UriHandler parser = new UriHandler("value");
            parser.Set("param", "this is an = sign", true);
            UriHandler parser2 = new UriHandler(parser.ToUrl());
            var param = parser2.GetParameterValue("param");
            Assert.NotNull(param);
            Assert.AreEqual("this is an = sign", param);
        }

        [Test]
        public void UrlWithEmpercantVarTest()
        {
            UriHandler parser = new UriHandler("value");
            parser.Set("param", "this is an & sign", true);
            var url = parser.ToUrl();
            UriHandler parser2 = new UriHandler(url);
            var param = parser2.GetParameterValue("param");
            Assert.NotNull(param);
            Assert.AreEqual("this is an & sign", param);
        }

        [Test]
        public void UrlWithQuoteVarTest()
        {
            UriHandler parser = new UriHandler("value");
            parser.Set("param", "this is an \" sign", true);
            var url = parser.ToUrl();
            UriHandler parser2 = new UriHandler(url);
            var param = parser2.GetParameterValue("param");
            Assert.NotNull(param);
            Assert.AreEqual("this is an ' sign", param);
        }


        [Test]
        public void ParsingDataTest()
        {
            UriHandler parser = new UriHandler("value");
            parser.Set("param", new Example
            {
                a = "hello",
                b = "hi"
            });
            var url = parser.ToUrl();
            UriHandler parser2 = new UriHandler(url);
            var param = parser2.GetParameterValue<Example>("param");
            Assert.NotNull(param);
            Assert.AreEqual("hello", param.a);
            Assert.AreEqual("hi", param.b);
        }

        [Test]
        public void ParsingDataWithWrongDataTest()
        {
            UriHandler parser = new UriHandler("value");
            parser.Set("param", new Example
            {
                a = "&=&&=",
                b = "\"\"\"\"\"\"\"\"\"\""
            });
            var url = parser.ToUrl();
            UriHandler parser2 = new UriHandler(url);
            var param = parser2.GetParameterValue<Example>("param");
            Assert.NotNull(param);
            Assert.AreEqual("&=&&=", param.a);
        }

        [Test]
        public void ParsingDataWithSingleQuoteTest()
        {
            UriHandler parser = new UriHandler("value");
            parser.Set("param", new Example
            {
                a = "It's fun",
            });
            var url = parser.ToUrl();
            UriHandler parser2 = new UriHandler(url);
            var param = parser2.GetParameterValue<Example>("param");
            Assert.NotNull(param);
            Assert.AreEqual("It's fun", param.a);
        }

        [Test]
        public void ParsingDataMultipleTimesTest()
        {
            UriHandler parser = new UriHandler("value");
            parser.Set("param", new Example
            {
                a = "It's fun&=",
            });
            var url = parser.ToUrl();
            UriHandler parser2 = new UriHandler(url);
            var url2 = parser2.ToUrl();
            UriHandler parser3 = new UriHandler(url2);
            Assert.AreEqual("It's fun&=", parser3.GetParameterValue<Example>("param")?.a);
        }

        [Test]
        public void ParsingMultipleValuesTest()
        {
            UriHandler parser = new UriHandler("value");
            parser.Set("param", new Example
            {
                a = "It's fun&=",
            });

            parser.Set("a", "b");
            parser.Set("ab", "bc", true);

            UriHandler parser2 = new UriHandler(parser.ToUrl());
            Assert.AreEqual("It's fun&=", parser2.GetParameterValue<Example>("param")?.a);
        }

    }

    class Example
    {
        public string a;
        public string b;
    }
}
