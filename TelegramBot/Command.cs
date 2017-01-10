using System;
using System.Threading.Tasks;
using ApiLibs.Telegram;

namespace TelegramBot
{
    public abstract class Command
    {
        public abstract Task<Command> Run(string query, Message m);
        public abstract string CommandName { get; }
    }
}