using System;
using System.Collections.Generic;
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

        public override void Run(Message m)
        {
            Random r = new Random();
            tgs.SendMessage(m.chat.id, smartAssAnswers[(int)((double)(smartAssAnswers.Count-1) * r.NextDouble())]);
        }

        public override string CommandName
        {
            get { return @"/ping"; }
        }
    }
}