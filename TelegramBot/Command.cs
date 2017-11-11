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
            return Task.FromResult((Command) null);
        }

        public virtual Task<Command> Run(string query, ChosenInlineResult m)
        {
            return Task.FromResult((Command) null);
        }

        public abstract string CommandName { get; }
    }

    public abstract class Command<T> : Command where T : new()
    {
        public Command(TelegramService tgs) : base(tgs) { }

        public override async Task<Command> Run(string query, TgMessage m)
        {
            var result = Parser.Default.ParseArguments<T>(query.Replace("—", "--").Split());
            Task t = new Task(() => { });
            if (query.Contains("—help"))
            {
                var help = HelpText.AutoBuild(result);
                help.Copyright = "";
                help.Heading = "";
                await _tgs.SendMessage(m.from.id, "*Usage:*" + help, ParseMode.Markdown);
                return null;
            }
            Command o = null;
            result.WithNotParsed(errs =>
            {
                OnError(m, result, errs);
                t.Start();
            });
            result.WithParsed(async args =>
            {
                o = await Run(args, m);
                t.Start();
            });
            t.Wait();
            return o;
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

        public override Task<Command> Run(string query, TgInlineQuery m)
        {
            var result = Parser.Default.ParseArguments<T>(query.Split());
            Command o = null;
            Task t = new Task(() => { });
            result.WithNotParsed(errs =>
            {
                OnError(m, result, errs);
                t.Start();
            });
            result.WithParsed(async args =>
            {
                o = await Run(args, m);
                t.Start();
            });
            t.Wait();
            return Task.FromResult(o);
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

        public Task<Command> Run(string query, ChosenInlineResult m)
        {
            return null;
        }

        public virtual async Task<Command> Run(T query, TgMessage m)
        {
            await _tgs.SendMessage(m.from.id, "This is not supported for this command");
            return null;
        }

        public virtual Task<Command> Run(T query, TgInlineQuery m)
        {
            return null;
        }

        public virtual Task<Command> Run(T query, ChosenInlineResult m)
        {
            return null;
        }
    }
}