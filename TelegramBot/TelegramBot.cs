using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ApiLibs.General;
using ApiLibs.Telegram;
using ApiLibs.Todoist;
using TelegramBot.Commands;

namespace TelegramBot
{
    public class TelegramBot
    {
        private readonly TelegramService _tgs;
        public static int admin { get; private set; }

        private static readonly string ApplicationDataPath =
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar +
            "TelegramBot" + Path.DirectorySeparatorChar;

        List<Command> actionLib;
        private IDatabaseValueStore database;

        public static void Main(string[] args)
        {
            Passwords passwords = Passwords.ReadPasswords(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar +
            "Laurentia" + Path.DirectorySeparatorChar);

            TelegramService tgs = new TelegramService(passwords.Telegram_token, ApplicationDataPath);

            TelegramBot bot = new TelegramBot(tgs, -1);
            Task.Run(async () =>
            {
                await bot.Run();
            }).Wait();
        }

        public TelegramBot(TelegramService tgs, int adminId, IDatabaseValueStore valueStore = null)
        {
            admin = adminId;
            _tgs = tgs;
            actionLib = new List<Command>();
            database = valueStore;
        }

        /// <summary>
        /// Runs the bot
        /// WARNING: Infinite loop
        /// </summary>
        public async Task Run()
        {
            try
            {
                await _tgs.SendMessage(admin, "TelegramBot is online");
                while (true)
                {
                    try
                    {
                        //TgMessages messages = await _tgs.GetMessages(1000);
                        //await MessageRecieved(messages);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                  
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await _tgs.SendMessage(admin, "*Telegrambot encountered fatal error:* " + e.Message, ParseMode.Markdown);
                await _tgs.SendMessage(admin, "Stacktrace: \n" + e.StackTrace);
            }
        }

        public void Add(Command com)
        {
            actionLib.Add(com);
        }


        public async Task MessageRecieved(TgMessage message)
        {
            IKeyValueStore store = null;
            if (database != null)
            {
                store = await database.GetUserData(message.from.id);
                if (store.Get("ReturnFunction") != null && store.Get("ReturnFunction") != "")
                {
                    await store.Set("ReturnFunction", "");
                }
            }

            Match match = Regex.Match(message.text, @"^/(\w+)");
            if (match.Success)
            {
                string commandName = match.Groups[1].Value;
                var res = ConvertToCommand(commandName);
                if (res != null && HasPrivilige(message.from.id, res))
                {
                    var args = message.text.Replace("/" + commandName, "");
                    if (args.StartsWith(" "))
                    {
                        args = args.Remove(0, 1);
                    }
                    await res.Run(args, message, store);
                }
            }
            else
            {
                if (store.Get("ReturnFunction") != null && store.Get("ReturnFunction") != "")
                {
                    var res = ConvertToCommand(store.Get("ReturnFunction").Split('?')[0]);
                    if (res != null)
                    {
                        await res.Run(message.text, message, store);
                    }
                }
            }
        }

        public async Task MessageRecieved(TgInlineQuery inlineQuery)
        {

            Match match = Regex.Match(inlineQuery.query, @"^(\w+)");
            if (match.Success)
            {
                string commandName = match.Groups[1].Value;
                var res = ConvertToCommand(commandName);
                if (res != null && HasPrivilige(inlineQuery.from.id, res))
                {
                    var args = inlineQuery.query.Replace(commandName, "");
                    if (args.StartsWith(" "))
                    {
                        args = args.Remove(0, 1);
                    }
                    await res.Run(args, inlineQuery);
                }
            }
            else
            {
                if (inlineQuery.query.Replace(" ", "") == "")
                {
                    await _tgs.AnswerInlineQuery(inlineQuery.id, new List<InlineQueryResultArticle>
                    {
                        new InlineQueryResultArticle
                        {
                            id = "1",
                            title = "This command is not recognized",
                            input_message_content = new InputTextMessageContent
                            {
                                message_text = "NotFound"
                            }
                        }
                    });
                }
            }
        }

        public async Task MessageRecieved(ChosenInlineResult inlineResult)
        {
            Command c;
            Match match = Regex.Match(inlineResult.query, @"^(\w+)");
            if (match.Success)
            {
                string commandName = match.Groups[1].Value;
                var res = ConvertToCommand(commandName);
                if (res != null)
                {
                    await res.Run(inlineResult.query.Replace(commandName + " ", ""), inlineResult);
                }
            }
            
        }

        private Command ConvertToCommand(string commandName)
        {
            if (commandName == null)
            {
                return null;
            }

            return actionLib.Find(i => i.CommandName == commandName);
        }

        public bool HasPrivilige(int fromId, Command c)
        {
            switch (c.privilege)
            {
                case Privilege.Admin:
                    return fromId == admin;
                case Privilege.BlackList:
                    return !c.PrivilegeList.Contains(fromId);
                case Privilege.WhiteList:
                    if (fromId == admin) return true;
                    return c.PrivilegeList.Contains(fromId);
                case Privilege.Public:
                    return true;
                default:
                    return false;
            }
        }
    }
}
