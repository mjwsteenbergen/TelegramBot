using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ApiLibs.Telegram;
using ApiLibs.Todoist;
using TelegramBot.Commands;

namespace TelegramBot
{
    public class LaurentiaBot
    {
        Dictionary<string, Command> actionLib;

        public LaurentiaBot(TelegramService tgs, TodoistService ts)
        {
            tgs.MessageRecieved += MessageRecieved;

            actionLib = new Dictionary<string, Command>();
            Add(new TodoCommand(ts));
            Add(new PingCommand(tgs));
            Add(new QuoteCommand(tgs));

        }

        private void Add(Command com)
        {
            actionLib.Add(com.CommandName, com);
        }

        private void MessageRecieved(Message m, EventArgs e)
        {
            Command c;

            Match match = Regex.Match(m.text, @"^/\w+");

            if (match.Success && actionLib.TryGetValue(match.Groups[0].Value, out c))
            {
                c.Run(m);
            }
        }
    }
}
