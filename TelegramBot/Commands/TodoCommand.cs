using System.Threading.Tasks;
using ApiLibs.Telegram;
using ApiLibs.Todoist;

namespace TelegramBot.Commands
{
    internal class TodoCommand : Command
    {
        private TodoistService ts;
        public TodoCommand(TodoistService ts)
        {
            this.ts = ts;
        }

        public override async Task<Command> Run(Message m)
        {
            if (m.reply_to_message != null)
            {
                await ts.AddTodo(m.reply_to_message.text);
            }
            else
            {
                await ts.AddTodo(m.text.Replace("/todo", ""));
            }
            return null;
        }

        public override string CommandName => @"/todo";
    }
}