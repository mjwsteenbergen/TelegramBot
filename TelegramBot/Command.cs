using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiLibs.Telegram;
using CommandLine;
using CommandLine.Text;

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

    public abstract class Command<T> : Command where T : new()
    {
        public Command(TelegramService tgs) : base(tgs) { }

        public override async Task Run(string query, TgMessage m, IKeyValueStore store)
        {
            var result = Parser.Default.ParseArguments<T>(query.Replace("—", "--").Split());
            Task t = new Task(() => { });
            if (query.Contains("—help"))
            {
                var help = HelpText.AutoBuild(result);
                help.Copyright = "";
                help.Heading = "";
                await _tgs.SendMessage(m.from.id, "*Usage:*" + help, ParseMode.Markdown);
            }
            result.WithNotParsed(errs =>
            {
                OnError(m, result, errs);
                t.Start();
            });
            result.WithParsed(async args =>
            {
                await Run(args, m, store);
                t.Start();
            });
            t.Wait();
        }

        private void OnError(TgMessage m, ParserResult<T> result, IEnumerable<Error> errs)
        {
            HelpText.AutoBuild(result, h =>
            {
                h.AddOptions(result);
                h.Heading = "";
                h.Copyright = "";
                Task.Run(async () =>
                {
                    await _tgs.SendMessage(m.from.id, "*I was unable to execute this request:*\n" + SentenceBuilder.Factory().FormatError(errs.ElementAt(0)), ParseMode.Markdown);
                }).Wait();
                return h;
            }, e => e);
        }

        public override Task<IEnumerable<InlineQueryResultArticle>> Run(string query, TgInlineQuery m)
        {
            var result = Parser.Default.ParseArguments<T>(query.Split());
            Task t = new Task(() => { });
            result.WithNotParsed(errs =>
            {
                OnError(m, result, errs);
                t.Start();
            });
            result.WithParsed(async args =>
            {
                await Run(args, m);
                t.Start();
            });
            t.Wait();
            return Task.FromResult<IEnumerable<InlineQueryResultArticle>>(null);
        }

        private void OnError(TgInlineQuery m, ParserResult<T> result, IEnumerable<Error> errs)
        {
            HelpText.AutoBuild(result, h =>
            {
                h.AddOptions(result);
                h.Heading = "";
                h.Copyright = "";
                Task.Run(async () =>
                {
                    await _tgs.AnswerInlineQuery(m.id, new List<InlineQueryResultArticle>
                    {
                        new InlineQueryResultArticle
                        {
                            id = "Error",
                            input_message_content = new InputTextMessageContent(){ message_text = SentenceBuilder.Factory().FormatError(errs.ElementAt(0))},
                            title = SentenceBuilder.Factory().FormatError(errs.ElementAt(0))
                        }
                    });
                }).Wait();
                return h;
            }, e => e);
        }

        public override Task Run(string query, ChosenInlineResult m)
        {
            return null;
        }

        public virtual async Task Run(T query, TgMessage m, IKeyValueStore store)
        {
            await _tgs.SendMessage(m.from.id, "This is not supported for this command");
        }

        public virtual Task Run(T query, TgInlineQuery m)
        {
            return null;
        }

        public virtual Task Run(T query, ChosenInlineResult m)
        {
            return null;
        }
    }
}