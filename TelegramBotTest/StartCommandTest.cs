using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ApiLibs.Telegram;
using NUnit.Framework;
using TelegramBot;
using TelegramBot.Commands;

namespace TelegramBotTest
{

    public class StartCommandTest
    {
        private TelegramBot.TelegramBot bot;

        [SetUp]
        public void SetUp()
        {
            TelegramService tgs = new TelegramService("a", "a");
            bot = new TelegramBot.TelegramBot(tgs, 1);
            bot.Add(new StartCommand(tgs, bot));
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
                text = "/start test_pong",
                from = new From
                {
                    id = 1
                }
            });
            Assert.AreEqual("pong", output, "Was: " + output);
        }



    }


}
