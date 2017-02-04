using System.Threading.Tasks;
using ApiLibs.Telegram;
using ApiLibs.Todoist;

namespace TelegramBot.Commands
{
    internal class TodoCommand : Command
    {
        private TodoistService ts;
        private readonly TelegramService _tgs;

        public TodoCommand(TodoistService ts, TelegramService tgs) : base(tgs)
        {
            this.ts = ts;
            _tgs = tgs;
        }

        public override async Task<Command> Run(string query, TgMessage m)
        {
            if (m.reply_to_message != null)
            {
                await ts.AddTodo(m.reply_to_message.text);
            }
            else
            {
                await ts.AddTodo(query);
            }

            await _tgs.SendMessage(m.from.id, "Todo was added");

            return null;
        }

        public override string CommandName => "todo";
    }
}