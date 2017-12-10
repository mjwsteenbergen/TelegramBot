using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiLibs.Telegram;
using TelegramBot;

namespace TelegramBot.Commands
{
    public class QuoteCommand : Command
    {
        public QuoteCommand(TelegramService tgs) : base (tgs)
        { }

        public override Privilege privilege => Privilege.Public;

        public override async Task Run(string query, TgMessage m, IKeyValueStore store)
        {
            if (m.reply_to_message != null && m.reply_to_message != null)
            {
                await _tgs.SendMessage(m.chat.id, "\"" + m.reply_to_message.text + "\" - " + m.reply_to_message.@from.first_name + " " + DateTime.Now.Year);
            }
        }

        public override string CommandName => "quote";
    }
}
