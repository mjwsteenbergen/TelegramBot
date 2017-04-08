using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiLibs.Telegram;

namespace TelegramBot
{
    class ExampleCommand : Command
    {
        public ExampleCommand(TelegramService tgs) : base(tgs)
        {
        }

        public override async Task<Command> Run(string query, TgInlineQuery m)
        {
            await _tgs.answerInlineQuery(m.id, new List<InlineQueryResultArticle>
            {
                new InlineQueryResultArticle
                {
                    id = "Id_of_item",
                    title = "Example name",
                    input_message_content = new InputTextMessageContent
                    {
                        message_text = "Text that is displayed when tapped"
                    }
                }
            });
            return null;
        }

        public override async Task<Command> Run(string query, ChosenInlineResult m)
        {
            await _tgs.SendMessage(m.from.id, "You chose " + m.result_id + " from query " + query + " official query: " + m.query);
            return null;
        }

        public override string CommandName => "Example";
    }
}
