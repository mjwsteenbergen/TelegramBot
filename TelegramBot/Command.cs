using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiLibs.Telegram;

namespace TelegramBot
{
    public abstract class Command
    {
        protected internal TelegramService _tgs;

        public virtual Privilege privilege => Privilege.Admin;

        public List<int> PrivilegeList = new List<int>();

        public Command(TelegramService tgs)
        {
            _tgs = tgs;
        }

        public virtual async Task Run(string query, TgMessage m, IKeyValueStore store)
        {
            await _tgs.SendMessage(m.from.id, "This is not supported for this command");
        }

        public virtual Task<IEnumerable<InlineQueryResultArticle>> Run(string query, TgInlineQuery m)
        {
            return Task.FromResult<IEnumerable<InlineQueryResultArticle>>(new List<InlineQueryResultArticle>());
        }

        public virtual Task Run(string query, ChosenInlineResult m)
        {
            return Task.CompletedTask;
        }

        public abstract string CommandName { get; }
    }

    public enum Privilege
    {
        Admin, WhiteList, BlackList, Public
    }
}