using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiLibs.Telegram;

namespace TelegramBot.Commands
{
    class IdCommand : Command
    {
        public IdCommand(TelegramService tgs) : base(tgs)
        {

        }

        public override async Task Run(string query, TgMessage m, IKeyValueStore store)
        {
            await _tgs.SendMessage(m.chat.id, m.from.id.ToString());
        }

        public override string CommandName => "getId";
    }
}
