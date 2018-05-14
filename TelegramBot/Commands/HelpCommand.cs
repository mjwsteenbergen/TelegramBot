using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiLibs.Telegram;

namespace TelegramBot.Commands
{
    public class HelpCommand : Command
    {
        private List<Command> ActionList { get; }

        public HelpCommand(TelegramService tgs, List<Command> actionList) : base(tgs)
        {
            ActionList = actionList;
        }

        public override Privilege privilege => Privilege.Public;

        public override async Task Run(string query, TgMessage m, IKeyValueStore store)
        {
            string helpText = ActionList
                .Where(i => TelegramBot.HasPrivilige(m.from.id, i))
                .Where(i => i.GetType().GetMethod("Run", new[] { typeof(string), typeof(TgMessage), typeof(IKeyValueStore) }).DeclaringType == i.GetType())
                .Select(i => "/" + i.CommandName + "\n")
                .Aggregate((i,j) => i + j);

            await _tgs.SendMessage(m.from.id, "The following commands are available to you: \n" + helpText);
        }

        public override string CommandName => "help";
    }
}
