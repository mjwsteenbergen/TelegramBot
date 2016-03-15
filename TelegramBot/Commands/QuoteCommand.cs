using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiLibs.Telegram;
using TelegramBot;

namespace TelegramBot.Commands
{
    class QuoteCommand : Command
    {
        private readonly TelegramService _tg;

        public QuoteCommand(TelegramService tg)
        {
            _tg = tg;
        }

        public override void Run(Message m)
        {
            if (m.reply_to_message != null && m.reply_to_message != null)
            {
                _tg.SendMessage(m.chat.id, "\"" + m.reply_to_message.text + "\" - " + m.reply_to_message.@from.first_name + " " + DateTime.Now.Year);
            }   
        }

        public override string CommandName
        {
            get { return @"/quote"; }
        }
    }
}
