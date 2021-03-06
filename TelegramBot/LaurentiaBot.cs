﻿using System;
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
    public class LaurentiaBot
    {
        private readonly TelegramService _tgs;
        private int admin = -1;

        private static readonly string ApplicationDataPath =
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar +
            "TelegramBot" + Path.DirectorySeparatorChar;

        Dictionary<string, Command> actionLib;
        private Dictionary<int, Command> currentConv;

        public static void Main(string[] args)
        {
            Passwords passwords = Passwords.ReadPasswords(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar +
            "Laurentia" + Path.DirectorySeparatorChar);

            TelegramService tgs = new TelegramService(passwords.Telegram_token, ApplicationDataPath);
            TodoistService todoist = new TodoistService(passwords.TodoistKey, passwords.TodoistUserAgent);

            LaurentiaBot bot = new LaurentiaBot(tgs, todoist, -1);
            Task.Run(async () =>
            {
                await bot.Run();
            }).Wait();
        }

        public LaurentiaBot(TelegramService tgs, TodoistService ts, int adminId)
        {
            admin = adminId;
            _tgs = tgs;
            currentConv = new Dictionary<int, Command>();
            actionLib = new Dictionary<string, Command>();
           
            Add(new TodoCommand(ts, tgs));
            Add(new PingCommand(tgs));
            Add(new QuoteCommand(tgs));
            Add(new ExampleCommand(tgs));
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
                        TgMessages messages = await _tgs.GetMessages(1000);
                        await MessageRecieved(messages, EventArgs.Empty);
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
            actionLib.Add(com.CommandName, com);
        }

        private async Task MessageRecieved(TgMessages m, EventArgs e)
        {
            Command c;

            foreach (TgMessage message in m.Messages)
            {
                try
                {
                    if (currentConv.TryGetValue(message.from.id, out c) && c != null)
                    {
                        currentConv.Add(message.from.id, await c.Run(message.text, message));
                        continue;
                    }

                    Match match = Regex.Match(message.text, @"^/(\w+)");

                    if (match.Success && actionLib.TryGetValue(match.Groups[1].Value, out c))
                    {
                        string args = message.text.Replace("/" + match.Groups[1].Value + " ", "");

                        currentConv[message.from.id] = await c.Run(args, message);
                    }
                }
                catch (Exception exception)
                {
                    await HandleException(exception);
                }
            }
            
            foreach (TgInlineQuery inlineQuery in m.tgInlineQueries)
            {
                try
                {
                    Match match = Regex.Match(inlineQuery.query, @"^(\w+)");

                    if (match.Success && actionLib.TryGetValue(match.Groups[1].Value, out c))
                    {
                        currentConv[inlineQuery.from.id] = await c.Run(inlineQuery.query.Replace(match.Groups[1].Value + " ", ""), inlineQuery);
                    }
                }
                catch (Exception exception)
                {
                    await HandleException(exception);
                }
            }


            foreach (ChosenInlineResult inlineResult in m.ChosenInlineResults)
            {
                try
                {
                    Match match = Regex.Match(inlineResult.query, @"^(\w+)");

                    if (match.Success && actionLib.TryGetValue(match.Groups[1].Value, out c))
                    {
                        currentConv[inlineResult.from.id] = await c.Run(inlineResult.query.Replace(match.Groups[1].Value + " ", ""), inlineResult);
                    }
                }
                catch (Exception exception)
                {
                    await HandleException(exception);
                }
                
            }
        }

        public async Task HandleException(Exception e)
        {
            Console.WriteLine(e);
            await _tgs.SendMessage(admin, "*Telegrambot encountered error while handling message:*" + e.Message, ParseMode.Markdown);
            await _tgs.SendMessage(admin, e.StackTrace);
        }
    }
}
