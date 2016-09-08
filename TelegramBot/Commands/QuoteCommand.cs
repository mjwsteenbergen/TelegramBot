﻿using System;
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

        public override async Task<Command> Run(Message m)
        {
            if (m.reply_to_message != null && m.reply_to_message != null)
            {
                await _tg.SendMessage(m.chat.id, "\"" + m.reply_to_message.text + "\" - " + m.reply_to_message.@from.first_name + " " + DateTime.Now.Year);
            }
            return null;
        }

        public override string CommandName => @"/quote";
    }
}
