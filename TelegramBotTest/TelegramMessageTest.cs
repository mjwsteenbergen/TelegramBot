using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ApiLibs.Telegram;
using NUnit.Framework;
using TelegramBot;

namespace TelegramBotTest
{
    public class TelegramMessageTest
    {
        private TelegramBot.TelegramBot bot;

        [SetUp]
        public void SetUp()
        {
            bot = new TelegramBot.TelegramBot(new TelegramService("a", "a"), 1);
        }

        [Test]
        public async Task Test()
        {
            var com = new TelegramMessageTestCommand(new TelegramService(null, null));
            string output = "Not triggered";
            com.test = (q, m) =>
            {
                output = q;
                return null;
            };
            bot.Add(com);



            await bot.MessageRecieved(new TgMessage
            {
                text = "/test item",
                from = new From
                {
                    id = 1
                }
            });
            Assert.AreEqual("item", output);
        }



    }

    internal class TelegramMessageTestCommand : Command
    {
        public TelegramMessageTestCommand(TelegramService tgs) : base(tgs)
        {

        }

        public override Task<Command> Run(string query, TgMessage m)
        {
            return Task.FromResult(test.Invoke(query, m));
        }

        public Func<string, TgMessage, Command> test;
        public override string CommandName => "test";


    }
}
