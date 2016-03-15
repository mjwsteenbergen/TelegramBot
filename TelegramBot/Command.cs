using ApiLibs.Telegram;

namespace TelegramBot
{
    public abstract class Command
    {
        public abstract void Run(Message m);
        public abstract string CommandName { get; }
    }
}