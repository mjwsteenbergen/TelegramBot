using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ApiLibs.Telegram;

namespace TelegramBot.Commands
{
    public class StartCommand : Command
    {
        private TelegramService tgs;
        private readonly TelegramBot tgb;
        private List<string> smartAssAnswers;

        public StartCommand(TelegramService tgs, TelegramBot tgb) : base(tgs)
        {
            this.tgs = tgs;
            this.tgb = tgb;
        }

        public override Privilege privilege => Privilege.Public;

        public override async Task Run(string query, TgMessage m, IKeyValueStore store)
        {
            var result = Regex.Match(m.text, "/start (.+)_(.+)");

            m.text = "/" + result.Groups[1].Value + " " + result.Groups[2].Value;

            await tgb.MessageRecieved(m);
        }

        public override string CommandName => "start";
    }
}