using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiLibs.Telegram;

namespace TelegramBot.Commands
{
    internal class PingCommand : Command
    {
        private TelegramService tgs;
        private List<string> smartAssAnswers;

        public PingCommand(TelegramService tgs)
        {
            this.tgs = tgs;
            smartAssAnswers = new List<string> { "pong", "pong", "pang", "NO. JUST NO!", "STOP IT", "tableflip"};
        }

        public override async Task<Command> Run(Message m)
        {
            Random r = new Random();
            await tgs.SendMessage(m.chat.id, smartAssAnswers[(int)((double)(smartAssAnswers.Count-1) * r.NextDouble())]);
            return null;
        }

        public override string CommandName
        {
            get { return @"/ping"; }
        }
    }
}