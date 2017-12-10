using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiLibs.Telegram;

namespace TelegramBot.Commands
{
    public class PingCommand : Command
    {
        private TelegramService tgs;
        private List<string> smartAssAnswers;

        public PingCommand(TelegramService tgs) : base(tgs)
        {
            this.tgs = tgs;
            smartAssAnswers = new List<string> { "pong", "pong", "pang", "NO. JUST NO!", "STOP IT", "tableflip"};
        }

        public override Privilege privilege => Privilege.Public;

        public override async Task Run(string query, TgMessage m, IKeyValueStore store)
        {
            Random r = new Random();
            await tgs.SendMessage(m.chat.id, smartAssAnswers[(int)((double)(smartAssAnswers.Count-1) * r.NextDouble())]);
        }

        public override string CommandName => "ping";
    }
}