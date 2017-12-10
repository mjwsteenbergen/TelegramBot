using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ApiLibs.Telegram;
using NSubstitute;
using NUnit.Framework;
using TelegramBot;

namespace TelegramBotTest
{
    [TestFixture]
    public class KeyValueStoreTest
    {
        [Test]
        public async Task Test()
        {
            var s = Substitute.For<IDatabaseValueStore>();
            var kvs = Substitute.For<IKeyValueStore>();
            s.GetUserData(1).Returns(Task.FromResult(kvs));
            var bot = new TelegramBot.TelegramBot(null, 1, s);
            await bot.MessageRecieved(new ApiLibs.Telegram.TgMessage
            {
                from = new ApiLibs.Telegram.From
                {
                    id = 1
                },
                text = ""
            });
            await s.Received().GetUserData(1);
        }

        [Test]
        public async Task TestReturnFunction()
        {
            var s = Substitute.For<IDatabaseValueStore>();
            var kvs = Substitute.For<IKeyValueStore>();
            kvs.Get("ReturnFunction").Returns("app");
            s.GetUserData(1).Returns(Task.FromResult(kvs));
            var bot = new TelegramBot.TelegramBot(null, 1, s);

            var app = new ComApp(null);
            var nonapp = new NonComApp(null);

            bot.Add(app);
            bot.Add(nonapp);

            await bot.MessageRecieved(new ApiLibs.Telegram.TgMessage
            {
                from = new ApiLibs.Telegram.From
                {
                    id = 1
                },
                text = ""
            });
            await s.Received().GetUserData(1);

            Assert.IsTrue(app.run);
            Assert.IsFalse(nonapp.run);
        }

        [Test]
        public async Task TestReturnFunctionRemovesLast()
        {
            var s = Substitute.For<IDatabaseValueStore>();
            var kvs = Substitute.For<IKeyValueStore>();
            kvs.Get("ReturnFunction").Returns("app");
            s.GetUserData(1).Returns(Task.FromResult(kvs));
            var bot = new TelegramBot.TelegramBot(null, 1, s);

            var app = new ComApp(null);
            var nonapp = new NonComApp(null);

            bot.Add(app);
            bot.Add(nonapp);

            await bot.MessageRecieved(new ApiLibs.Telegram.TgMessage
            {
                from = new ApiLibs.Telegram.From
                {
                    id = 1
                },
                text = ""
            });
            await s.Received().GetUserData(1);

            await kvs.Received().Set("ReturnFunction", "");
        }

        [Test]
        public async Task TestReturnFunctionIsNotRemovedWhenAlreadyEmpty()
        {
            var s = Substitute.For<IDatabaseValueStore>();
            var kvs = Substitute.For<IKeyValueStore>();
            kvs.Get("ReturnFunction").Returns("");
            s.GetUserData(1).Returns(Task.FromResult(kvs));
            var bot = new TelegramBot.TelegramBot(null, 1, s);

            var app = new ComApp(null);

            bot.Add(app);

            await bot.MessageRecieved(new TgMessage
            {
                from = new From
                {
                    id = 1
                },
                text = ""
            });
            await s.Received().GetUserData(1);

            await kvs.DidNotReceive().Set("ReturnFunction", "");
        }
    }

    class ComApp : Command
    {
        public override string CommandName => "app";
        public bool run = false;

        public ComApp(TelegramService tgs) : base(tgs)
        {
        }

        public override Task Run(string query, TgMessage m, IKeyValueStore store)
        {
            run = true;
            return Task.CompletedTask;
        }
    }

    class NonComApp :Command
    {
        public bool run = false;

        public NonComApp(TelegramService tgs) : base(tgs)
        {
        }

        public override string CommandName => "napp";

        public override Task Run(string query, TgMessage m, IKeyValueStore store)
        {
            run = true;
            return Task.CompletedTask;
        }
    }
}
