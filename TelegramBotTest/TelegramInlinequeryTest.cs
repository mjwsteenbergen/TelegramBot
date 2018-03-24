using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ApiLibs.Telegram;
using NUnit.Framework;
using TelegramBot;

namespace TelegramBotTest {
    public class TelegramInlineQueryTest {

        private TelegramBot.TelegramBot bot;

        [SetUp]
        public void SetUp()
        {
            bot = new TelegramBot.TelegramBot(null, 1);
        }

        [Test]
        public async Task Test() {
            var com = new TelegramInlineMessageTestCommand(new TelegramService(null, null));
            string output = "Not triggered";
            com.test = (q, m) =>
            {
                output = q;
                return null;
            };
            bot.Add(com);



            await bot.MessageRecieved(new TgInlineQuery
            {
                query = "test item",
                from = new From
                {
                    id = 1
                }
            });
            Assert.AreEqual("item", output);
        }

        [Test]
        public void TestHelp()
        {
            var com = new TelegramInlineMessageTestCommand(new TelegramService(null, null));
            string output = "Not triggered";
            com.test = (q, m) =>
            {
                output = q;
                return null;
            };
            bot.Add(com);
            bot.Add(new TelegramMessageTestCommand(new TelegramService(null, null)));


            var s = bot.GetAllCommands(new TgInlineQuery
            {
                query = "",
                from = new From
                {
                    id = 1
                }
            });
            Assert.AreEqual(1, s.Count);
        }


        public async Task TestNotExistantCommand()
        {
            var com = new TelegramInlineMessageTestCommand(new TelegramService(null, null));
            string output = "Not triggered";
            com.test = (q, m) =>
            {
                output = q;
                return null;
            };
            bot.Add(com);

            await bot.MessageRecieved(new TgInlineQuery
            {
                query = "test2 item",
                from = new From
                {
                    id = 1
                }
            });
            Assert.AreNotEqual("item", output);
        }


        internal class TelegramInlineMessageTestCommand : Command
        {
            public TelegramInlineMessageTestCommand(TelegramService tgs) : base(tgs)
            {

            }

            public override Task<IEnumerable<InlineQueryResultArticle>> Run(string query, TgInlineQuery m) {
                test.Invoke(query, m);
                return Task.FromResult<IEnumerable<InlineQueryResultArticle>>(null);
            }

            public Func<string, TgInlineQuery, Command> test;
            public override string CommandName => "test";


        }
    }

}