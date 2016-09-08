using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiLibs.Telegram;

namespace TelegramBot.Commands
{
    class OvCommand : Command
    {
        private TelegramService tgs;

        public OvCommand(TelegramService tgs)
        {
            this.tgs = tgs;
        }

        public override async Task<Command> Run(Message m)
        {
            ReplyKeyboardMarkup markup = new ReplyKeyboardMarkup
            {
                keyboard = 
                new [] {
                     new [] { new KeyboardButton { text = "Yes" } , new KeyboardButton { text = "No"} }
                },
                one_time_keyboard = true
            };

            await tgs.SendMessage(m.from.id, "Would you like to be notified of your stops?", replyMarkup: markup);
            return null;
        }

        public override string CommandName => @"/ov";
    }
}
