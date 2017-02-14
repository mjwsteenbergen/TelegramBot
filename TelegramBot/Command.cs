using System;
using System.Threading.Tasks;
using ApiLibs.Telegram;

namespace TelegramBot
{
    public abstract class Command
    {
        protected internal TelegramService _tgs;

        public Command(TelegramService tgs)
        {
            _tgs = tgs;
        }

        public virtual async Task<Command> Run(string query, TgMessage m)
        {
            await _tgs.SendMessage(m.from.id, "This is not supported for this command");
            return null;
        }

        public virtual Task<Command> Run(string query, TgInlineQuery m)
        {
            return null;
        }

        public virtual Task<Command> Run(string query, ChosenInlineResult m)
        {
            return null;
        }

        public abstract string CommandName { get; }
    }
}